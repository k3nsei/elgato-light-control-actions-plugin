namespace Loupedeck.ElgatoLightControlPlugin
{
    using System;

    using ElgatoLightApiClient;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class ElgatoLightControlPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        // Initializes a new instance of the plugin class.
        public ElgatoLightControlPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);

            // Initialize the Elgato Light API client.
            ElgatoLightApiClient.Init();
        }
    }
}