using PotionCraft.ObjectBased.UIElements.Tooltip;
using PotionCraft.ScriptableObjects;
using PotionCraft.Utils;
using System;
using Toolbar.UIElements.Buttons;
using UnityEngine;
using UnityEngine.Events;

namespace Toolbar
{
    internal static class ToolbarWrapper
    {
        public static void RegisterInit(UnityAction listener)
        {
            if (listener == null)
            {
                return;
            }
            ToolbarAPI.RegisterInit(listener);
        }

        public static DelegateToolbarButton CreateDelegateButtonWithIcon(string buttonUID, string iconName, Action onRelease, Func<TooltipContent> getTooltip, bool randomizeColors = false)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                return null;
            }

            var sprIcon = ToolbarUtils.GetSpriteFromColoredIcon(iconName, randomizeColors);
            var button = ToolbarAPI.CreateDelegateButton(buttonUID, sprIcon, onRelease, getTooltip);
            return button;
        }

        public static ConCmdToolbarButton CreateConCmdButtonWithIcon(string buttonUID, string iconName, string command, bool randomizeColors = false)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                return null;
            }

            var sprIcon = ToolbarUtils.GetSpriteFromColoredIcon(iconName, randomizeColors);
            var button = ToolbarAPI.CreateConCmdButton(buttonUID, sprIcon, command);
            return button;
        }

        public static SubPanelToolbarButton CreateSubPanelButtonWithIcon(string buttonUID, string iconName, string panelURI, Func<TooltipContent> getTooltip, bool randomizeColors = false)
        {
            if (string.IsNullOrEmpty(iconName) || string.IsNullOrEmpty(panelURI))
            {
                return null;
            }

            var sprIcon = ToolbarUtils.GetSpriteFromColoredIcon(iconName, randomizeColors);
            var button = ToolbarAPI.CreateSubPanelButton(buttonUID, sprIcon, panelURI, getTooltip);
            return button;
        }

        public static void AddButtonToRootPanel(BaseToolbarButton button)
        {
            if ((button == null) || !ToolbarAPI.IsInitialised)
            {
                return;
            }
            ToolbarAPI.AddButtonToRootPanel(button);
        }
    }
}
