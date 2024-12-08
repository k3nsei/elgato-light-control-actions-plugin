namespace Loupedeck.ElgatoLightControlPlugin
{
    using ElgatoLightControl.ApiClient;

    using Helpers;

    public class ElgatoLightControlPlugin : Plugin
    {
        public static readonly String PluginName = "ElgatoLightControl";

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        public ElgatoLightControlPlugin()
        {
            // Initialize the plugin logger
            PluginLogger.Init(this.Log);

            // Initialize the plugin key-value store
            PluginKeyValueStore.Init(
                this.TryGetPluginSetting,
                this.SetPluginSetting,
                this.DeletePluginSetting
            );

            // Initialize the plugin device manager
            PluginDeviceManager.Init();

            // Initialize the Elgato Light API client
            ApiClient.Init(PluginLogger.Instance);

            // Initialize the plugin resources
            PluginResources.Init(this.Assembly);
        }

        public override void Load() => PluginDeviceManager.OnLoad();

        public override void Unload() => PluginDeviceManager.OnUnload();
    }
}