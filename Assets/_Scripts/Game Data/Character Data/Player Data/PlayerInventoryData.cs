using UnityEngine.InputSystem;
using UnityEngine;
using System;
using MyBox;
using MyCode.Player.Inventory;


namespace MyCode.Data.Player
{
    [CreateAssetMenu(fileName = "NewInventoryData", menuName = "DataObjects/Player/Inventory")]
    public class PlayerInventoryData : ScriptableObject
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        [SerializeField] private InventoryObject inventory;
        [SerializeField] private InventoryItem _primaryEquipment;
        [SerializeField] private InventoryItem _secondaryEquipment;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        [SerializeField] private float dropRange;

        [Space]
        [Separator("Inputs", true)]
        [Space]

        [Header("Input Action")]
        [Space]
        [SerializeField] private InputActionReference input_DropItem;
        [SerializeField] private InputActionReference input_ToggleInventory;

        private GameObject inventoryUI;
        private bool inventoryOpen;

        public event Action<bool> OnInventoryStatusChange;
        public event Action<InventoryItem> OnAddItem;


        // Inventory
        public InventoryObject Inventory { get => inventory; set => inventory = value; }
        public InventoryItem PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public InventoryItem SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }

        // Item drop
        public float DropRange { get => dropRange; set => dropRange = value; }

        // Inputs
        public InputActionReference DropItemInput { get => input_DropItem; }
        public InputActionReference ToggleInventoryInput { get => input_ToggleInventory; }
        public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }

        // Inventory state
        public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }
        

        public void InvokeOnInventoryStatusChange(bool _newState)
        {
            OnInventoryStatusChange?.Invoke(_newState);
        }

        public void InvokeOnAddItem(InventoryItem _item)
        {
            Debug.Log("add item action invoked");
            OnAddItem?.Invoke(_item);
        }

        private void OnEnable()
        {
            input_DropItem.action.Enable();
            input_ToggleInventory.action.Enable();
        }

        private void OnDisable()
        {
            input_DropItem.action.Disable();
            input_ToggleInventory.action.Disable();
        }
    }

}
