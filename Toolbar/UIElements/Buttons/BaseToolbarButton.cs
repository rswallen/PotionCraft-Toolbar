using BepInEx.Logging;
using PotionCraft.Core.Extensions;
using PotionCraft.ObjectBased.UIElements;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using System.Collections.Generic;
using Toolbar.UIElements.Panels;
using Toolbar.UIElements.ScrollObjects;
using UnityEngine;

namespace Toolbar.UIElements.Buttons
{
    public abstract class BaseToolbarButton : SpriteChangingButton
    {
        /// <summary>
        /// Reference to plugin logger for debugging purposes. For internal use only.
        /// </summary>
        internal static ManualLogSource Log => ToolbarPlugin.Log;

        /// <summary>
        /// Constructor for creating a toolbar button of type T.
        /// </summary>
        /// <typeparam name="T">Type of button to create.</typeparam>
        /// <returns>Reference to a freshly created toolbar button of type T.</returns>
        internal static T Create<T>() where T : BaseToolbarButton
        {
            // Make button anchor GameObject
            GameObject btnAnchor = new()
            {
                name = $"{typeof(T).Name}Anchor",
                layer = LayerMask.NameToLayer("UI"),
            };
            btnAnchor.SetActive(true);

            // Make button GameObject
            {
                GameObject obj = new()
                {
                    name = typeof(T).Name,
                    layer = LayerMask.NameToLayer("UI"),
                };
                obj.SetActive(false);
                obj.transform.SetParent(btnAnchor.transform, false);

                var button = obj.AddComponent<T>();
                button.Anchor = btnAnchor;

                button.thisCollider = obj.AddComponent<BoxCollider2D>();
                (button.thisCollider as BoxCollider2D).size = new(0.3f, 0.3f);

                // needed in order for tooltips to display
                button.tooltipContentProvider = obj.AddComponent<TooltipContentProvider>();
                button.tooltipContentProvider.fadingType = TooltipContentProvider.FadingType.UIElement;
                button.tooltipContentProvider.tooltipCollider = button.thisCollider;
                button.tooltipContentProvider.positioningSettings = new List<PositioningSettings>()
                {
                    new PositioningSettings()
                    {
                        bindingPoint = PositioningSettings.BindingPoint.TransformPosition,
                        freezeX = false,
                        freezeY = true,
                        position = new Vector2(0.45f, 0.4f),
                        tooltipCorner = PositioningSettings.TooltipCorner.LeftTop,
                    }
                };

                button.IgnoreRotationForPivot = true;
                button.showOnlyFingerWhenInteracting = true;
                button.raycastPriorityLevel = -21000;

                return button;
            }
        }

        /// <summary>
        /// Panel that this button is bound to.
        /// </summary>
        public BaseToolbarPanel ParentPanel { get; internal set; }

        /// <summary>
        /// Gameobject used to position the button on the panel. If you wish to offset the button,
        /// change BaseToolbarButton.transform.localPosition.
        /// </summary>
        public GameObject Anchor { get; internal set; }

        public ToolbarScrollView ScrollView { get; internal set; }

        /// <summary>
        /// Boolean indicating whether the button should active. If false, containing panel will skip over this button when refilled.
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    UpdateActive();
                }
            }
        }
        private bool isActive = false;

        /// <summary>
        /// Boolean indicating whether the user can interact with the button.
        /// </summary>
        public bool CanInteract
        {
            get { return canInteract; }
            internal set
            {
                if (canInteract != value)
                {
                    canInteract = value;
                    UpdateInteractable();
                }
            }
        }
        private bool canInteract = true;

        /// <summary>
        /// Boolean indicating whether the button is currently visible by the user.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            internal set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    UpdateVisibility();
                }
            }
        }
        private bool isVisible = true;

        public virtual void LateUpdate()
        {
            DisableWhenOutOfBounds();
        }

        /// <summary>
        /// Used to determine whether the button can be clicked.
        /// </summary>
        /// <returns>True if button can be clicked, false otherwise.</returns>
        public override bool CanBeInteractedNow()
        {
            return canInteract && base.CanBeInteractedNow();
        }

        /// <summary>
        /// Called when the value of IsActive changes
        /// </summary>
        public virtual void UpdateActive()
        {
            gameObject.SetActive(IsActive);
            if (ParentPanel != null)
            {
                ParentPanel.Refill();
            }
        }

        /// <summary>
        /// Called when the value of CanInteract changes
        /// </summary>
        public virtual void UpdateInteractable() { }

        /// <summary>
        /// Called when the value of IsVisible changes
        /// </summary>
        public virtual void UpdateVisibility()
        {
            SetSpritesEnabled(IsVisible);
        }

        /// <summary>
        /// Update IsVisible and CanInteract based on the current position of the button on the panel.
        /// Adapted from PotionCraft.ObjectBased.InteractiveItem.InventoryObject::DisableItemWhenOutOfBounds
        /// </summary>
        private void DisableWhenOutOfBounds()
        {
            if (ScrollView == null)
            {
                return;
            }

            if  ((ParentPanel == null) || !ParentPanel.IsOpen)
            {
                return;
            }

            if (ParentPanel is HorizontalToolbarPanel)
            {
                CanInteract = (transform.position.x < ScrollView.MaxInteractPos.x) && (transform.position.x > ScrollView.MinInteractPos.x);
                IsVisible = (transform.position.x < ScrollView.MaxVisiblePos.x) && (transform.position.x > ScrollView.MinVisiblePos.x);
            }
            else if (ParentPanel is VerticalToolbarPanel)
            {
                CanInteract = (transform.position.y < ScrollView.MaxInteractPos.y) && (transform.position.y > ScrollView.MinInteractPos.y);
                IsVisible = (transform.position.y < ScrollView.MaxVisiblePos.y) && (transform.position.y > ScrollView.MinVisiblePos.y);
            }
        }

        public override void UpdateSpriteAlpha(float alpha)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.SetColorAlpha(alpha);
            }
            if (spriteRendererIcon != null)
            {
                spriteRendererIcon.SetColorAlpha(alpha);
            }
            if (spriteRendererScratches != null)
            {
                spriteRendererScratches.SetColorAlpha(alpha);
            }
        }
    }
}