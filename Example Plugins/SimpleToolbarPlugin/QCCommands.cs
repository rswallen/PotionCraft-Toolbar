using BepInEx.Logging;
using QFSW.QC;
using Toolbar;
using UnityEngine;

namespace SimpleToolbarPlugin
{
    [CommandPrefix("STP-")]
    internal static class QCCommands
    {
        private static ManualLogSource Log => Plugin.Log;

        [Command("ButtonCommandTest", "Command that ConCmdToolbarButton will call", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        public static void ButtonCommandTest(int number)
        {
            string message = $"Command: the number is {number}";
            Log.LogMessage(message);
            Plugin.PrintRecipeMapMessage(message, new Vector2(0.5f, 2.5f));
        }

        [Command("MakeDelegateButtonBatch", "Command that creates a given number of delegate buttons (that do nothing) to the root panel", true, true, Platform.AllPlatforms, MonoTargetType.Single)]
        public static void MakeDelegateButtonBatch(int number)
        {
            for (int i = 0; i < number; i++)
            {
                var iconName = ToolbarUtils.GetRandomIcon().name;
                var button = ToolbarWrapper.CreateDelegateButtonWithIcon(iconName, null, delegate ()
                {
                    return new()
                    {
                        header = "STP Batch DelegateButton",
                    };
                }, true);
                button.IsActive = true;
                ToolbarWrapper.AddButtonToRootPanel(button);
            }
        }

        [Command("AddSubPanelToRoot", "Command that creates subpanel with the given panelURI", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        internal static void AddSubPanelToRoot(string panelURI)
        {
            if (ToolbarAPI.DoesToolbarPanelExist(panelURI))
            {
                Log.LogInfo("ToolbarPanel with that URI already exists");
                return;
            }

            var button = ToolbarWrapper.CreateSubPanelButtonWithIcon("Hypnotoad", panelURI, delegate ()
            {
                return new()
                {
                    header = "STP SubPanel",
                };
            });
            if (button == null)
            {
                Log.LogInfo("Failed to create SubPanelButton");
                return;
            }
            button.IsActive = true;
            ToolbarAPI.AddButtonToRootPanel(button);

            var iconName = ToolbarUtils.GetRandomIcon().name;
            var button2 = ToolbarWrapper.CreateDelegateButtonWithIcon(iconName, null, delegate ()
            {
                return new()
                {
                    header = "STP SubPanel DelegateButton",
                };
            }, true);
            button2.IsActive = true;
            button.SubPanel.AddButton(button2);
        }

        [Command("AddButtonsToSubPanel", "Command that adds a given number of dummy delegate buttons to a subpanel of a given panelURI", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        internal static void AddButtonsToSubPanel(string panelURI, int numButtons)
        {
            var panel = ToolbarAPI.GetToolbarPanel(panelURI);
            if (panel == null)
            {
                Log.LogInfo("SubPanel does not exist");
                return;
            }

            for (int i = 0; i < numButtons; i++)
            {
                var iconName = ToolbarUtils.GetRandomIcon().name;
                var button = ToolbarWrapper.CreateDelegateButtonWithIcon(iconName, null, delegate ()
                {
                    return new()
                    {
                        header = "STP Batch DelegateButton on SubPanel",
                    };
                }, true);
                button.IsActive = true;
                panel.AddButton(button, false);
            }
            panel.Refill();
        }
    }
}