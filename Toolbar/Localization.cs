using BepInEx.Logging;
using HarmonyLib;
using PotionCraft.Assemblies.DataBaseSystem.PreparedObjects;
using PotionCraft.LocalizationSystem;
using System;

namespace Toolbar
{
    internal class Localization
    {
        [HarmonyPostfix, HarmonyPatch(typeof(LocalizationManager), "LoadLocalizationData")]
        public static void LoadLocalizationData_Postfix(ref LocalizationData __result)
        {
            // key.GetText throws an error if there isn't text for every language
            // temp solution unless/until proper localisation can be created
            foreach (int lang in Enum.GetValues(typeof(LocalizationManager.Locale)))
            {
                __result.Add(lang, "toolbar_opentoolbar_tooltip", "Mod Toolbar");
            }
        }
    }
}