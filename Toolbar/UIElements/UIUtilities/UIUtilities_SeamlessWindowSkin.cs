﻿using PotionCraft.ObjectBased.UIElements;
using UnityEngine;

namespace Toolbar.UIElements
{
    internal static partial class UIUtilities
    {
        internal static SeamlessWindowSkin SkinTemplate
        {
            get
            {
                _skinTemplate ??= GetTooltipSeamlessWindowSkin();
                return _skinTemplate;
            }
        }
        private static SeamlessWindowSkin _skinTemplate;

        internal static SeamlessWindowSkin SpawnSkin()
        {
            var template = SkinTemplate;
            if (template == null)
            {
                Log.LogError("Could not find a skin to clone. Impossible to create new SeamlessWindowSkin");
                return null;
            }

            var clone = Object.Instantiate(template);

            foreach (var renderer in clone.bg.renderers)
            {
                renderer.sortingLayerID = SortingLayerID;
                renderer.sortingOrder = 10;
            }
            clone.fg.renderer.sortingLayerID = SortingLayerID;
            clone.fg.renderer.sortingOrder = 15;

            clone.name = typeof(SeamlessWindowSkin).Name;
            clone.SetAlpha(1f);
            clone.gameObject.SetActive(true);
            return clone;
        }

        private static SeamlessWindowSkin GetTooltipSeamlessWindowSkin()
        {
            var skins = Resources.FindObjectsOfTypeAll<SeamlessWindowSkin>();
            if ((skins?.Length ?? 0) == 0)
            {
                return null;
            }

            foreach (var skin in skins)
            {
                if (skin != null)
                {
                    if (!skin.isActiveAndEnabled)
                    {
                        if ((skin.bg?.renderers?.Length ?? 0) != 0)
                        {
                            if (skin.bg.renderers[0]?.sprite != null)
                            {
                                if (skin.bg.renderers[0].sprite.name.Contains("Tooltips"))
                                {
                                    return skin;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
