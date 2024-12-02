namespace ElgatoLightApiClient.Services
{
    using System.Collections.Immutable;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    internal class DeviceDiscovery
    {
        private static readonly Int16 Port = 9123;

        private static readonly TimeSpan ScanInterval = TimeSpan.FromMinutes(5);

        private static readonly Lazy<DeviceDiscovery> _instance = new(() => new DeviceDiscovery());

        internal static DeviceDiscovery Instance => _instance.Value;

        private readonly BehaviorSubject<IImmutableList<String>> _devices = new(ImmutableList<String>.Empty);

        internal IObservable<IImmutableList<String>> Devices => this._devices.AsObservable();

        internal Action<String> logInfo { get; set; } = _ => { };

        internal Action<Exception> logError { get; set; } = _ => { };

        private DeviceDiscovery()
        {
            // Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(
            //     _ =>
            //     {
            //         this.ProbeDevices();
            //         Observable.Interval(ScanInterval).Subscribe(_ => this.ProbeDevices());
            //     }
            // );
        }

        private static Boolean IsPortOpen(
            String ip, Int16 port,
            TimeSpan timeout,
            Action<String> logInfo,
            Action<Exception> logError)
        {
            logInfo($"Checking if port {port} is open on {ip}");

            try
            {
                using var client = new TcpClient();

                var result = client.BeginConnect(ip, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout);

                if (!success)
                {
                    return false;
                }

                client.EndConnect(result);

                return true;
            }
            catch (Exception ex)
            {
                logError(ex);

                return false;
            }
        }

        private static IImmutableList<String> GetLocalIPAddresses()
        {
            var activeInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic =>
                    nic.OperationalStatus is OperationalStatus.Up &&
                    nic.NetworkInterfaceType is NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211
                )
                .SelectMany(nic => nic.GetIPProperties().UnicastAddresses)
                .ToImmutableList();

            var ipv4Addresses = activeInterfaces
                .Where(ip =>
                    ip.Address.AddressFamily is AddressFamily.InterNetwork &&
                    ip.Address.ToString() is not "127.0.0.1"
                )
                .Select(ip => ip.Address.ToString())
                .ToImmutableList();

            return ipv4Addresses;
        }


        private static IEnumerable<String> GetSubnetAddresses(IPAddress gateway)
        {
            var gatewayBytes = gateway.GetAddressBytes();

            for (Byte i = 1; i < 255; i++)
            {
                gatewayBytes[3] = i;

                yield return new IPAddress(gatewayBytes).ToString();
            }
        }

        private void ProbeDevices()
        {
            var localIp = GetLocalIPAddresses().FirstOrDefault();

            if (localIp == null)
            {
                this.logInfo("No local IP address found");

                return;
            }

            var ipBytes = IPAddress.Parse(localIp).GetAddressBytes();

            ipBytes[3] = 1; // Set the last byte to 1 to assume the gateway address

            var gatewayIpAddress = new IPAddress(ipBytes);

            var devices = GetSubnetAddresses(gatewayIpAddress)
                .Where(ip => IsPortOpen(ip, Port, TimeSpan.FromMilliseconds(100), this.logInfo, this.logError))
                .ToImmutableList();

            this.logInfo($"Found {devices.Count} devices. With IP addresses: {String.Join(", ", devices)}");
 
            this._devices.OnNext(devices);
        }
    }
}