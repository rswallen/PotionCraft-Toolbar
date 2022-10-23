using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem.Game;
using QFSW.QC;

namespace Toolbar
{
    internal class Localization
    {
        static ManualLogSource Log => ToolbarPlugin.Log;

        [HarmonyPostfix, HarmonyPatch(typeof(LocalizationManager), "ParseLocalizationData")]
        public static void ParseLocalizationData_Postfix()
        {
            LocalizationManager.textData[LocalizationManager.Locale.en].AddText("toolbar_opentoolbar_tooltip", "Mod Toolbar");
        }
    }
}