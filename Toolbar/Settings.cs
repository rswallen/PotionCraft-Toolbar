using BepInEx.Configuration;
using BepInEx.Logging;
using QFSW.QC;
using System;

namespace Toolbar
{
    [CommandPrefix("Toolbar-")]
    internal static class Settings
    {
        private static ManualLogSource Log => ToolbarPlugin.Log;
        private static ConfigFile Config => ToolbarPlugin.Config;

        private static bool initialised = false;

        private static ConfigEntry<float> uiScale;
        private static ConfigEntry<int> maxButtonsVert;
        private static ConfigEntry<int> maxButtonsHorz;
        private static ConfigEntry<bool> showSubPanelIndicators;

        public static float UIScale
        {
            get => uiScale.Value;
            set
            {
                if (uiScale.Value != value)
                {
                    uiScale.Value = value;
                }
            }
        }

        public static int MaxButtonsVert
        {
            get => maxButtonsVert.Value;
            set
            {
                if (maxButtonsVert.Value != value)
                {
                    maxButtonsVert.Value = value;
                }
            }
        }

        public static int MaxButtonsHorz
        {
            get => maxButtonsHorz.Value;
            set
            {
                if (maxButtonsHorz.Value != value)
                {
                    maxButtonsHorz.Value = value;
                }
            }
        }

        public static bool ShowSubPanelIndicators
        {
            get => showSubPanelIndicators.Value;
            set
            {
                if (showSubPanelIndicators.Value != value)
                {
                    showSubPanelIndicators.Value = value;
                }
            }
        }

        public static void Initialize()
        {
            if (!initialised)
            {
                uiScale = Config.Bind("General", "UIScale", 0.75f, "UI Scale for the toolbar");
                uiScale.SettingChanged += ToolbarManager.OnChangedUIScale;

                var vertDesc = new ConfigDescription("Maximum number of buttons on vertical panels", new AcceptableValueRange<int>(3, int.MaxValue));
                maxButtonsVert = Config.Bind("General", "MaxButtonsVert", 10, vertDesc);
                maxButtonsVert.SettingChanged += ToolbarManager.OnChangedMaxButtonsVert;

                var horzDesc = new ConfigDescription("Maximum number of buttons on horizontal panels", new AcceptableValueRange<int>(3, int.MaxValue));
                maxButtonsHorz = Config.Bind("General", "MaxButtonsHorz", 15, horzDesc);
                maxButtonsHorz.SettingChanged += ToolbarManager.OnChangedMaxButtonsHorz;

                showSubPanelIndicators = Config.Bind("General", "ShowSubPanelIndicators", true, "Whether to show or hide sprites indicating that which buttons open subpanels");
                showSubPanelIndicators.SettingChanged += ToolbarManager.OnChangedShowIndicators;

                initialised = true;
                Config.Reload();
            }
        }

        [Command("SetUIScale", "Updates the scale of the toolbar UI", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void SetUIScale(float scale)
        {
            UIScale = scale;
        }

        [Command("SetMaxButtonsVert", "Updates the maximum number of butons a vertical panel can display at once", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void SetMaxButtonsVert(int maxButtons)
        {
            MaxButtonsVert = maxButtons;
        }

        [Command("SetMaxButtonsHorz", "Updates the maximum number of butons a horizontal panel can display at once", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void SetMaxButtonsHorz(int maxButtons)
        {
            MaxButtonsHorz = maxButtons;
        }

        [Command("ToggleSubPanelIndicators", "Toggles the visibility of SubPanel indicator sprites", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void ToggleSubPanelIndicators()
        {
            showSubPanelIndicators.Value = !showSubPanelIndicators.Value;
        }

        [Command("ReloadConfig", "Reload the plugin config file", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        private static void ReloadConfig()
        {
            Config.Reload();
        }
    }
}