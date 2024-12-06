namespace ElgatoLightApiClient.Services
{
    using System.Net;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Makaretu.Dns;

    public static class DeviceDiscovery
    {
        private static readonly String Service = "_elg._tcp";

        private static readonly Subject<Unit> ProbeSubject = new();

        private static readonly Dictionary<String, IPAddress> Devices = new();

        public static EventHandler<DeviceDiscoveryEventArgs> DeviceDiscovered = delegate { };

        public static void Discover() => ProbeSubject.OnNext(Unit.Default);

        static DeviceDiscovery()
        {
            ProbeSubject
                .Throttle(TimeSpan.FromSeconds(5))
                .SelectMany(_ => Observable.FromAsync(QueryDevices))
                .Subscribe((_) => Logger.Verbose(String.Join(", ", Devices.Values)));
        }

        private static async Task QueryDevices()
        {
            using var mdns = new MulticastService();
            using var sd = new ServiceDiscovery(mdns);

            mdns.NetworkInterfaceDiscovered += OnNetworkInterfaceDiscovered;
            sd.ServiceInstanceDiscovered += OnServiceInstanceDiscovered;

            using var cts = new CancellationTokenSource(2000);

            try
            {
                mdns.Start();
                await Task.Delay(2000, cts.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled after 2 seconds
            }
            finally
            {
                mdns.Stop();
            }

            return;

            void OnNetworkInterfaceDiscovered(Object sender, NetworkInterfaceEventArgs e)
            {
                sd.QueryUnicastServiceInstances(Service);
            }

            void OnServiceInstanceDiscovered(Object sender, ServiceInstanceDiscoveryEventArgs e)
            {
                var name = e.ServiceInstanceName.ToString();
                var ipAddresses = e.RemoteEndPoint.Address;

                Devices.TryAdd(name, ipAddresses);

                DeviceDiscovered?.Invoke(null, new DeviceDiscoveryEventArgs(name, ipAddresses));
            }
        }
    }

    public class DeviceDiscoveryEventArgs(String name, IPAddress ipAddress) : EventArgs
    {
        public readonly String Name = name;
        public readonly IPAddress IpAddress = ipAddress;
    }
}