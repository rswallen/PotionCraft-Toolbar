using BepInEx.Logging;
using UnityEngine;

namespace Toolbar.UIElements
{
    public static partial class UIUtilities
    {
        internal static ManualLogSource Log => ToolbarPlugin.Log;

        public static readonly string SortingLayerName = "TutorialHint";
        public static int SortingLayerID
        {
            get => SortingLayer.NameToID(SortingLayerName);
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
            GameObject obj = new()
            {
                name = objName,
                layer = LayerMask.NameToLayer("UI"),
            };
            obj.SetActive(true);
            obj.transform.SetParent(parent.transform, false);
            var sr = obj.AddComponent<T>();
            sr.sortingLayerID = SortingLayerID;
            sr.sortingOrder = sortOrder;
            return sr;
        }
    }
}