namespace ElgatoLightControl.ApiClient.Services;

using System.Net;
using System.Text;
using System.Text.Json;

using DTO;

internal static class ApiHttpClient
{
	private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(2) };

	internal static async Task<LightStateDto> GetStateAsync(string lightIpAddress,
		CancellationToken cancellationToken)
	{
		var url = ComposeUrl(lightIpAddress, "/elgato/lights");

		var response = await HttpClient.GetAsync(url, cancellationToken);

		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<LightsResponseDto>(content);

		return data.Lights.Count > 0 ? data.Lights[0] : new LightStateDto();
	}

	internal static async Task<IEnumerable<LightStateDto>> GetStatesAsync(IEnumerable<string> lightIpAddresses,
		CancellationToken cancellationToken)
	{
		var tasks = lightIpAddresses.Select(ip => GetStateAsync(ip, cancellationToken));

		return await Task.WhenAll(tasks);
	}

	internal static async Task SetPowerStateAsync(string lightIpAddress, bool value,
		CancellationToken cancellationToken)
	{
		var url = ComposeUrl(lightIpAddress, "/elgato/lights");

		var data = new SetPowerStateRequestDto(new[] { new LightPowerStateDto(value ? (byte)1 : (byte)0) }
			.AsReadOnly());

		var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

		var response =
			await HttpClient.PutAsync(url, content, cancellationToken);

		response.EnsureSuccessStatusCode();
	}

	internal static async Task SetBrightnessAsync(string lightIpAddress, byte value,
		CancellationToken cancellationToken)
	{
		var url = ComposeUrl(lightIpAddress, "/elgato/lights");

		var data = new SetBrightnessRequestDto(new[] { new LightBrightnessStateDto(value) }.AsReadOnly());

		var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

		var response =
			await HttpClient.PutAsync(url, content, cancellationToken);

		response.EnsureSuccessStatusCode();
	}

	internal static async Task SetColorTemperatureAsync(string lightIpAddress, ushort value,
		CancellationToken cancellationToken)
	{
		var url = ComposeUrl(lightIpAddress, "/elgato/lights");

		var data = new SetColorTemperatureRequestDto(
			new[] { new LightColorTemperatureStateDto(value) }.AsReadOnly());

		var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

		var response =
			await HttpClient.PutAsync(url, content, cancellationToken);

		response.EnsureSuccessStatusCode();
	}

	private static Uri ComposeUrl(string origin, string path, short port = 9123)
	{
		if (!IPAddress.TryParse(origin, out var ipAddress))
		{
			var ex = new ArgumentException("Provided Elgato light address is not a valid IP address",
				nameof(origin));

			Logger.Error(ex.Message);

			throw ex;
		}

		return new UriBuilder { Scheme = "http", Host = ipAddress.ToString(), Port = port, Path = path }.Uri;
	}
}
