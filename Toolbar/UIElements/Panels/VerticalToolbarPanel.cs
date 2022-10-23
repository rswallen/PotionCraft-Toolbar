using PotionCraft.ObjectBased.UIElements.Scroll;
using System.Linq;
using Toolbar.UIElements.Buttons;
using UnityEngine;

namespace Toolbar.UIElements.Panels
{
    public sealed class VerticalToolbarPanel : BaseToolbarPanel
    {
        internal static VerticalToolbarPanel Create(GameObject anchor, BaseToolbarButton button)
        {
            if (UIUtilities.ScrollTemplate == null)
            {
                return null;
            }

            var panel = Create<VerticalToolbarPanel>();
            panel.transform.SetParent(anchor.transform, false);
            panel.Anchor = anchor;
            panel.ParentButton = button;
            
            GameObject scrollObj = new()
            {
                name = "VerticalScroll",
                layer = LayerMask.NameToLayer("UI"),
            };
            scrollObj.SetActive(true);
            scrollObj.transform.SetParent(panel.transform, false);
            scrollObj.transform.localPosition = new Vector2(-0.4f, 0f);

            var scroll = UIUtilities.SpawnScroll();
            scroll.transform.SetParent(scrollObj.transform, false);

            panel.scrollview.verticalScroll = scrollObj;
            panel.scrollview.verticalScrollPointer = scrollObj.GetComponentInChildren<ScrollPointer>();
            panel.scrollview.verticalScrollPointer.scrollView = panel.scrollview;

            panel.contentFade.transform.localPosition = new(0.15f, 0f);

            panel.maskOffset = new(-0.2f, -0.3f);

            panel.IsOpen = true;
            return panel;
        }

        public override void Awake()
        {
            base.Awake();
            UpdateMinMaxLength(Settings.MaxButtonsVert);
        }

        public override void Refill()
        {
            // start from the end of the list and work backwards
            int numButtons = 0;
            int position = 0;
            foreach (var button in buttons.Reverse())
            {
                if ((button != null) && button.IsActive)
                {
                    button.Anchor.transform.localPosition = (padding[0] + (position++ * padding[1])) * Vector2.down;
                    numButtons++;
                }
            }

            ContentLength = GetPanelLength(numButtons);
            var length = Mathf.Clamp(ContentLength, minLength, maxLength);
            UpdateSize(new Vector2(panelWidth, length));
        }

        public override void UpdateSize(Vector2 newSize)
        {
            transform.localPosition = new Vector2(0f, 0.5f + (newSize.y / 2f));

            skin.UpdateSize(newSize);
            
            contentAnchor.transform.localPosition = new(0.1f, newSize.y / 2f);
            
            float scrollLength = newSize.y - 0.4f;

            var axis = scrollview.verticalScrollPointer.axis;
            axis.spriteRenderer.size = new(0.1f, scrollLength);
            (axis.thisCollider as CapsuleCollider2D).size = new(0.15f, scrollLength);

            float scrollHalfLength = scrollLength / 2f;

            var pointer = scrollview.verticalScrollPointer;
            pointer.startPosition = new(0f, scrollHalfLength);
            pointer.endPosition = new(0f, -scrollHalfLength);

            float maskScaleX = ((newSize.x + maskOffset.x) / contentMask.sprite.rect.width) * contentMask.sprite.pixelsPerUnit;
            float maskScaleY = ((newSize.y + maskOffset.y) / contentMask.sprite.rect.height) * contentMask.sprite.pixelsPerUnit;
            contentMask.transform.localScale = new(maskScaleX, maskScaleY);

            contentFade.size = new(newSize.x + fadeOffset.x, newSize.y + fadeOffset.y);

            // update scrollview collider
            scrollview.UpdateSize(newSize);
            scrollview.SetPositionTo(1f, false);
            pointer.SetPosition(1f, true, false);
        }
    }
}