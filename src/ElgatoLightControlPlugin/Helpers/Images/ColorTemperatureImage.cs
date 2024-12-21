namespace Loupedeck.ElgatoLightControlPlugin.Helpers.Images;

using ElgatoLightControl.ApiClient.Helpers;

internal static class ColorTemperatureImage
{
	internal static BitmapImage ToImage(ushort mired, PluginImageSize imageSize)
	{
		var kelvin = TemperatureConverter.MiredToKelvin(mired);

		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var size = Math.Min(bitmapBuilder.Width, bitmapBuilder.Height);

		bitmapBuilder.FillRectangle(
			0,
			0,
			bitmapBuilder.Width,
			bitmapBuilder.Height,
			BitmapColor.Black
		);

		bitmapBuilder.FillCircle(
			size * .5f,
			size * .5f,
			size * .5f,
			KelvinToColor(kelvin)
		);

		bitmapBuilder.DrawText(
			$"{kelvin}K",
			BitmapColor.Black
		);

		return bitmapBuilder.ToImage();
	}

	private static BitmapColor KelvinToColor(ushort kelvin)
	{
		kelvin = (ushort)(Math.Clamp(kelvin, (ushort)1000, (ushort)40000) / 100);

		var red = kelvin <= 66 ? 255 : Math.Clamp(329.698727446 * Math.Pow(kelvin - 60, -0.1332047592), 0, 255);

		var green = kelvin <= 66
			? Math.Clamp(99.4708025861 * Math.Log(kelvin) - 161.1195681661, 0, 255)
			: Math.Clamp(288.1221695283 * Math.Pow(kelvin - 60, -0.0755148492), 0, 255);

		var blue = kelvin >= 66
			? 255
			: kelvin <= 19
				? 0
				: Math.Clamp(138.5177312231 * Math.Log(kelvin - 10) - 305.0447927307, 0, 255);

		return new BitmapColor((byte)red, (byte)green, (byte)blue);
	}
}
