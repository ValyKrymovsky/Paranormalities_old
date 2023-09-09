using UnityEngine;
using System;
using MyBox;
using MyCode.GameData.Inventory;
using System.Linq;


namespace MyCode.GameData.PlayerData
{
    public class PlayerInventory
    {
        [Space]
        [Separator("Inventory", true)]
        [Space]

        [Header("Inventory Object")]
        [Space]
        private InventoryItem[] _inventory;
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
        public event Action<InventoryItem, int> OnAddItem;
        public event Action<InventoryItem, SlotType> OnAddEquipment;


        // Inventory
        public InventoryItem[] Inventory { get => _inventory; set => _inventory = value; }
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

        public void MoveItem(int _oldSlot, int _newSlot)
        {
            if (_oldSlot > _inventory.Length - 1 || _newSlot > _inventory.Length - 1)
            {
                if (_oldSlot > _inventory.Length - 1)
                {
                    int oldSlotIndex = _oldSlot - (_inventory.Length - 1);
                    
                    if (oldSlotIndex == 1)
                    {
                        InventoryItem temp = _primaryEquipment;

                        _primaryEquipment = _inventory[_newSlot];
                        _inventory[_newSlot] = temp;
                    }
                    else if (oldSlotIndex == 2)
                    {
                        InventoryItem temp = _secondaryEquipment;

                        _secondaryEquipment = _inventory[_newSlot];
                        _inventory[_newSlot] = temp;
                    }

                    return;
                }

                if (_newSlot > _inventory.Length - 1)
                {
                    int newSlotIndex = _newSlot - (_inventory.Length - 1);

                    if (newSlotIndex == 1)
                    {
                        InventoryItem temp = _primaryEquipment;

                        _primaryEquipment = _inventory[_oldSlot];
                        _inventory[_oldSlot] = temp;
                    }
                    else if (newSlotIndex == 2)
                    {
                        InventoryItem temp = _secondaryEquipment;

                        _secondaryEquipment = _inventory[_oldSlot];
                        _inventory[_oldSlot] = temp;
                    }

                    return;
                }
            }

            InventoryItem originalItem = _inventory[_oldSlot];

            _inventory[_oldSlot] = _inventory[_newSlot];
            _inventory[_newSlot] = originalItem;
        }

        public void EquipmentSwap(SlotType _originalSlot)
        {
            if (_originalSlot == SlotType.Primary)
            {
                InventoryItem temp = _primaryEquipment;

                _primaryEquipment = _secondaryEquipment;
                _secondaryEquipment = temp;
                return;
            }
            else if (_originalSlot == SlotType.Secondary)
            {
                InventoryItem temp = _secondaryEquipment;

                _secondaryEquipment = _primaryEquipment;
                _primaryEquipment = temp;
            }
        }

        public bool AddItem(InventoryItem _item)
        {
            if (IsFull() == true) return false;
            if (HasItem(_item)) return false;

            if (_item.Item.itemType != ItemObject.ItemType.Equipment)
            {
                int arrayIndex = _inventory.IndexOfItem(_inventory.Where(item => item == InventoryItem.empty).First());
                _inventory[arrayIndex] = _item;
                OnAddItem?.Invoke(_item, arrayIndex);
                return true;
            }
            
            if (_primaryEquipment == InventoryItem.empty)
            {
                _primaryEquipment = _item;
                OnAddEquipment?.Invoke(_item, SlotType.Primary);
                return true;
            }
            
            if (_secondaryEquipment == InventoryItem.empty)
            {
                _secondaryEquipment = _item;
                OnAddEquipment?.Invoke(_item, SlotType.Secondary);
                return true;
            }

            return false;
        }

        public void RemoveItem(InventoryItem _item)
        {
            if (HasItem(_item))
            {
                // inventory[inventory.IndexOfItem(_item)] ;
            }
        }

        public InventoryItem GetItemAtIndex(int _index)
        {
            try
            {
                InventoryItem item = _inventory.ElementAt(_index);
                return item;
            }
            catch (ArgumentOutOfRangeException)
            {
                return new InventoryItem(0, null, null, null);
            }
        }

        public bool HasItem(InventoryItem _item)
        {
            if (_item.Item == null) return false;

            if (_inventory.Where(item => item.ItemId == _item.ItemId).Any()) return true;

            return false;
        }

        public void Clear()
        {
            Array.Clear(_inventory, 0, _inventory.Length);
        }

        public bool IsFull()
        {
            bool check = _inventory.Where(item => item != InventoryItem.empty).Count() >= _inventory.Length;
            return check;
        }
    }

}
