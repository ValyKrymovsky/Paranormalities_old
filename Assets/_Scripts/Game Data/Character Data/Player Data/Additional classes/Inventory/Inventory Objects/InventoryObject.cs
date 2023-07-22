using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

namespace MyCode.Player.Inventory
{
    [CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public List<InventoryItem> inventory = new List<InventoryItem>();
        public int size = 16;
        public bool inventoryFull;

        public event Action<InventoryItem> OnAddEquipment;

        public bool AddItem(InventoryItem _item)
        {
            if (IsFull() == false)
            {
                if (!HasItem(_item))
                {
                    inventory.Add(_item);

                    if (_item.Item.itemType == ItemObject.ItemType.Equipment) OnAddEquipment?.Invoke(_item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void RemoveItem(InventoryItem _item)
        {
            if (HasItem(_item))
            {
                inventory.Remove(_item);
            }
        }

        public int GetSize()
        {
            return size;
        }

        public void SetSize(int _size)
        {
            size = _size;
        }

        public InventoryItem GetItemAsIndex(int _index)
        {
            try
            {
                InventoryItem item = inventory.ElementAt(_index);
                return item;
            }
            catch (ArgumentOutOfRangeException)
            {
                return new InventoryItem(null, null, null);
            }
        }

        public int GetIndexOfItem(InventoryItem _item)
        {
            return inventory.IndexOf(_item);
        }

        public bool HasItem(InventoryItem _item)
        {
            if (_item.Item == null)
            {
                return false;
            }
            else
            {
                foreach (InventoryItem item in inventory)
                {
                    if (item.Item.Equals(_item))
                    {
                        return true;
                    }
                    continue;
                }
                return false;
            }
        }

        public void Clear()
        {
            inventory.Clear();
        }

        public bool IsFull()
        {
            if (inventory.Count >= size)
            {
                return true;
            }

            return false;
        }

        public void PrintInventory()
        {
            int index = 0;
            foreach (InventoryItem item in inventory)
            {
                Debug.Log(string.Format("Id: {0}, Item: {1}, model : {2}", index, item.Item, item.Model));
                index++;
            }
        }
    }

}
