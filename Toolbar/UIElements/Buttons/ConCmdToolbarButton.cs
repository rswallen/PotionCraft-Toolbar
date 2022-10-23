using PotionCraft.ObjectBased.UIElements.Tooltip;
using QFSW.QC;
using Toolbar.Extensions;
using UnityEngine;

namespace Toolbar.UIElements.Buttons
{
    public class ConCmdToolbarButton : BaseToolbarButton
    {
        internal static ConCmdToolbarButton Create(Sprite icon, string command)
        {
            var button = BaseToolbarButton.Create<ConCmdToolbarButton>();

            button.spriteRenderer = UIUtilities.MakeRendererObj<SpriteRenderer>(button.gameObject, "MainRenderer", 100);
            button.spriteRenderer.SetupToolbarSprite(icon, true);

            button.Command = command;

            return button;
        }

        public string Command { get; private protected set; }

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            if (!string.IsNullOrEmpty(Command))
            {
                QuantumConsoleProcessor.InvokeCommand(Command, true);
            }
        }

        public override TooltipContent GetTooltipContent()
        {
            return new()
            {
                header = $"Command: '{Command}'",
            };
        }
    }
}
