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
        internal static int buttonNumLast = 0;

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
            var button = RotatingIconToolbarButton.Create("advtoolbarplugin.button.rotating");
            button.IsActive = true;
            ToolbarAPI.AddButtonToRootPanel(button);
        }

        [Command("ATP-MakeMagicButtonPanel", "Command that adds a given number of MagicToolbarButtons to a subpanel of a given panelURI", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        private static void MakeMagicButtonPanel(string uid, int numButtons)
        {
            var panelUID = "advtoolbarplugin.panel." + uid;
            if (ToolbarAPI.DoesToolbarPanelExist(panelUID))
            {
                Log.LogInfo("ToolbarPanel with that URI already exists");
                return;
            }

            var iconName = ToolbarUtils.GetRandomIcon().name;
            var sprite = ToolbarUtils.GetSpriteFromColoredIcon(iconName, true);
            var buttonUID = "advtoolbarplugin.button." + uid;
            
            var button = ToolbarAPI.CreateSubPanelButton(buttonUID, sprite, panelUID, delegate ()
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

            for (int i = buttonNumLast; i < numButtons; i++)
            {
                buttonUID = $"advtoolbarplugin.button.{uid}.{buttonNumLast}";
                var button2 = MagicToolbarButton.Create(buttonUID, i == 0);
                button2.IsActive = true;
                button.SubPanel.AddButton(button2, false);
            }
            button.SubPanel.Refill();
        }
    }
}
