using PotionCraft.ScriptableObjects;
using System.IO;
using Toolbar.UIElements;
using UnityEngine;

namespace Toolbar
{
    public static class ToolbarUtils
    {
        /// <summary>
        /// Creates a new Texture2D object and attempts to fill it with data from a given file.
        /// </summary>
        /// <param name="filePath">File to load texture data from.</param>
        /// <param name="output">Output reference to Texture2D object. Null if file doesn't exist or image loading failed.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool LoadTextureFromFile(string filePath, out Texture2D output)
        {
            if (!File.Exists(filePath))
            {
                output = null;
                return false;
            }
            var data = File.ReadAllBytes(filePath);

            // Do not create mip levels for this texture, use it as-is.
            output = new Texture2D(0, 0, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Bilinear,
            };

            bool result = output.LoadImage(data);
            if (!result)
            {
                UnityEngine.Object.Destroy(output);
                output = null;
            }
            return result;
        }

        /// <summary>
        /// Helper function for creating renderer components.
        /// </summary>
        /// <typeparam name="T">Type of renderer to create (eg: SpriteRenderer, SpriteMask).</typeparam>
        /// <param name="parent">GameObject to attach the transform of the new gameObject to.</param>
        /// <param name="objName">Name of the new gameObject.</param>
        /// <param name="sortOrder">Value to set Renderer.sortingOrder to.</param>
        /// <returns>Reference to freshly create Renderer of type T.</returns>
        public static T MakeRendererObj<T>(GameObject parent, string objName, int sortOrder) where T : Renderer
        {
            return UIUtilities.MakeRendererObj<T>(parent, objName, sortOrder);
        }

        /// <summary>
        /// Creates a sprite from a given Texture2D object, scaled such that the larger of width or height will be equal in length to size.
        /// </summary>
        /// <param name="texture">Texture2D object to create the sprite from.</param>
        /// <param name="size">Size (in Unity units) of the larger sprite dimension.</param>
        /// <returns>Reference to a freshly created sprite.</returns>
        public static Sprite MakeSprite(string name, Texture2D texture, float size = 0.75f)
        {
            if (texture == null)
            {
                return null;
            }
            float ppu = Mathf.Max(texture.width, texture.height) / (size == 0f ? 1f : size);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), ppu);
            sprite.name = name;
            return sprite;
        }

        /// <summary>
        /// Creates a sprite from a PotionCraft icon.
        /// </summary>
        /// <param name="iconName">Name of the icon to use.</param>
        /// <param name="randomizeColors">If true, colors will be randomized.</param>
        /// <returns>Reference to a freshly created sprite.</returns>
        public static Sprite GetSpriteFromColoredIcon(string iconName, bool randomizeColors)
        {
            var ico = Icon.GetByName(iconName).GetColoredIconWithDefaultColors();
            if (randomizeColors)
            {
                int length = ico.iconColors.Length;
                ico.iconColors = new Color[length];
                ico.areColorsCustom = new bool[length];

                for (int color = 0; color < length; color++)
                {
                    ico.iconColors[color] = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 0.60f, 0.75f);
                    ico.areColorsCustom[color] = true;
                }
            }
            return ico.GetSprite(ContourColor, true);
        }

        /// <summary>
        /// Grabs a random PotionCraft icon.
        /// </summary>
        /// <returns>A random icon.</returns>
        public static Icon GetRandomIcon()
        {
            int count = Icon.allIcons.Count;
            return Icon.allIcons[UnityEngine.Random.RandomRangeInt(0, count)];
        }

        /// <summary>
        /// Returns the color object used to specific the color of contours (creates a fresh object if cached value is null).
        /// </summary>
        public static ColorObject ContourColor
        {
            get
            {
                if (contourColor == null)
                {
                    contourColor ??= ScriptableObject.CreateInstance<ColorObject>();
                    contourColor.color = Color.black;
                }
                return contourColor;
            }
        }
        private static ColorObject contourColor = null;
    }
}
