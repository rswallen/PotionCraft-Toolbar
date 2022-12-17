using BepInEx.Logging;
using UnityEngine;

namespace Toolbar.UIElements
{
    internal static partial class UIUtilities
    {
        internal static ManualLogSource Log => ToolbarPlugin.Log;

        public static readonly string SortingLayerName = "TutorialHint";
        public static int SortingLayerID
        {
            get => SortingLayer.NameToID(SortingLayerName);
        }

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