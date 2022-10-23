using PotionCraft.ObjectBased.UIElements.Tooltip;
using System;
using Toolbar.Extensions;
using UnityEngine;

namespace Toolbar.UIElements.Buttons
{
    public sealed class DelegateToolbarButton : BaseToolbarButton
    {
        internal static DelegateToolbarButton Create(Sprite icon, Action onRelease, Func<TooltipContent> getTooltip)
        {
            var button = BaseToolbarButton.Create<DelegateToolbarButton>();
            
            button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
            button.spriteRenderer.SetupToolbarSprite(icon, true);

            button.onRelease = onRelease;
            button.getTooltip = getTooltip;

            return button;
        }

        private Action onRelease;
        private Func<TooltipContent> getTooltip;

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            onRelease?.Invoke();
        }

        public override TooltipContent GetTooltipContent()
        {
            return getTooltip?.Invoke();
        }
    }
}