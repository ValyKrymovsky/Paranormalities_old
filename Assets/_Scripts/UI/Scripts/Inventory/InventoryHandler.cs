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

        [Space]
        [Header("Ghost Icon")]
        private static VisualElement ghostIcon;
        private static bool isDragging;
        private static InventorySlot originalSlot;

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

            ghostIcon = root.Q<VisualElement>("GhostIcon");

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
                        UpdateDescription(inventorySlot.item.description, inventorySlot.item.itemIcon);
                    }
                });

                inventorySlot.RegisterCallback<MouseOutEvent>((type) =>
                {
                    if (inventorySlot.item != Item.empty)
                        UpdateDescription(null, null);
                });
            }

            root.style.display = DisplayStyle.None;

            ghostIcon.RegisterCallback<PointerMoveEvent>(DragItem);
            ghostIcon.RegisterCallback<PointerUpEvent>(PlaceItem);
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

        public static void TakeItem(Vector2 _position, InventorySlot _originalInventorySlot)
        {
            isDragging = true;
            originalSlot = _originalInventorySlot;

            ghostIcon.style.top = _position.y - ghostIcon.layout.height / 2;
            ghostIcon.style.left = _position.x - ghostIcon.layout.width / 2;

            ghostIcon.style.backgroundImage = new StyleBackground(_originalInventorySlot.GetItemImage());

            ghostIcon.style.visibility = Visibility.Visible;
        }

        private void DragItem(PointerMoveEvent _event)
        {
            if (!isDragging) return;

            ghostIcon.style.top = _event.position.y - ghostIcon.layout.height / 2;
            ghostIcon.style.left = _event.position.x - ghostIcon.layout.width / 2;
        }

        private void PlaceItem(PointerUpEvent _event)
        {
            if (!isDragging) return;

            IEnumerable<InventorySlot> overlapingSlots = inventorySlots.Where(x => x.worldBound.Overlaps(ghostIcon.worldBound));

            if (overlapingSlots.Count() == 0)
            {
                ReturnToOriginalSlot(originalSlot);
                StopDragging();
                return;
            }

            // Original Slot = dragged from, New Slot = dragged to
            InventorySlot newSlot = overlapingSlots.OrderBy(x => Vector2.Distance(x.worldBound.position, ghostIcon.worldBound.position)).First();
            Item originalSlotItem = originalSlot.item;
            originalSlotItem.itemIcon = ghostIcon.style.backgroundImage.value.sprite;

            // Returns item to original slot if the item is not equipment and is trying to go to equipment slots
            if (originalSlot.item.itemType != ItemType.Equipment &&
            newSlot.name != "InventorySlot")
            {
                ReturnToOriginalSlot(originalSlot);
                StopDragging();
                return;
            }

            // Swaps equipment items in primary and secondary equipment slots, if both are equipment items are present
            if (originalSlot.name != "InventorySlot" &&
                (newSlot.name != "InventorySlot" && newSlot.name != originalSlot.name) &&
                newSlot.item != Item.empty)
            {
                SwapEquipment();
                StopDragging();
                return;
            }

            // Returns item to original slot when the new slot is not empty
            if (newSlot.item != Item.empty)
            {
                ReturnToOriginalSlot(originalSlot);
                StopDragging();
                return;
            }

            // Sets the new slot with the original slot item and empties the original slot
            newSlot.SetItemParameters(originalSlotItem);
            originalSlot.ResetParameters();

            StopDragging();

        }

        private void StopDragging()
        {
            isDragging = false;
            originalSlot = null;
            ghostIcon.style.visibility = Visibility.Hidden;
        }

        private void ReturnToOriginalSlot(InventorySlot _originalSlot)
        {
            _originalSlot.SetSlotImage(ghostIcon.style.backgroundImage.value.sprite);
        }

        private void SwapEquipment()
        {
            Item tempPrimaryItem = primarySlot.item;
            primarySlot.SetItemParameters(secondarySlot.item);
            secondarySlot.SetItemParameters(tempPrimaryItem);
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
