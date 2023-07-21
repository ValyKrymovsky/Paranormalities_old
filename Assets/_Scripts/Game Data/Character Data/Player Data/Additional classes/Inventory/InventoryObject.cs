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
        public List<(ItemObject item, GameObject model)> inventory = new List<(ItemObject item, GameObject model)>();
        public (ItemObject item, GameObject model) selectedItem;
        public int size = 16;
        public bool inventoryFull;

        public bool AddItem(ItemObject _item, GameObject _model)
        {
            if (IsFull() == false)
            {
                if (!HasItem(_item))
                {
                    inventory.Add((_item, _model));
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

        public void RemoveItem(ItemObject _item, GameObject _model)
        {
            if (HasItem(_item))
            {
                inventory.Remove((_item, _model));
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

        public (ItemObject item, GameObject model) GetItemAsIndex(int _index)
        {
            try
            {
                (ItemObject item, GameObject model) item = inventory.ElementAt(_index);
                return item;
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, null);
            }
        }

        public int GetIndexOfItem((ItemObject item, GameObject model) _item)
        {
            return inventory.IndexOf(_item);
        }

        /// <returns></returns>
        public (ItemObject item, GameObject model) GetSelectedItem()
        {
            return selectedItem;
        }

        public bool HasItem(ItemObject _item)
        {
            if (_item == null)
            {
                return false;
            }
            else
            {
                foreach ((ItemObject item, GameObject model) item in inventory)
                {
                    if (item.item.Equals(_item))
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
            foreach ((ItemObject item, GameObject model) item in inventory)
            {
                Debug.Log(string.Format("Id: {0}, Item: {1}, model : {2}", index, item.item, item.model));
                index++;
            }
        }
    }

}
