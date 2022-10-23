using BepInEx.Logging;
using PotionCraft.ObjectBased.InteractiveItem;
using PotionCraft.ObjectBased.UIElements;
using PotionCraft.ObjectBased.UIElements.Scroll.Settings;
using System.Collections.Generic;
using System.Linq;
using Toolbar.UIElements.Buttons;
using Toolbar.UIElements.ScrollObjects;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolbar.UIElements.Panels
{
    public abstract class BaseToolbarPanel : InteractiveItem
    {
        /// <summary>
        /// Reference to plugin logger for debugging purposes. For internal use only.
        /// </summary>
        internal static ManualLogSource Log => ToolbarPlugin.Log;

        /// <summary>
        /// Constructor for creating a toolbar panel of type T.
        /// </summary>
        /// <typeparam name="T">Type of panel to create.</typeparam>
        /// <returns>Reference to a freshly created toolbar panel of type T.</returns>
        internal protected static T Create<T>() where T : BaseToolbarPanel
        {
            if (UIUtilities.SkinTemplate == null)
            {
                return default;
            }
            
            GameObject obj = new()
            {
                name = typeof(T).Name,
                layer = LayerMask.NameToLayer("UI"),
            };
            obj.SetActive(false);

            var panel = obj.AddComponent<T>();
            
            // create the skin that serves as the background
            panel.skin = UIUtilities.SpawnSkin();
            panel.skin.transform.SetParent(obj.transform, false);

            // sorting group stuff (from InteractiveItem)
            panel.sortingGroup = obj.AddComponent<SortingGroup>();
            panel.sortingGroup.sortingLayerID = UIUtilities.SortingLayerID;
            panel.sortingGroup.sortingOrder = 0;

            // Make ScrollView GameObject
            {
                GameObject scrollviewObj = new()
                {
                    name = typeof(ToolbarScrollView).Name,
                    layer = LayerMask.NameToLayer("UI"),
                };
                scrollviewObj.SetActive(true);
                scrollviewObj.transform.SetParent(obj.transform, false);

                panel.scrollview = scrollviewObj.AddComponent<ToolbarScrollView>();
                panel.scrollview.panel = panel;
                panel.scrollview.thisCollider = scrollviewObj.AddComponent<BoxCollider2D>();

                panel.scrollview.settings = ScriptableObject.CreateInstance<ScrollViewSettings>();
                panel.scrollview.settings.changeByScroll = 1.0f;
                panel.scrollview.settings.contentMoveAnimTime = 0.1f;
                panel.scrollview.settings.scrollDeadZone = 0.1f;
            }

            // Make ContentAnchor GameObject
            {
                panel.contentAnchor = new()
                {
                    name = "ContentAnchor",
                    layer = LayerMask.NameToLayer("UI"),
                };
                panel.contentAnchor.SetActive(true);
                panel.contentAnchor.transform.SetParent(obj.transform, false);

                // Make Content GameObject
                {
                    GameObject contentObj = new()
                    {
                        name = "Content",
                        layer = LayerMask.NameToLayer("UI"),
                    };
                    contentObj.SetActive(true);
                    panel.scrollview.content = contentObj.AddComponent<Content>();
                    panel.scrollview.content.transform.SetParent(panel.contentAnchor.transform, false);
                }
            }

            // Make ContentMask GameObject
            {
                var maskTexture = TextureCache.GetTexture("BoxMask");
                panel.contentMask = UIUtilities.MakeRendererObj<SpriteMask>(panel.gameObject, "ContentMask", 510);
                panel.contentMask.sprite = Sprite.Create(maskTexture, new(0f, 0f, maskTexture.width, maskTexture.height), new(0.5f, 0.5f));
            }

            // Make ContentFadse GameObject
            {
                var fadeTexture = TextureCache.GetTexture("RecipeBook Recipe Substrate");
                panel.contentFade = UIUtilities.MakeRendererObj<SpriteRenderer>(panel.gameObject, "ContentFade", 500);
                panel.contentFade.drawMode = SpriteDrawMode.Sliced;
                panel.contentFade.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                panel.contentFade.sprite = Sprite.Create(fadeTexture, new(300, 210, 200, 780), new(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new(5, 30, 5, 30));
            }

            panel.showOnlyFingerWhenInteracting = true;
            panel.raycastPriorityLevel = -13015;

            return panel;
        }

        /// <summary>
        /// Gameobject used to position the panel. Shared with ParentButton.
        /// </summary>
        public GameObject Anchor { get; internal set; }

        /// <summary>
        /// Button that changes the visibility of the panel.
        /// </summary>
        public BaseToolbarButton ParentButton { get; internal set; }

        /// <summary>
        /// Total length of the panel content, including padding and all active buttons.
        /// </summary>
        public float ContentLength { get; internal set; } = 1f;

        /// <summary>
        /// Unique ID used to identify the panel via the Toolbar API. 
        /// </summary>
        public string UID { get; internal set; } = "";

        /// <summary>
        /// Returns a list of the buttons in the panel. Cannot be used to add, remove or reorder buttons on the panel.
        /// </summary>
        public List<BaseToolbarButton> Buttons
        {
            get => buttons.ToList();
        }
        internal readonly HashSet<BaseToolbarButton> buttons = new();

        /// <summary>
        /// Anchor for all content objects (buttons). Parenting the anchor of each button to this and offsetting each active button by a different amount
        /// allows us to move just this in order to scroll through the buttons.
        /// </summary>
        private protected GameObject contentAnchor;

        /// <summary>
        /// Background skin.
        /// </summary>
        private protected SeamlessWindowSkin skin;

        /// <summary>
        /// Facilitates updating the visibility and interactivity of each buttons and managers the scrollbar.
        /// </summary>
        private protected ToolbarScrollView scrollview;

        /// <summary>
        /// Prevents buttons from being visible when they are outside the panel.
        /// </summary>
        private protected SpriteMask contentMask;

        /// <summary>
        /// A gradient to smooth the boundary of the content mask so buttons fade away over a short distance
        /// instead of abruptly disappearing
        /// </summary>
        private protected SpriteRenderer contentFade;
        
        private protected Vector3 padding = new(0.7f, 0.9f, 0.7f);
        private protected float minLength;
        private protected float maxLength;
        private protected float panelWidth = 1.2f;
        private protected Vector2 maskOffset = new(-0.2f, -0.3f);
        private protected Vector2 fadeOffset = new(0f, -0.2f);

        /// <summary>
        /// Open or close the panel by setting this to true or false.
        /// </summary>
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (isOpen != value)
                {
                    isOpen = value;
                    UpdateOpened();
                }
            }
        }
        private bool isOpen;

        public virtual void Start()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Add a button to the panel. If refill is set to false, the button position will not be updated.
        /// Only do this if you plan to add multiple buttons at once. Once you have finished adding buttons,
        /// call BaseToolbarPanel::Refill()
        /// </summary>
        /// <param name="button">The button to add. Button must not have been added to another panel.</param>
        /// <param name="refill">Defaults to true. Set to false if you plan on adding multiple buttons at once.</param>
        /// <returns>True on success, false otherwise.</returns>
        public virtual bool AddButton(BaseToolbarButton button, bool refill = true)
        {
            if ((button == null) || (button.ParentPanel != null) || buttons.Contains(button))
            {
                return false;
            }
            buttons.Add(button);

            button.Anchor.transform.SetParent(scrollview.content.transform, false);
            button.ScrollView = scrollview;
            button.ParentPanel = this;

            if (refill)
            {
                Refill();
                UpdateParentButtonLocked();
            }
            return true;
        }

        /// <summary>
        /// Remove a button from the panel. If refill is set to false, the positions of other buttons will not be updated.
        /// Only do this if you plan to remove multiple buttons at once. Once you have finished removing buttons,
        /// call BaseToolbarPanel::Refill()
        /// </summary>
        /// <param name="button">The button to remove. Button must have been added to this panel.</param>
        /// <param name="refill">Defaults to true. Set to false if you plan on removing multiple buttons at once.</param>
        /// <returns>True on success, false otherwise.</returns>
        public virtual bool RemoveButton(BaseToolbarButton button, bool refill = true)
        {
            if ((button == null) || (button.ParentPanel != this) || !buttons.Contains(button))
            {
                return false;
            }
            buttons.Remove(button);

            button.Anchor.transform.SetParent(null, false);
            button.ScrollView = null;
            button.ParentPanel = null;

            if (refill)
            {
                Refill();
                UpdateParentButtonLocked();
            }
            return true;
        }

        /// <summary>
        /// Disables all subpanels except the panel associated with the given button.
        /// </summary>
        /// <param name="notMe">The button of the subpanel to skip. Can be null.</param>
        public void DisableOtherPanels(PanelToolbarButton notMe)
        {
            foreach (BaseToolbarButton button in buttons)
            {
                if ((button is SubPanelToolbarButton subButton) && (subButton != notMe))
                {
                    if (subButton?.SubPanel != null)
                    {
                        subButton.SubPanel.IsOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// Repositions all buttons on the panel and resizes the panel and all components accordingly
        /// </summary>
        public abstract void Refill();

        /// <summary>
        /// Resizes all panel components according to the given Vector2
        /// </summary>
        /// <param name="newSize"></param>
        public abstract void UpdateSize(Vector2 newSize);

        /// <summary>
        /// Closes the panel if open, and vice versa
        /// </summary>
        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        /// <summary>
        /// Called when the panel is opened or closed
        /// </summary>
        private protected virtual void UpdateOpened()
        {
            if (!isOpen)
            {
                DisableOtherPanels(null);
            }
            gameObject.SetActive(isOpen);
        }

        /// <summary>
        /// Used to calculate the total length of a panel with the given number of buttons, including padding at each end.
        /// </summary>
        /// <param name="numButtons">Number of buttons to calculate the length for.</param>
        /// <returns>The calculated length.</returns>
        private protected float GetPanelLength(int numButtons)
        {
            int gaps = (numButtons < 1) ? 0 : numButtons - 1;
            return padding[0] + (gaps * padding[1]) + padding[2];
        }

        /// <summary>
        /// Recalculate the min/max length of the panel and refill it.
        /// </summary>
        /// <param name="maxButtons">New max number of buttons used to calculate the panel length.</param>
        internal void UpdateMinMaxLength(int maxButtons)
        {
            minLength = GetPanelLength(1);
            maxLength = GetPanelLength(maxButtons);
            Refill();
        }

        /// <summary>
        /// Lock the parent button of the panel if there are no active buttons on the panel. Unlock otherwise.
        /// </summary>
        public void UpdateParentButtonLocked()
        {
            if (ParentButton != null)
            {
                foreach (var button in buttons)
                {
                    if (button?.IsActive ?? false)
                    {
                        // if we find one active button, unlock parent button
                        ParentButton.Locked = false;
                        return;
                    }
                }
                ParentButton.Locked = true;
            }
        }
    }
}