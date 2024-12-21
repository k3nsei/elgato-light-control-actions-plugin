namespace ElgatoLightControl.ApiClient.Helpers.Serialization;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class StringOrNumberConverter : JsonConverter<string>
{
	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.Number:
				try
				{
					return reader.GetInt32().ToString(CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					return reader.GetDouble().ToString(CultureInfo.InvariantCulture);
				}
			case JsonTokenType.String:
				return reader.GetString() ?? string.Empty;
			default:
				throw new JsonException();
		}
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		if (int.TryParse(value, out var intValue))
		{
			writer.WriteNumberValue(intValue);
		}
		else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue))
		{
			writer.WriteNumberValue(doubleValue);
		}
		else
		{
			writer.WriteStringValue(value);
		}
	}
}
