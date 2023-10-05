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

        //             //
        // UI Elements //
        //             //

        [Separator("Inventory UI", true)]
        [SerializeField] private UIDocument inventoryUI;
        public VisualElement root;
        public VisualElement descriptionElement;
        public VisualElement descriptionImage;
        public Label descriptionText;

        [Space]
        [Header("Inventory Slots")]
        public VisualElement inventoryGrid;
        public List<InventorySlot> inventorySlots = new List<InventorySlot>();

        [Space]
        [Header("Equipment Slots")]
        private VisualElement equipment;
        public InventorySlot primarySlot;
        public InventorySlot secondarySlot;

        [SerializeField] private InputActionReference _input_DropItem;
        [SerializeField] private InputActionReference _input_ToggleInventory;

        private void Awake()
        {

            //             //
            // UI Elements //
            //             //

            inventoryUI = GetComponent<UIDocument>();
            root = inventoryUI.rootVisualElement;

            descriptionElement = root.Q<VisualElement>("ItemDescription");
            descriptionImage = descriptionElement.Q<VisualElement>("Image");
            descriptionText = descriptionElement.Q<Label>("Description");

            inventoryGrid = root.Q<VisualElement>("InventoryGrid");

            equipment = root.Q<VisualElement>("Equipment");
            primarySlot = equipment.Q<InventorySlot>("PrimarySlot");
            secondarySlot = equipment.Q<InventorySlot>("SecondarySlot");

            foreach (InventorySlot inventorySlot in inventoryGrid.Children())
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
                        UpdateDescription(inventorySlot.item.description, inventorySlot.item.sprite);
                    }
                });

                inventorySlot.RegisterCallback<MouseOutEvent>((type) =>
                {
                    if (inventorySlot.item != Item.empty)
                        UpdateDescription(null, null);
                });
            }

            root.style.display = DisplayStyle.None;
        }

        private void OnEnable()
        {
            _input_ToggleInventory.action.Enable();
            _input_DropItem.action.Enable();

            _input_ToggleInventory.action.performed += ToggleInventoryUI;
            PlayerManager.InventoryData.Inventory.OnAddItem += AddItemToUI;
        }

        private void OnDisable()
        {
            _input_ToggleInventory.action.Disable();
            _input_DropItem.action.Disable();

            _input_ToggleInventory.action.performed -= ToggleInventoryUI;
            PlayerManager.InventoryData.Inventory.OnAddItem -= AddItemToUI;
            
        }

        private void UpdateDescription(string _description, Sprite _itemImage)
        {
            descriptionImage.style.backgroundImage = new StyleBackground(_itemImage);
            descriptionText.text = _description;
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
            if (root.style.display == DisplayStyle.Flex)
            {
                if (PlayerManager.MovementData.FreezeOnInventory)
                    Time.timeScale = 1;

                root.style.display = DisplayStyle.None;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                PlayerManager.InventoryData.InvokeOnInventoryStateChange(false);
                return;
            }

            if (PlayerManager.MovementData.FreezeOnInventory)
                Time.timeScale = 0;

            root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            PlayerManager.InventoryData.InvokeOnInventoryStateChange(true);
        }
    }

}
