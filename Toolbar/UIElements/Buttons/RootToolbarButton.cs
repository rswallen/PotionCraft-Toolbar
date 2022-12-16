using PotionCraft.Core.Extensions;
using PotionCraft.LocalizationSystem;
using PotionCraft.ManagersSystem;
using PotionCraft.ObjectBased.UIElements;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using PotionCraft.ScriptableObjects;
using UnityEngine;

namespace Toolbar.UIElements.Buttons
{
    internal sealed class RootToolbarButton : PanelToolbarButton
    {
        public static RootToolbarButton Create()
        {
            var button = BaseToolbarButton.Create<RootToolbarButton>();
            button.transform.localPosition = new Vector2(0.1f, 0f);

            button.skinBgd = UIUtilities.SpawnSkin();
            button.skinBgd.transform.SetParent(button.Anchor.transform, false);
            button.skinBgd.UpdateSize(new(1.2f, 1.0f));

            Icon ico = Icon.GetByName("Poo2");

            button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
            button.spriteRenderer.sprite = ToolbarUtils.MakeSprite("RootToolbarButtonIconBackground", ico.textures[0]);
            button.SetFillHSV(Random.Range(0f, 1f));

            button.spriteRendererIcon = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "IconRenderer", 110);
            button.spriteRendererIcon.sprite = ToolbarUtils.MakeSprite("RootToolbarButtonIconContour", ico.contourTexture);
            button.spriteRendererIcon.color = Color.black;

            button.spriteRendererScratches = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "ScratchRenderer", 120);
            button.spriteRendererScratches.sprite = ToolbarUtils.MakeSprite("RootToolbarButtonIconScratches", ico.scratchesTexture);
            button.spriteRendererScratches.color = Color.black;

            button.IsActive = true;
            button.Locked = true;
            return button;
        }

        private SeamlessWindowSkin skinBgd;

        private float _hue = 0.00f;
        private float _sat = 0.60f;
        private float _val = 0.75f;

        public override void Awake()
        {
            base.Awake();
            Managers.Menu.onCurrentMenuChanged.AddListener(delegate ()
            {
                if (Managers.Menu.IsMenuOpened)
                {
                    Locked = true;
                    return;
                }
                SubPanel.UpdateParentButtonLocked();
            });
            Locked = true;
        }

        public override void WhenButtonIsSelected()
        {
            base.WhenButtonIsSelected();
            if (!Locked)
            {
                _hue += (Time.deltaTime * 0.1f);
                SetFillHSV(_hue, _sat, _val);
            }
        }

        public override void UpdateLockedState()
        {
            base.UpdateLockedState();
            spriteRenderer.color = Locked ? Color.HSVToRGB(0.25f, 0.25f, 0.50f) : Color.HSVToRGB(_hue, _sat, _val);
        }

        public override void UpdateSpriteAlpha(float alpha)
        {
            spriteRenderer.SetColorAlpha(alpha);
            spriteRendererIcon.SetColorAlpha(alpha);
            spriteRendererScratches.SetColorAlpha(alpha);
        }

        public override TooltipContent GetTooltipContent()
        {
            return Locked ? null : new TooltipContent()
            {
                header = new Key($"toolbar_opentoolbar_tooltip").GetText()
            };
        }

        public void SetFillHSV(float hue, float saturation = 0.60f, float value = 0.75f)
        {
            _hue = hue % 1f;
            if (_hue < 0f)
            {
                _hue = 1f - _hue;
            }
            _sat = Mathf.Clamp01(saturation);
            _val = Mathf.Clamp01(value);
            spriteRenderer.color = Color.HSVToRGB(_hue, _sat, _val);
        }
    }
}
