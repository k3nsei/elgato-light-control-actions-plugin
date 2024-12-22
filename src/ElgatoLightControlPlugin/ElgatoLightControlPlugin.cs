namespace Loupedeck.ElgatoLightControlPlugin;

using ElgatoLightControl.ApiClient;

using Helpers;

public class ElgatoLightControlPlugin : Plugin
{
	public static readonly string PluginName = "ElgatoLightControl";

	private readonly CancellationTokenSource _cancellationTokenSource = new();

	public CancellationToken CancellationToken => this._cancellationTokenSource.Token;

	public ElgatoLightControlPlugin()
	{
		PluginLogger.Init(this.Log);

		PluginKeyValueStore.Init(
			this.TryGetPluginSetting,
			this.SetPluginSetting,
			this.DeletePluginSetting
		);

		PluginDeviceManager.Init();

		ApiClient.Init(PluginLogger.Instance);

		PluginResources.Init(this.Assembly);
	}

	public override bool UsesApplicationApiOnly => true;

	public override bool HasNoApplication => true;

	public override void Load() => PluginDeviceManager.OnLoad();

	public override void Unload()
	{
		PluginDeviceManager.OnUnload();

		this._cancellationTokenSource.Cancel();
	}
}
