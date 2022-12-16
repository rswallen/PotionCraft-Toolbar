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

        private static int buttonNumLast = 0;

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
            for (int i = buttonNumLast; i < number; i++)
            {
                var buttonUID = "simpletoolbarplugin.button." + i;
                var iconName = ToolbarUtils.GetRandomIcon().name;
                var button = ToolbarWrapper.CreateDelegateButtonWithIcon(buttonUID, iconName, null, delegate ()
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

        [Command("AddSubPanelToRoot", "Command that creates subpanel with the given UID", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        internal static void AddSubPanelToRoot(string uid)
        {
            string panelUID = "simpletoolbarplugin.panel." + uid;
            if (ToolbarAPI.DoesToolbarPanelExist(panelUID))
            {
                Log.LogInfo("ToolbarPanel with that UID already exists");
                return;
            }

            var buttonUID = "simpletoolbarplugin.button." + uid;
            var button = ToolbarWrapper.CreateSubPanelButtonWithIcon(buttonUID, "Hypnotoad", panelUID, delegate ()
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

            buttonUID = "simpletoolbarplugin.button." + uid + ".delegate";
            var iconName = ToolbarUtils.GetRandomIcon().name;
            var button2 = ToolbarWrapper.CreateDelegateButtonWithIcon(buttonUID, iconName, null, delegate ()
            {
                return new()
                {
                    header = "STP SubPanel DelegateButton",
                };
            }, true);
            button2.IsActive = true;
            button.SubPanel.AddButton(button2);
        }

        [Command("AddButtonsToSubPanel", "Command that adds a given number of dummy delegate buttons to a subpanel of a given UID", true, true, Platform.AllPlatforms, MonoTargetType.All)]
        internal static void AddButtonsToSubPanel(string uid, int numButtons)
        {
            string panelUID = "simpletoolbarplugin.panel." + uid;
            var panel = ToolbarAPI.GetToolbarPanel(panelUID);
            if (panel == null)
            {
                Log.LogInfo("SubPanel does not exist");
                return;
            }

            for (int i = buttonNumLast; i < numButtons; i++)
            {
                var buttonUID = "simpletoolbarplugin.button." + i;
                var iconName = ToolbarUtils.GetRandomIcon().name;
                var button = ToolbarWrapper.CreateDelegateButtonWithIcon(buttonUID, iconName, null, delegate ()
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