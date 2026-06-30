using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.InventoryEngine
{
	public class MyInventoryDisplay: InventoryDisplay
	{
        static MyInventoryDisplay _instance;
        public static MyInventoryDisplay Instance { get { return _instance; } }

        // Use this for initialization
        void Start()
		{
            if (_instance == null)
            {               
                _instance = this;
            }
        }
       
        /// <summary>
		/// Draws the content of the inventory (slots and icons)
		/// </summary>
		protected override void DrawInventoryContent()
        {
            if (SlotContainer != null)
            {
                SlotContainer.Clear();
            }
            else
            {
                SlotContainer = new List<InventorySlot>();
            }
            // we initialize our sprites 
            if (EmptySlotImage == null)
            {
                InitializeSprites();
            }
            // we remove all existing slots
            foreach (InventorySlot slot in transform.GetComponentsInChildren<InventorySlot>())
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(slot.gameObject);
                }
                else
                {
                    Destroy(slot.gameObject);
                }
            }
            // for each slot we create the slot and its content
            for (int i = 0; i < TargetInventory.Content.Length; i++)
            {
                DrawSlot(i);
            }

            if (_slotPrefab != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_slotPrefab.gameObject);
                    _slotPrefab = null;
                }
                else
                {
                    DestroyImmediate(_slotPrefab.gameObject);
                    _slotPrefab = null;
                }
            }

            if (EnableNavigation)
            {
                SetupSlotNavigation();
            }
        }

        /// <summary>
		/// Creates the slot prefab to use in all slot creations
		/// </summary>
		protected override void InitializeSlotPrefab()
        {
            if (SlotPrefab != null)
            {
                _slotPrefab = Instantiate(SlotPrefab);
            }
            else
            {
                GameObject newSlot = new GameObject();
                newSlot.AddComponent<RectTransform>();

                newSlot.AddComponent<Image>();
                newSlot.MMGetComponentNoAlloc<Image>().raycastTarget = true;

                _slotPrefab = newSlot.AddComponent<MyInventorySlot>();
                _slotPrefab.transition = Selectable.Transition.SpriteSwap;

                Navigation explicitNavigation = new Navigation();
                explicitNavigation.mode = Navigation.Mode.Explicit;
                _slotPrefab.GetComponent<MyInventorySlot>().navigation = explicitNavigation;

                _slotPrefab.interactable = true;

                newSlot.AddComponent<CanvasGroup>();
                newSlot.MMGetComponentNoAlloc<CanvasGroup>().alpha = 1;
                newSlot.MMGetComponentNoAlloc<CanvasGroup>().interactable = true;
                newSlot.MMGetComponentNoAlloc<CanvasGroup>().blocksRaycasts = true;
                newSlot.MMGetComponentNoAlloc<CanvasGroup>().ignoreParentGroups = false;

                // we add the icon
                GameObject itemIcon = new GameObject("Slot Icon", typeof(RectTransform));
                itemIcon.transform.SetParent(newSlot.transform);
                UnityEngine.UI.Image itemIconImage = itemIcon.AddComponent<Image>();
                _slotPrefab.IconImage = itemIconImage;
                RectTransform itemRectTransform = itemIcon.GetComponent<RectTransform>();
                itemRectTransform.localPosition = Vector3.zero;
                itemRectTransform.localScale = Vector3.one;
                MMGUI.SetSize(itemRectTransform, IconSize);

                // we add the quantity placeholder
                GameObject textObject = new GameObject("Slot Quantity", typeof(RectTransform));
                textObject.transform.SetParent(itemIcon.transform);
                Text textComponent = textObject.AddComponent<Text>();
                _slotPrefab.QuantityText = textComponent;
                textComponent.font = QtyFont;
                textComponent.fontSize = QtyFontSize;
                textComponent.color = QtyColor;
                textComponent.alignment = QtyAlignment;
                RectTransform textObjectRectTransform = textObject.GetComponent<RectTransform>();
                textObjectRectTransform.localPosition = Vector3.zero;
                textObjectRectTransform.localScale = Vector3.one;
                MMGUI.SetSize(textObjectRectTransform, (SlotSize - Vector2.one * QtyPadding));

                _slotPrefab.name = "SlotPrefab";
            }
        }
    }
}