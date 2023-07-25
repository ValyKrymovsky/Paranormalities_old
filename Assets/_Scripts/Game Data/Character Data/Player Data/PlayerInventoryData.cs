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
        [SerializeField] private InventoryObject _inventory;
        [SerializeField] private InventoryItem _primaryEquipment;
        [SerializeField] private InventoryItem _secondaryEquipment;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        [SerializeField] private float dropRange;
        

        private GameObject inventoryUI;
        private bool inventoryOpen;

        public event Action<bool> OnInventoryStatusChange;
        public event Action<InventoryItem> OnAddItem;


        // Inventory
        public InventoryObject Inventory { get => _inventory; set => _inventory = value; }
        public InventoryItem PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public InventoryItem SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }

        // Item drop
        public float DropRange { get => dropRange; set => dropRange = value; }

        public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }

        // Inventory state
        public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }
        

        public void InvokeOnInventoryStatusChange(bool _newState)
        {
            OnInventoryStatusChange?.Invoke(_newState);
        }

        public void InvokeOnAddItem(InventoryItem _item)
        {
            OnAddItem?.Invoke(_item);
        }

    }

}
