using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace Toolbar
{
    [BepInPlugin("com.github.rswallen.potioncraft.toolbar", "Toolbar", "2.1.0")]
    internal class ToolbarPlugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static new ConfigFile Config;

        private void Awake()
        {
            Log = Logger;
            Config = base.Config;
            
            Harmony.CreateAndPatchAll(typeof(Localization));
            Harmony.CreateAndPatchAll(typeof(ToolbarManager));

            Settings.Initialize();

            Log.LogInfo($"Toolbar is loaded");
        }
    }
}
