using UnityEngine;
using System;
using MyBox;
using MyCode.GameData.Inventory;


namespace MyCode.GameData.PlayerData
{
    public class PlayerInventory
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        private InventoryObject _inventory;
        private InventoryItem _primaryEquipment;
        private InventoryItem _secondaryEquipment;

        [Space]
        [Separator("Drop", true)]
        [Space]

        [Header("Item Drop")]
        [Space]
        private float _dropRange;

        private bool _inventoryOpen;

        public event Action<bool> OnInventoryStatusChange;
        public event Action<InventoryItem> OnAddItem;


        // Inventory
        public InventoryObject Inventory { get => _inventory; set => _inventory = value; }
        public InventoryItem PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public InventoryItem SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }

        // Item drop
        public float DropRange { get => _dropRange; set => _dropRange = value; }

        // Inventory state
        public bool InventoryOpen { get => _inventoryOpen; set => _inventoryOpen = value; }


        public PlayerInventory(PlayerInventoryData _data)
        {
            _inventory = _data.Inventory;
            _primaryEquipment = _data.PrimaryEquipment;
            _secondaryEquipment = _data.SecondaryEquipment;

            _dropRange = _data.DropRange;

            _inventoryOpen = _data.InventoryOpen;
        }

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
