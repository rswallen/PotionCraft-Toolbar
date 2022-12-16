using BepInEx.Logging;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using Toolbar;
using Toolbar.Extensions;
using Toolbar.UIElements;
using Toolbar.UIElements.Buttons;
using UnityEngine;

namespace AdvToolbarPlugin
{
    internal sealed class MagicToolbarButton : BaseToolbarButton
    {
        internal static ManualLogSource Log => Plugin.Log;

        public static MagicToolbarButton Create(string buttonUID, bool revealOtherButtons)
        {
            var button = ToolbarAPI.CreateCustomButton<MagicToolbarButton>(buttonUID);

            var iconName = ToolbarUtils.GetRandomIcon().name;

            button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
            button.spriteRenderer.SetupToolbarSprite(ToolbarUtils.GetSpriteFromColoredIcon(iconName, true), true);

            button.hoveredSprite = button.spriteRenderer.sprite;
            button.pressedSprite = button.spriteRenderer.sprite;
            button.normalSprite = button.spriteRenderer.sprite;
            button.lockedSprite = button.spriteRenderer.sprite;

            button.revealOthers = revealOtherButtons;
            
            return button;
        }

        private bool revealOthers;

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            if (revealOthers)
            {
                foreach (var button in ParentPanel.Buttons)
                {
                    button.IsActive = true;
                }
            }
            else
            {
                this.IsActive = false;
            }
        }

        public override TooltipContent GetTooltipContent()
        {
            return new()
            {
                header = Locked ? "Locked" : revealOthers ? "Click me to make the others reappear" : "Click me to make me vanish",
            };
        }
    }
}
