using MyBox;
using System;
using System.Linq;
using UnityEngine;

namespace MyCode.GameData
{
    public enum SlotType
    {
        Normal, Primary, Secondary
    }

    [Serializable]
    public class Inventory
    {
        [SerializeField] private Item[] _inventory;
        [SerializeField] private Item _primaryEquipment;
        [SerializeField] private Item _secondaryEquipment;

        public Item[] InventoryArray { get => _inventory; set => _inventory = value; }
        public Item PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public Item SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }

        public event Action<Item, SlotType> OnAddItem;

        public Inventory(int _inventorySize)
        {
            _inventory = new Item[_inventorySize];
            for (int i = 0; i < _inventorySize; i++)
            {
                _inventory[i] = Item.empty;
            }
            _primaryEquipment = Item.empty;
            _secondaryEquipment = Item.empty;
        }

        public bool IsFull()
        {
            return _inventory.Where(item => item != Item.empty).Count() >= _inventory.Length;
        }

        public int Size()
        {
            return _inventory.Length;
        }

        public bool Contains(Item item)
        {
            if (item == Item.empty) return false;

            if (_inventory.Contains(item)) return true;

            return false;
        }

        public bool AddItem(Item item)
        {
            if (item == null || IsFull() || Contains(item)) return false;

            if (item.type == ItemType.Equipment)
            {
                if (_primaryEquipment == Item.empty)
                {
                    _primaryEquipment = item;
                    OnAddItem?.Invoke(item, SlotType.Primary);
                    return true;
                }
                else if (_secondaryEquipment == Item.empty)
                {
                    _secondaryEquipment = item;
                    OnAddItem?.Invoke(item, SlotType.Secondary);
                    return true;
                }
            }

            int arrayIndex = _inventory.IndexOfItem(_inventory.Where(i => i == Item.empty).First());
            _inventory[arrayIndex] = item;
            OnAddItem?.Invoke(item, SlotType.Normal);
            return true;
        }

        public void FillWithNull()
        {
            for(int i = 0; i < _inventory.Length; i++)
            {
                _inventory[i] = Item.empty;
            }

            _primaryEquipment = Item.empty;
            _secondaryEquipment = Item.empty;
        }

        public void RemoveItem(Item item)
        {
            if (Contains(item))
            {
                int itemIndex = _inventory.IndexOfItem(_inventory.Where(i => i.id == item.id).First());
                _inventory[itemIndex] = Item.empty;
            }
        }
    }
}

