using BepInEx;
using BepInEx.Logging;
using PotionCraft.ManagersSystem;
using PotionCraft.ManagersSystem.Potion;
using PotionCraft.ObjectBased.UIElements.FloatingText;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using PotionCraft.ScriptableObjects;
using PotionCraft.Settings;
using Toolbar;
using Toolbar.UIElements.Buttons;
using UnityEngine;

namespace SimpleToolbarPlugin
{
    [BepInPlugin("com.github.rswallen.potioncraft.simpletoolbarplugin", "Simple Toolbar Plugin", "1.0.0")]
    [BepInDependency("com.github.rswallen.potioncraft.toolbar", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private static DelegateToolbarButton delegateButton;
        private static ConCmdToolbarButton conCmdButton;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"Simple Toolbar Plugin is loaded");
        }

        private void Start()
        {
            ToolbarWrapper.RegisterInit(InitCallback);
        }

        private static void InitCallback()
        {
            delegateButton = ToolbarWrapper.CreateDelegateButtonWithIcon("simpletoolbarplugin.delegate", "Astral1", SayHello, delegate ()
            {
                return new()
                {
                    header = "SimpleToolbarPlugin: DelegateToolbarButton",
                };
            }, true);
            delegateButton.IsActive = true;
            ToolbarWrapper.AddButtonToRootPanel(delegateButton);

            conCmdButton = ToolbarWrapper.CreateConCmdButtonWithIcon("simpletoolbarplugin.concmd", "Astral1", "STP-ButtonCommandTest 9001", true);
            conCmdButton.IsActive = true;
            ToolbarWrapper.AddButtonToRootPanel(conCmdButton);
        }

        internal static void SayHello()
        {
            Log.LogMessage("Hello");
            PrintRecipeMapMessage("Hello", new Vector2(0.5f, 2.5f));
        }

        public static void PrintRecipeMapMessage(string message, Vector2 offset)
        {
            var prefab = Settings<PotionManagerSettings>.Asset.collectedFloatingTextPrefab;
            Vector2 msgPos = Managers.RecipeMap.recipeMapObject.transmitterWindow.ViewRect.center + offset;
            var content = new CollectedFloatingText.FloatingTextContent(message, CollectedFloatingText.FloatingTextContent.Type.Text, 0f);
            CollectedFloatingText.SpawnNewText(prefab, msgPos, content, Managers.Game.Cam.transform, false, false);
        }
    }
}