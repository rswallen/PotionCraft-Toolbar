using PotionCraft.Core.Extensions;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using System;
using Toolbar.UIElements.Panels;
using UnityEngine;

namespace Toolbar.UIElements.Buttons
{
    public class SubPanelToolbarButton : PanelToolbarButton
    {
        internal static T Create<T>(string panelUID) where T : SubPanelToolbarButton
        {
            var button = BaseToolbarButton.Create<T>();
            button.spriteRendererBg = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "BgdRenderer", 90);
            button.spriteRendererBg.color = new(0.5f, 0.5f, 0.1f, button.bgAlpha);
            button.spriteRendererBg.enabled = Settings.ShowSubPanelIndicators;
            button.spriteRendererBg.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            button.spriteRendererBg.sprite = ToolbarUtils.MakeSprite("SubPanelButtonIndicator", TextureCache.GetTexture("RadialMenu Dot Substrate"), 0.9f);
            
            button.SubPanel = HorizontalToolbarPanel.Create(button.Anchor, button);
            button.SubPanel.transform.localPosition = new(0f, -0.1f);
            button.SubPanel.UID = panelUID;

            button.Locked = true;
            return (T)button;
        }

        internal SpriteRenderer spriteRendererBg;
        internal Func<TooltipContent> getTooltip;

        public float bgAlpha = 0.8f;

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            ParentPanel?.DisableOtherPanels(this);
        }

        public override void UpdateVisibility()
        {
            base.UpdateVisibility();
            spriteRendererBg.enabled = Settings.ShowSubPanelIndicators && IsVisible;
        }

        public override void UpdateSpriteAlpha(float alpha)
        {
            base.UpdateSpriteAlpha(alpha);
            spriteRendererBg.SetColorAlpha(bgAlpha * alpha);
        }

        public override TooltipContent GetTooltipContent()
        {
            return getTooltip?.Invoke();
        }
    }
}
