using UnityEngine;

namespace Toolbar.Extensions
{
    public static class SpriteRendererExtensions
    {
        public static void SetupToolbarSprite(this SpriteRenderer renderer, Sprite icon, bool rescale, float size = 0.75f)
        {
            if ((renderer == null) || (icon == null))
            {
                return;
            }

            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.sprite = icon;

            if (rescale)
            {
                float spriteSize = Mathf.Max(renderer.size.x, renderer.size.y);
                float scale = (spriteSize == 0f) ? size : (size / spriteSize);
                renderer.transform.localScale = Vector3.one * scale;
            }
        }
    }
}
