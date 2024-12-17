namespace Loupedeck.ElgatoLightControlPlugin.Helpers;

internal static class FolderImage
{
	internal static BitmapImage ToImage(string resourceName, PluginImageSize imageSize)
	{
		using var bitmapBuilder = new BitmapBuilder(imageSize);

		bitmapBuilder.DrawImage(
			EmbeddedResources.ReadImage(resourceName),
			(int)(bitmapBuilder.Width * 0.1),
			(int)(bitmapBuilder.Height * 0.1),
			(int)(bitmapBuilder.Width * 0.8),
			(int)(bitmapBuilder.Height * 0.8)
		);

		return bitmapBuilder.ToImage();
	}
}
