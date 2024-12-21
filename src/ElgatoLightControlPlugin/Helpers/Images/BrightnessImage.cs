namespace Loupedeck.ElgatoLightControlPlugin.Helpers.Images;

internal static class BrightnessImage
{
	internal static BitmapImage ToImage(byte brightness, PluginImageSize imageSize)
	{
		using var bitmapBuilder = new BitmapBuilder(imageSize);

		var size = Math.Min(bitmapBuilder.Width, bitmapBuilder.Height);
		var alpha = (byte)Math.Round(150 + brightness / 100f * (255 - 150));

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
			new BitmapColor(255, 255, 255, alpha)
		);

		bitmapBuilder.DrawText(
			$"{brightness}%",
			BitmapColor.Black
		);

		return bitmapBuilder.ToImage();
	}
}
