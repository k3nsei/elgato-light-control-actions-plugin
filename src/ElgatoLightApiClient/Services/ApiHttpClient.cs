namespace ElgatoLightApiClient.Services
{
    using System.Net;
    using System.Text;
    using System.Text.Json;

    using DTO;

    internal sealed class ApiHttpClient
    {
        private static readonly Lazy<ApiHttpClient> _instance = new(() => new ApiHttpClient());

        public static ApiHttpClient Instance => _instance.Value;

        private readonly HttpClient _httpClient;

        private ApiHttpClient() => this._httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };

        internal async Task<LightStateDto> GetStateAsync(String lightIpAddress, CancellationToken cancellationToken)
        {
            var baseUrl = LightIpAddressToBaseUrl(lightIpAddress);

            var response = await this._httpClient.GetAsync($"{baseUrl}/elgato/lights", cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<LightsResponseDto>(content);

            return data.Lights.Count > 0 ? data.Lights[0] : new LightStateDto();
        }

        internal async Task<IEnumerable<LightStateDto>> GetStatesAsync(IEnumerable<String> lightIpAddresses,
            CancellationToken cancellationToken)
        {
            var tasks = lightIpAddresses.Select(ip => this.GetStateAsync(ip, cancellationToken));

            return await Task.WhenAll(tasks);
        }

        internal async Task SetPowerStateAsync(String lightIpAddress, Boolean value,
            CancellationToken cancellationToken)
        {
            var baseUrl = LightIpAddressToBaseUrl(lightIpAddress);

            var data = new SetPowerStateRequestDto(new[] { new LightPowerStateDto(value == true ? (Byte)1 : (Byte)0) }
                .AsReadOnly());

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response =
                await this._httpClient.PutAsync($"{baseUrl}/elgato/lights", content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        internal async Task SetBrightnessAsync(String lightIpAddress, Byte value, CancellationToken cancellationToken)
        {
            var baseUrl = LightIpAddressToBaseUrl(lightIpAddress);

            var data = new SetBrightnessRequestDto(new[] { new LightBrightnessStateDto(value) }.AsReadOnly());

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response =
                await this._httpClient.PutAsync($"{baseUrl}/elgato/lights", content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        internal async Task SetColorTemperatureAsync(String lightIpAddress, UInt16 value,
            CancellationToken cancellationToken)
        {
            var baseUrl = LightIpAddressToBaseUrl(lightIpAddress);

            var data = new SetColorTemperatureRequestDto(
                new[] { new LightColorTemperatureStateDto(value) }.AsReadOnly());

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response =
                await this._httpClient.PutAsync($"{baseUrl}/elgato/lights", content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        private static String LightIpAddressToBaseUrl(String lightIpAddress)
        {
            if (!IPAddress.TryParse(lightIpAddress, out _))
            {
                throw new ArgumentException("Provided Elgato light address is not a valid IP address",
                    nameof(lightIpAddress));
            }

            return $"http://{lightIpAddress}:9123";
        }
    }
}