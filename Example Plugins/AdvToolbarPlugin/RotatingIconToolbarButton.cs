using BepInEx.Logging;
using PotionCraft.ObjectBased.UIElements.Tooltip;
using System.Linq;
using Toolbar;
using Toolbar.Extensions;
using Toolbar.UIElements;
using Toolbar.UIElements.Buttons;
using UnityEngine;

namespace AdvToolbarPlugin
{
    internal sealed class RotatingIconToolbarButton : BaseToolbarButton
    {
        internal static ManualLogSource Log => Plugin.Log;

        public static RotatingIconToolbarButton Create()
        {
            if (!GetTextures(out Texture2D active, out Texture2D idle, out Texture2D quest))
            {
                return null;
            }

            var button = ToolbarAPI.CreateCustomButton<RotatingIconToolbarButton>();

            button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
            button.spriteRenderer.SetupToolbarSprite(ToolbarUtils.MakeSprite("RotatingButtonIdle", idle), false);

            button.hoveredSprite = ToolbarUtils.MakeSprite("RotatingButtonActive", active);
            button.pressedSprite = button.hoveredSprite;
            button.lockedSprite = button.spriteRenderer.sprite;
            button.normalSprite = button.spriteRenderer.sprite;

            button.spriteRendererIcon = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "IconRenderer", 110);
            button.spriteRendererIcon.SetupToolbarSprite(ToolbarUtils.MakeSprite("RotatingButtonFront", quest, 0.5f), false);

            button.hoveredSpriteIcon = button.spriteRendererIcon.sprite;
            button.pressedSpriteIcon = button.spriteRendererIcon.sprite;
            button.normalSpriteIcon = button.spriteRendererIcon.sprite;
            button.lockedSpriteIcon = button.spriteRendererIcon.sprite;

            return button;
        }

        private bool clockwise = true;

        public override void WhenButtonIsSelected()
        {
            base.WhenButtonIsSelected();
            float current = spriteRenderer.transform.localEulerAngles.z;
            current += Time.deltaTime * (clockwise ? -180f : 180f);
            spriteRenderer.transform.localEulerAngles = Vector3.forward * current;
        }

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            clockwise = !clockwise;
        }

        public override TooltipContent GetTooltipContent()
        {
            return new()
            {
                header = Locked ? "Locked" : clockwise ? "Round and round and round it goes..." : "...seog ti dnuor dna dnuor dna dnuoR",
            };
        }

        private static bool GetTextures(out Texture2D active, out Texture2D idle, out Texture2D quest)
        {
            active = null;
            idle = null;
            quest = null;

            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            var rotating = textures.Where(x => x.name.Contains("RadialMenu OptionsIcon"));
            
            foreach (var texture in rotating)
            {
                if (texture.name.EndsWith("Active"))
                {
                    active = texture;
                    continue;
                }
                if (texture.name.EndsWith("Idle"))
                {
                    idle = texture;
                    continue;
                }
            }

            quest = textures.Where(x => x.name.Equals("Quest Icon Active")).First();

            bool valid = true;
            if (active == null)
            {
                Log.LogError("Couldn't find 'RadialMenu OptionsIcon Active'");
                valid = false;
            }

            if (idle == null)
            {
                Log.LogError("Couldn't find 'RadialMenu OptionsIcon Idle'");
                valid = false;
            }

            if (quest == null)
            {
                Log.LogError("Couldn't find 'Quest Icon Active'");
                valid = false;
            }

            return valid;
        }
    }
}
