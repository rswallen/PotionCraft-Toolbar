using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolbar
{
    internal static class TextureCache
    {
        private static readonly Dictionary<string, Texture2D> cache = new();

        public static Texture2D GetTexture(string name)
        {
            Texture2D tex = null;
            cache.TryGetValue(name, out tex);
            if (tex == null)
            {
                tex = Array.Find(Resources.FindObjectsOfTypeAll<Texture2D>(), (x => x.name == name));
                cache[name] = tex;
            }
            return tex;
        }
    }
}
