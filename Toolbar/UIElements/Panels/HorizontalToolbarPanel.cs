using PotionCraft.ObjectBased.UIElements.Scroll;
using System;
using Toolbar.UIElements.Buttons;
using UnityEngine;

namespace Toolbar.UIElements.Panels
{
    public sealed class HorizontalToolbarPanel : BaseToolbarPanel
    {
        internal static HorizontalToolbarPanel Create(GameObject anchor, BaseToolbarButton button)
        {
            if (UIUtilities.ScrollTemplate == null)
            {
                return null;
            }

            var panel = Create<HorizontalToolbarPanel>();
            panel.transform.SetParent(anchor.transform, false);
            panel.Anchor = anchor;
            panel.ParentButton = button;

            GameObject scrollObj = new()
            {
                name = "HorizontalScroll",
                layer = LayerMask.NameToLayer("UI"),
            };
            scrollObj.SetActive(true);
            scrollObj.transform.SetParent(panel.transform, false);
            scrollObj.transform.localPosition = new Vector2(0f, -0.4f);

            var scroll = UIUtilities.SpawnScroll();
            scroll.transform.SetParent(scrollObj.transform, false);
            scroll.scrollType = Scroll.ScrollType.Horizontal;

            panel.scrollview.horizontalScroll = scrollObj;
            panel.scrollview.horizontalScrollPointer = scrollObj.GetComponentInChildren<ScrollPointer>();
            panel.scrollview.horizontalScrollPointer.scrollView = panel.scrollview;
            panel.scrollview.horizontalScrollPointer.disableXPositionChanging = false;
            panel.scrollview.horizontalScrollPointer.disableYPositionChanging = true;

            var axis = scrollObj.GetComponentInChildren<ScrollAxis>();
            axis.spriteRenderer.transform.localEulerAngles = new(0f, 0f, 90f);
            var collider = axis.thisCollider as CapsuleCollider2D;
            collider.direction = CapsuleDirection2D.Horizontal;

            panel.contentFade.transform.localEulerAngles = new(0f, 0f, 90f);

            panel.maskOffset = new(-0.3f, -0.2f);

            panel.IsOpen = true;
            return panel;
        }

        public override void Awake()
        {
            base.Awake();
            UpdateMinMaxLength(Settings.MaxButtonsHorz);
        }

        public override bool AddButton(BaseToolbarButton button, bool refill = true)
        {
            if (button is PanelToolbarButton)
            {
                return false;
            }
            return base.AddButton(button, refill);
        }

        public override void Refill()
        {
            int numButtons = 0;
            int position = 0;
            foreach (var button in buttons)
            {
                if ((button != null) && button.IsActive)
                {
                    button.Anchor.transform.localPosition = (padding[0] + (position++ * padding[1])) * Vector2.right;
                    numButtons++;
                }
            }
            ContentLength = GetPanelLength(numButtons);
            var length = Mathf.Clamp(ContentLength, minLength, maxLength);
            UpdateSize(new Vector2(length, panelWidth));
        }

        public override void UpdateSize(Vector2 newSize)
        {
            transform.localPosition = new Vector2(0.5f + (newSize.x / 2f), -0.1f);

            skin.UpdateSize(newSize);
            
            contentAnchor.transform.localPosition = new(-newSize.x / 2f, 0.1f);
            
            float scrollLength = newSize.x - 0.4f;

            var axis = scrollview.horizontalScrollPointer.axis;
            axis.spriteRenderer.size = new(0.1f, scrollLength);
            (axis.thisCollider as CapsuleCollider2D).size = new(scrollLength, 0.15f);

            float scrollHalfLength = scrollLength / 2f;

            var pointer = scrollview.horizontalScrollPointer;
            pointer.startPosition = new(-scrollHalfLength, 0f);
            pointer.endPosition = new(scrollHalfLength, 0f);

            float maskScaleX = ((newSize.x + maskOffset.x) / contentMask.sprite.rect.width) * contentMask.sprite.pixelsPerUnit;
            float maskScaleY = ((newSize.y + maskOffset.y) / contentMask.sprite.rect.height) * contentMask.sprite.pixelsPerUnit;
            contentMask.transform.localScale = new(maskScaleX, maskScaleY);

            contentFade.size = new(newSize.y + fadeOffset.y, newSize.x + fadeOffset.x);

            scrollview.UpdateSize(newSize);
            scrollview.SetPositionTo(0f, false);
            pointer.SetPosition(0f, true, false);
        }
    }
}