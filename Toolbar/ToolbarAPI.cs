using PotionCraft.ObjectBased.UIElements.Tooltip;
using System;
using System.Linq;
using Toolbar.Extensions;
using Toolbar.UIElements;
using Toolbar.UIElements.Buttons;
using Toolbar.UIElements.Panels;
using UnityEngine;
using UnityEngine.Events;

namespace Toolbar
{
    public static class ToolbarAPI
    {
        /// <summary>
        /// Boolean indicating whether the plugin has created the root toolbar objects and is ready for addtional objects to be added to them.
        /// </summary>
        public static bool IsInitialised { get => ToolbarManager.IsInitialised; }

        /// <summary>
        /// Adds a given function to the callback that triggers when the root toolbar objects have been created.
        /// </summary>
        /// <param name="listener">Function to add. Should be a static function that returns void and takes 0 arguments.</param>
        public static void RegisterInit(UnityAction listener)
        {
            if (listener == null)
            {
                return;
            }

            // if plugin is already initialised, call the action directly
            if (IsInitialised)
            {
                listener.Invoke();
            }
            else
            {
                ToolbarManager.onPluginInitialised.AddListener(listener);
            }
        }

        /// <summary>
        /// Creates a custom toolbar button of type T.
        /// </summary>
        /// <typeparam name="T">Type of button to create. Must derive from BaseToolbarButton.</typeparam>
        /// <returns>Reference to freshly create toolbar button of type T.</returns>
        public static T CreateCustomButton<T>() where T : BaseToolbarButton
        {
            if (!IsInitialised)
            {
                return null;
            }

            // can't create PanelToolbarButtons with this
            if (typeof(T).IsSubclassOf(typeof(PanelToolbarButton)))
            {
                return null;
            }

            return BaseToolbarButton.Create<T>();
        }

        /// <summary>
        /// Creates a custom subpanel toolbar button of type T.
        /// </summary>
        /// <typeparam name="T">Type of button to create. Must derive from SubPanelToolbarButton.</typeparam>
        /// <param name="panelUID">Unique string to identify the panel by. Cannot be null or empty</param>
        /// <returns>Reference to freshly create toolbar button of type T.</returns>
        public static T CreateCustomSubPanelButton<T>(string panelUID) where T : SubPanelToolbarButton
        {
            if (!IsInitialised)
            {
                return null;
            }

            if (DoesToolbarPanelExist(panelUID))
            {
                return null;
            }

            var button = SubPanelToolbarButton.Create<T>(panelUID);
            ToolbarManager.SubPanels.Add(button.SubPanel);
            return button;
        }

        /// <summary>
        /// Creates a DelegateToolbarButton with custom Sprite, click callback and tooltip callback.
        /// </summary>
        /// <param name="icon">Sprite to be used to display the button. Can be null.</param>
        /// <param name="onRelease">Function to call when button is clicked. Should be a static function that returns void and takes 0 arguments. Can be null.</param>
        /// <param name="getTooltip">Function to call when button is clicked. Should be a static function that returns TooltipContent and takes 0 arguments. Can be null.</param>
        /// <returns></returns>
        public static DelegateToolbarButton CreateDelegateButton(Sprite icon, Action onRelease, Func<TooltipContent> getTooltip)
        {
            if (!IsInitialised)
            {
                return null;
            }
            return DelegateToolbarButton.Create(icon, onRelease, getTooltip);
        }

        /// <summary>
        /// Creates a ConCmdToolbarButton with custom Sprite and command.
        /// </summary>
        /// <param name="icon">Sprite to be used to display the button. Can be null.</param>
        /// <param name="command">Command to invoke when button is clicked. Include all arguments in the string.</param>
        /// <returns></returns>
        public static ConCmdToolbarButton CreateConCmdButton(Sprite icon, string command)
        {
            if (!IsInitialised)
            {
                return null;
            }
            return ConCmdToolbarButton.Create(icon, command);
        }

        /// <summary>
        /// Creates a SubPanelToolbarButton with custom Sprite.
        /// </summary>
        /// <param name="icon">Sprite to be used to display the button. Can be null.</param>
        /// <param name="panelUID">Unique string to identify the panel by. Cannot be null or empty</param>
        /// <param name="getTooltip">Function to call when button is clicked. Should be a static function that returns TooltipContent and takes 0 arguments. Can be null.</param>
        /// <returns></returns>
        public static SubPanelToolbarButton CreateSubPanelButton(Sprite icon, string panelUID, Func<TooltipContent> getTooltip)
        {
            if (!IsInitialised)
            {
                return null;
            }

            var button = CreateCustomSubPanelButton<SubPanelToolbarButton>(panelUID);
            if (button != null)
            {
                button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
                button.spriteRenderer.SetupToolbarSprite(icon, true);

                button.getTooltip = getTooltip;
            }
            return button;
        }

        /// <summary>
        /// Indicates whether a toolbar panel with the given panelUID exists.
        /// </summary>
        /// <param name="panelUID">UID used to identify the panel.</param>
        /// <returns>True if panel exists, false otherwise.</returns>
        public static bool DoesToolbarPanelExist(string panelUID)
        {
            return ToolbarManager.SubPanels.Any(x => x.UID.Equals(panelUID, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns BaseToolbarPanel of the given panelUID, or null if the panel does not exist.
        /// </summary>
        /// <param name="panelUID">UID used to identify the panel.</param>
        /// <returns>Reference to a BaseToolbarPanel or null.</returns>
        public static BaseToolbarPanel GetToolbarPanel(string panelUID)
        {
            return ToolbarManager.SubPanels.FirstOrDefault(x => x.UID.Equals(panelUID, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Attempts to add button to the root panel.
        /// </summary>
        /// <param name="button">Button to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool AddButtonToRootPanel(BaseToolbarButton button)
        {
            if (!IsInitialised)
            {
                return false;
            }
            return ToolbarManager.RootPanel.AddButton(button);
        }

        /// <summary>
        /// Attempts to remove button from the root panel.
        /// </summary>
        /// <param name="button">Button to remove.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool RemoveButtonFromRootPanel(BaseToolbarButton button)
        {
            if (!IsInitialised)
            {
                return false;
            }
            return ToolbarManager.RootPanel.RemoveButton(button);
        }

        /// <summary>
        /// Attempts to add button to the panel with the given UID.
        /// </summary>
        /// <param name="panelUID">UID used to identify the panel.</param>
        /// <param name="button">Button to add.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool AddButtonToSubPanel(string panelUID, BaseToolbarButton button)
        {
            if (!IsInitialised)
            {
                return false;
            }
            return GetToolbarPanel(panelUID)?.AddButton(button) ?? false;
        }

        /// <summary>
        /// Attempts to remove button from the panel with the given UID.
        /// </summary>
        /// <param name="panelUID">UID used to identify the panel.</param>
        /// <param name="button">Button to remove.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool RemoveButtonFromSubPanel(string panelUID, BaseToolbarButton button)
        {
            if (!IsInitialised)
            {
                return false;
            }
            return GetToolbarPanel(panelUID)?.RemoveButton(button) ?? false;
        }
    }
}