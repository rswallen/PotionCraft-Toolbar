using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.Assemblies.DataBaseSystem.PreparedObjects;
using PotionCraft.LocalizationSystem;

namespace Toolbar
{
    internal class Localization
    {
        static ManualLogSource Log => ToolbarPlugin.Log;

        [HarmonyPostfix, HarmonyPatch(typeof(LocalizationManager), "LoadLocalizationData")]
        public static void LoadLocalizationData_Postfix(ref LocalizationData __result)
        {
            __result.Add((int)LocalizationManager.Locale.en, "toolbar_opentoolbar_tooltip", "Mod Toolbar");
        }
    }
}