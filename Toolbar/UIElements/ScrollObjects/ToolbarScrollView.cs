using PotionCraft.ObjectBased.UIElements;
using PotionCraft.ObjectBased.UIElements.Scroll;
using Toolbar.UIElements.Panels;
using UnityEngine;

namespace Toolbar.UIElements.ScrollObjects
{
    public class ToolbarScrollView : ScrollView
    {
        public override void Start()
        {
            base.Start();
            contentColliderSize = thisCollider.size;
        }

        public void LateUpdate()
        {
            contentWidth = GetContentWidth(true);
            if (horizontalScroll != null)
            {
                CalculateOversize();
                if (activeHorizontal && oversize.x <= 0f)
                {
                    SetScrollActiveState(false);
                    return;
                }
                if (!activeHorizontal && oversize.x > 0f)
                {
                    SetScrollActiveState(true);
                }
            }
        }

        public override void CalculateOversize()
        {
            oversize = Vector2.zero;
            if (verticalScroll != null)
            {
                oversize += (contentHeight - contentColliderSize.y) * Vector2.up;
            }
            if (horizontalScroll != null)
            {
                oversize += (contentWidth - contentColliderSize.x) * Vector2.right;
            }
        }

        public override void SetPositionTo(float value, bool animated = false)
        {
            if (activeVertical)
            {
                content.transform.localPosition = new Vector2(0f, value * oversize.y);
            }
            if (activeHorizontal)
            {
                content.transform.localPosition = new Vector2(value * -oversize.x, 0f);
            }
        }

        public override float GetContentHeight(bool withCustomIncrease = true)
        {
            return Mathf.Max(panel.ContentLength, 1f) + (withCustomIncrease ? content.increaseBy.y : 0f);
        }

        public float GetContentWidth(bool withCustomIncrease = true)
        {
            return Mathf.Max(panel.ContentLength, 1f) + (withCustomIncrease ? content.increaseBy.x : 0f);
        }

        public void UpdateSize(Vector2 newSize)
        {
            thisCollider.size = newSize;
            contentColliderSize = thisCollider.size;
        }

        public Content content;
        public BoxCollider2D thisCollider;
        internal BaseToolbarPanel panel;

        private float contentWidth;

        private Vector2 minInteractPos;
        private Vector2 minVisiblePos;

        private Vector2 maxInteractPos;
        private Vector2 maxVisiblePos;

        public Vector2 MinInteractPos
        {
            get
            {
                UpdateMinMaxCoordinates();
                return minInteractPos;
            }
        }

        public Vector2 MaxInteractPos
        {
            get
            {
                UpdateMinMaxCoordinates();
                return maxInteractPos;
            }
        }

        public Vector2 MinVisiblePos
        {
            get
            {
                UpdateMinMaxCoordinates();
                return minVisiblePos;
            }
        }

        public Vector2 MaxVisiblePos
        {
            get
            {
                UpdateMinMaxCoordinates();
                return maxVisiblePos;
            }
        }
        
        private int lastFrameUpdate;

        private void UpdateMinMaxCoordinates()
        {
            if (lastFrameUpdate == Time.frameCount)
            {
                return;
            }
            lastFrameUpdate = Time.frameCount;

            Vector2 vector = transform.position;
            vector += thisCollider.offset;
            Vector2 vector2 = 0.5f * thisCollider.bounds.size;

            Vector2 minBasePos = vector - vector2;
            Vector2 maxBasePos = vector + vector2;

            Vector2 interactOffset = 0.55f * (Vector2)transform.lossyScale;
            minInteractPos = minBasePos + interactOffset;
            maxInteractPos = maxBasePos - interactOffset;

            Vector2 visibleOffset = 1.5f * (Vector2)transform.lossyScale;
            minVisiblePos = minBasePos - visibleOffset;
            maxVisiblePos = maxBasePos + visibleOffset;
            
        }
    }
}