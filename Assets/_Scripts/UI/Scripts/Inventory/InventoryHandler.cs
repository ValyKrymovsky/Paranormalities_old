using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine.InputSystem;
using MyCode.Managers;
using MyCode.GameData;

namespace MyCode.UI.Inventory
{
    public class InventoryHandler : MonoBehaviour
    {
        [Separator("Inventory UI", true)]
        [SerializeField] private UIDocument inventoryUI;
        private VisualElement _root;

        // Main panels
        private VisualElement _informationPanelL;
        private VisualElement _inventoryPanel;
        private VisualElement _informationPanelR;

        // Information panel left
        private VisualElement _healthElement;
        private VisualElement _notesElement;

        // Inventory panel
        private VisualElement _inventoryGrid;
        private VisualElement _itemInformation;
        private Label _itemName;
        private Label _itemDescription;

        // Information panel right
        private VisualElement _equipmentElement;
        private VisualElement _mapElement;


        private List<InventorySlot> inventorySlots = new List<InventorySlot>();

        private InventorySlot primarySlot;
        private InventorySlot secondarySlot;

        [SerializeField] private InputActionReference _input_DropItem;
        [SerializeField] private InputActionReference _input_ToggleInventory;

        private void Awake()
        {
            inventoryUI = GetComponent<UIDocument>();
            _root = inventoryUI.rootVisualElement;

            // Main panels
            _informationPanelL = _root.Q<VisualElement>("InformationPanelLeft");
            _inventoryPanel = _root.Q<VisualElement>("InventoryPanel");
            _informationPanelR = _root.Q<VisualElement>("InformationPanelRight");

            // Information panel left
            _healthElement = _informationPanelL.Q<VisualElement>("Health");
            _notesElement = _informationPanelL.Q<VisualElement>("Notes");

            // Inventory panel
            _inventoryGrid = _inventoryPanel.Q<VisualElement>("InventoryGrid");
            _itemInformation = _inventoryPanel.Q<VisualElement>("ItemDescription");
            _itemName = _itemInformation.Q<Label>("Name");
            _itemDescription = _itemInformation.Q<Label>("Description");

            // Information panel right
            _equipmentElement = _informationPanelR.Q<VisualElement>("Equipment");
            _mapElement = _informationPanelR.Q<VisualElement>("Map");

            primarySlot = _equipmentElement.Q<InventorySlot>("PrimarySlot");
            secondarySlot = _equipmentElement.Q<InventorySlot>("SecondarySlot");

            foreach (InventorySlot inventorySlot in _inventoryGrid.Children())
            {
                inventorySlots.Add(inventorySlot);
            }

            inventorySlots.Add(primarySlot);
            inventorySlots.Add(secondarySlot);

            foreach (InventorySlot inventorySlot in inventorySlots)
            {
                inventorySlot.RegisterCallback<MouseOverEvent>((type) =>
                {
                    if (inventorySlot.item != Item.empty)
                    {
                        UpdateDescription(inventorySlot.item);
                    }
                });

                inventorySlot.RegisterCallback<MouseOutEvent>((type) =>
                {
                    if (inventorySlot.item != Item.empty)
                        ResetDescription();
                });
            }

            _root.style.display = DisplayStyle.None;
        }

        private void OnEnable()
        {
            _input_ToggleInventory.action.Enable();
            _input_DropItem.action.Enable();

            _input_ToggleInventory.action.performed += ToggleInventoryUI;
            PlayerManager.Instance.InventoryData.Inventory.OnAddItem += AddItemToUI;
        }

        private void OnDisable()
        {
            _input_ToggleInventory.action.Disable();
            _input_DropItem.action.Disable();

            _input_ToggleInventory.action.performed -= ToggleInventoryUI;
            PlayerManager.Instance.InventoryData.Inventory.OnAddItem -= AddItemToUI;
            
        }

        private void UpdateDescription(Item item)
        {
            _itemName.text = item.itemName;
            _itemDescription.text = item.description;
        }

        private void ResetDescription()
        {
            _itemName.text = string.Empty;
            _itemDescription.text = string.Empty;
        }


        private void AddItemToUI(Item _item, SlotType _slotType)
        {
            switch(_slotType)
            {
                case SlotType.Normal:
                    GetFirstEmptySlot().SetItemParameters(_item);
                    break;

                case SlotType.Primary:
                    primarySlot.SetItemParameters(_item);
                    break;

                case SlotType.Secondary:
                    secondarySlot.SetItemParameters(_item);
                    break;
            }
        }

        private InventorySlot GetFirstEmptySlot()
        {
            List<InventorySlot> normalSlots = new List<InventorySlot>();
            for(int i = 0; i < inventorySlots.Count - 2; i++)
                normalSlots.Add(inventorySlots[i]);

            foreach (InventorySlot inventorySlot in normalSlots)
            {
                if (inventorySlot.item == Item.empty)
                {
                    return inventorySlot;
                }
                continue;
            }
            return null;
        }

        private InventorySlot GetEmptyEquipmentSlot()
        {
            if (primarySlot.item == Item.empty)
            {
                return primarySlot;
            }
            else if (secondarySlot.item == Item.empty)
            {
                return secondarySlot;
            }

            return null;
        }

        private void ToggleInventoryUI(InputAction.CallbackContext _ctx)
        {
            if (_root.style.display == DisplayStyle.Flex)
            {
                if (PlayerManager.Instance.MovementData.FreezeOnInventory)
                    Time.timeScale = 1;

                _root.style.display = DisplayStyle.None;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                PlayerManager.Instance.InventoryData.InvokeOnInventoryStateChange(false);
                return;
            }

            if (PlayerManager.Instance.MovementData.FreezeOnInventory)
                Time.timeScale = 0;

            _root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            PlayerManager.Instance.InventoryData.InvokeOnInventoryStateChange(true);
        }
    }

}
