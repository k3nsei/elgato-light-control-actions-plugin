namespace Loupedeck.ElgatoLightControlPlugin.Helpers.Images;

using Constants;

internal static class PowerStateImage
{
	internal const string AllLightsOn = "AllLightsOn";

	internal const string AllLightsOff = "AllLightsOff";

	internal const string LightOn = "LightOn";

	internal const string LightOff = "LightOff";

	internal static BitmapImage ToImage(string name, string type, PluginImageSize imageSize)
	{
		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var image = type switch
		{
			AllLightsOn => EmbeddedResources.ReadImage(ImageId.LightbulbGroupOn),
			AllLightsOff => EmbeddedResources.ReadImage(ImageId.LightbulbGroupOff),
			LightOn => EmbeddedResources.ReadImage(ImageId.LightbulbOn),
			LightOff => EmbeddedResources.ReadImage(ImageId.LightbulbOff),
			_ => null
		};

		if (image is not null)
		{
			bitmapBuilder.DrawImage(
				image,
				(int)(bitmapBuilder.Width * .15),
				0,
				(int)(bitmapBuilder.Width * .7),
				(int)(bitmapBuilder.Height * .7)
			);
		}

		if (!string.IsNullOrWhiteSpace(name))
		{
			bitmapBuilder.DrawText(
				name,
				0,
				(int)(bitmapBuilder.Height * .25),
				bitmapBuilder.Width,
				bitmapBuilder.Height
			);
		}

		return bitmapBuilder.ToImage();
	}
}
