using MyBox;
using System;
using System.Linq;

namespace MyCode.GameData
{
    public enum SlotType
    {
        Normal, Primary, Secondary
    }

    public class Inventory
    {
        private InventoryItem[] _inventory;
        private InventoryItem _primaryEquipment;
        private InventoryItem _secondaryEquipment;

        public event Action<InventoryItem, SlotType> OnAddItem;

        public Inventory(int _inventorySize)
        {
            _inventory = new InventoryItem[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                _inventory[i] = InventoryItem.empty;
            }
            _primaryEquipment = InventoryItem.empty;
            _secondaryEquipment = InventoryItem.empty;
        }

        public bool IsFull()
        {
            return _inventory.Where(item => item != InventoryItem.empty).Count() >= _inventory.Length;
        }

        public bool Contains(InventoryItem item)
        {
            if (item == InventoryItem.empty) return false;

            if (_inventory.Where(i => i.ItemId == item.ItemId).Any()) return true;

            return false;
        }

        public bool AddItem(InventoryItem item)
        {
            if (item == null || IsFull() || Contains(item)) return false;

            if (item.Item.itemType == Item.ItemType.Equipment)
            {
                if (_primaryEquipment == InventoryItem.empty)
                {
                    _primaryEquipment = item;
                    OnAddItem?.Invoke(item, SlotType.Primary);
                    return true;
                }
                else if (_secondaryEquipment == InventoryItem.empty)
                {
                    _secondaryEquipment = item;
                    OnAddItem?.Invoke(item, SlotType.Secondary);
                    return true;
                }
            }

            int arrayIndex = _inventory.IndexOfItem(_inventory.Where(i => i == InventoryItem.empty).First());
            _inventory[arrayIndex] = item;
            OnAddItem?.Invoke(item, SlotType.Normal);
            return true;
        }

        public void RemoveItem(InventoryItem item)
        {
            if (Contains(item))
            {
                int itemIndex = _inventory.IndexOfItem(_inventory.Where(i => i.ItemId == item.ItemId).First());
                _inventory[itemIndex] = InventoryItem.empty;
            }
        }
    }
}

