﻿using PotionCraft.Core.Extensions;
using PotionCraft.ObjectBased.UIElements;
using PotionCraft.ObjectBased.UIElements.Scroll;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolbar.UIElements
{
    public static partial class UIUtilities
    {
        internal static Scroll ScrollTemplate
        {
            get
            {
                _scrollTemplate ??= GetScrollFromDescriptionWindow();
                return _scrollTemplate;
            }
        }
        private static Scroll _scrollTemplate;

        internal static Scroll SpawnScroll()
        {
            var template = ScrollTemplate;
            if (template == null)
            {
                Log.LogError("Could not find a Scroll to clone. Impossible to create new Scroll");
                return null;
            }
            
            var clone = Object.Instantiate(template);
            clone.name = typeof(Scroll).Name;

            var pointer = clone.GetComponentInChildren<ScrollPointer>();
            pointer.activeOnStart = false;
            pointer.spriteRenderer.sortingLayerID = SortingLayerID;
            pointer.spriteRenderer.sortingOrder = 540;
            pointer.raycastPriorityLevel = -21500;

            var axis = clone.GetComponentInChildren<ScrollAxis>();
            axis.thisCollider = axis.GetComponent<CapsuleCollider2D>();
            axis.raycastPriorityLevel = -21500;

            var sprite = axis.spriteRenderer.sprite;
            axis.spriteRenderer.sprite = null;
            Object.Destroy(axis.spriteRenderer, 0.001f);
            axis.spriteRenderer = MakeRendererObj<SpriteRenderer>(axis.gameObject, "Sprite", 530);
            axis.spriteRenderer.sprite = sprite;
            axis.spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            axis.backgroundSpriteRenderer = axis.spriteRenderer;

            clone.gameObject.SetActive(true);
            return clone;
        }

        internal static Scroll GetScrollFromDescriptionWindow()
        {
            var scrolls = Resources.FindObjectsOfTypeAll<Scroll>();
            if ((scrolls?.Length ?? 0) == 0)
            {
                return null;
            }

            foreach (var scroll in scrolls)
            {
                if (scroll != null)
                {
                    if (!scroll.isActiveAndEnabled)
                    {
                        if (scroll.gameObject.FullName(false) == "DescriptionWindow/Anchor/InputFieldCanvas/VerticalScroll/Vertical")
                        {
                            return scroll;
                        }
                    }
                }
            }
            return null;
        }
    }
}
