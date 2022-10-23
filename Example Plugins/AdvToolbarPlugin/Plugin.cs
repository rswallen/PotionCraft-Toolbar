using BepInEx;
using BepInEx.Logging;
using QFSW.QC;
using Toolbar;
using UnityEngine;
using UnityEngine.UIElements;

namespace AdvToolbarPlugin
{
    [BepInPlugin("com.github.rswallen.potioncraft.advtoolbarplugin", "Advanced Toolbar Plugin", "1.0.0")]
    [BepInDependency("com.github.rswallen.potioncraft.toolbar", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"Advanced Toolbar Plugin is loaded");
        }

        private void Start()
        {
            ToolbarAPI.RegisterInit(ButtonSetup);
        }

        private void ButtonSetup()
        {
            var button = RotatingIconToolbarButton.Create();
            button.IsActive = true;
            ToolbarAPI.AddButtonToRootPanel(button);
        }

        [Command("ATP-MakeMagicButtonPanel", "Command that adds a given number of MagicToolbarButtons to a subpanel of a given panelURI", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        private static void MakeMagicButtonPanel(string panelURI, int numButtons)
        {
            if (ToolbarAPI.DoesToolbarPanelExist(panelURI))
            {
                Log.LogInfo("ToolbarPanel with that URI already exists");
                return;
            }

            var iconName = ToolbarUtils.GetRandomIcon().name;
            var sprite = ToolbarUtils.GetSpriteFromColoredIcon(iconName, true);
            var button = ToolbarAPI.CreateSubPanelButton(sprite, panelURI, delegate ()
            {
                return new()
                {
                    header = "IsActive Test",
                };
            });
            if (button == null)
            {
                Log.LogInfo("Failed to create SubPanelButton");
                return;
            }
            button.IsActive = true;
            ToolbarAPI.AddButtonToRootPanel(button);

            for (int i = 0; i < numButtons; i++)
            {
                var button2 = MagicToolbarButton.Create(i == 0);
                button2.IsActive = true;
                button.SubPanel.AddButton(button2, false);
            }
            button.SubPanel.Refill();
        }
    }
}
