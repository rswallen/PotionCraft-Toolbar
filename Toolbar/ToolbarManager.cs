using HarmonyLib;
using PotionCraft.ManagersSystem;
using PotionCraft.ObjectBased.RecipeMap.RecipeMapObject;
using System;
using System.Collections.Generic;
using Toolbar.UIElements.Buttons;
using Toolbar.UIElements.Panels;
using UnityEngine;
using UnityEngine.Events;

namespace Toolbar
{
    internal static class ToolbarManager
    {
        internal static readonly UnityEvent onPluginInitialised = new();

        internal static GameObject RootAnchor;
        internal static PanelToolbarButton RootButton;
        internal static BaseToolbarPanel RootPanel;
        internal static readonly List<BaseToolbarPanel> SubPanels = new();

        internal static bool IsInitialised { get; private set; } = false;

        internal static void CreateRootToolbarItems()
        {
            if (IsInitialised)
            {
                return;
            }

            RootAnchor = new()
            {
                name = "RootAnchor",
                layer = LayerMask.NameToLayer("UI"),
            };
            RootAnchor.transform.SetParent(Managers.Game.Cam.transform);
            RootAnchor.transform.localPosition = new Vector2(-12.55f, -6.9f);
            RootAnchor.transform.localScale = Settings.UIScale * Vector3.one;

            RootButton = RootToolbarButton.Create();
            RootButton.Anchor.transform.SetParent(RootAnchor.transform);
            RootButton.Anchor.transform.localScale = Vector3.one;
            RootButton.Anchor.transform.localPosition = new Vector2(0.6f, 0.5f);

            RootButton.SubPanel = VerticalToolbarPanel.Create(RootButton.Anchor, RootButton);
            RootButton.SubPanel.UID = "root";
            RootPanel = RootButton.SubPanel;

            IsInitialised = true;
        }

        internal static void OnChangedUIScale(object sender, EventArgs e)
        {
            RootAnchor.transform.localScale = Settings.UIScale * Vector3.one;
            RefillAll();
        }

        internal static void OnChangedMaxButtonsVert(object sender, EventArgs e)
        {
            RootPanel.UpdateMinMaxLength(Settings.MaxButtonsVert);
            RootPanel.Refill();
        }

        internal static void OnChangedMaxButtonsHorz(object sender, EventArgs e)
        {
            foreach (var panel in SubPanels)
            {
                panel.UpdateMinMaxLength(Settings.MaxButtonsHorz);
                panel.Refill();
            }
        }

        internal static void OnChangedShowIndicators(object sender, EventArgs e)
        {
            foreach (var button in RootPanel.buttons)
            {
                if (button is SubPanelToolbarButton subButton)
                {
                    subButton.UpdateVisibility();
                }
            }
        }

        internal static void RefillAll()
        {
            RootPanel.Refill();
            foreach (var panel in SubPanels)
            {
                panel.Refill();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RecipeMapObject), "Awake")]
        internal static void Awake_Postfix()
        {
            CreateRootToolbarItems();
            onPluginInitialised.Invoke();
            onPluginInitialised.RemoveAllListeners();
        }
    }
}
