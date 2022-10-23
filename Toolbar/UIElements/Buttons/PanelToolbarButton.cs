using Toolbar.UIElements.Panels;

namespace Toolbar.UIElements.Buttons
{
    public abstract class PanelToolbarButton : BaseToolbarButton
    {
        public BaseToolbarPanel SubPanel { get; internal set; }

        private protected bool hasStarted { get; private set; } = false;

        public override void Start()
        {
            base.Start();
            hasStarted = true;
            UpdateLockedState();
        }

        public override void OnButtonReleasedPointerInside()
        {
            base.OnButtonReleasedPointerInside();
            SubPanel?.Toggle();
        }

        public override void UpdateLockedState()
        {
            if (!hasStarted)
            {
                return;
            }

            base.UpdateLockedState();
            if (Locked && (SubPanel != null))
            {
                SubPanel.IsOpen = false;
            }
        }

        public override void UpdateInteractable()
        {
            if (SubPanel?.IsOpen ?? false)
            {
                SubPanel.IsOpen = false;
            }
        }
    }
}