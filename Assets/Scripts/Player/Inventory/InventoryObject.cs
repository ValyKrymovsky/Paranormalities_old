using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

interface IInventory
{
    public void PickUp();
}

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<KeyValuePair<ItemObject, GameObject>> inventory = new List<KeyValuePair<ItemObject, GameObject>>();
    public KeyValuePair<ItemObject, GameObject> selectedItem = new KeyValuePair<ItemObject, GameObject>();
    public static KeyValuePair<ItemObject, GameObject> nullItem = new KeyValuePair<ItemObject, GameObject>(null, null);
    public int size;
    public bool inventoryFull;
    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    /// <summary>
    /// Checks if ItemObject is in inventory. Adds the item with mesh model if not.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_model"></param>
    /// <returns>True if item was successfully added</returns>
    public bool AddItem(ItemObject _item, GameObject _model)
    {
        if (IsFull() == false)
        {
            bool hasItem = false;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].Key == _item)
                {
                    hasItem = true;
                    break;
                }
            }

            if (!hasItem)
            {
                var itemToAdd = new KeyValuePair<ItemObject, GameObject>(_item, _model);
                int itemToReplace = inventory.IndexOf(inventory.Where(index => index.Equals(nullItem)).First());
                inventory[itemToReplace] = itemToAdd;
                PrintInventory();
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

    /// <summary>
    /// Checks if ItemObject is in inventory. Removes the item with mesh model if yes.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_model"></param>
    public void RemoveItem(ItemObject _item, GameObject _model)
    {
        bool hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].Key == _item && inventory[i].Value == _model)
            {
                hasItem = true;
                break;
            }
        }

        if (hasItem)
        {
            var item = new KeyValuePair<ItemObject, GameObject>(_item, _model);
            var temp2 = new KeyValuePair<ItemObject, GameObject>(null, null);
            int itemIndex = inventory.IndexOf(item);
            inventory.RemoveAt(itemIndex);
            inventory.Insert(itemIndex, temp2);
            PrintInventory();
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>Inventory size</returns>
    public int GetSize()
    {
        return size;
    }

    /// <summary>
    /// Sets inventory size to _size.
    /// </summary>
    /// <param name="_size"></param>
    public void SetSize(int _size)
    {
        size = _size;
    }

    /// <summary>
    /// Returns KeyValuePair from inventory at given index.
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public KeyValuePair<ItemObject, GameObject> GetItem(int _index)
    {
        try
        {
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(_index).Key, inventory.ElementAt(_index).Value);
            selectedItem = item;
            return item;
        }
        catch (ArgumentOutOfRangeException)
        {
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(null, null);
            return item;
        }  
    }

    /// <summary>
    /// Returns next KeyValuePair from inventory at given index.
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public KeyValuePair<ItemObject, GameObject> GetNextItem(int _index)
    {
        try
        {
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(_index + 1).Key, inventory.ElementAt(_index + 1).Value);
            selectedItem = item;
            return item;
        }
        catch (ArgumentOutOfRangeException)
        {
            //KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(null, null);
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(0).Key, inventory.ElementAt(0).Value);
            selectedItem = item;
            return item;
        }
    }

    /// <summary>
    /// Returns previout KeyPairValue from inventory at given
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
    public KeyValuePair<ItemObject, GameObject> GetPreviousItem(int _index)
    {
        try
        {
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(_index - 1).Key, inventory.ElementAt(_index - 1).Value);
            selectedItem = item;
            return item;
        }
        catch (ArgumentOutOfRangeException)
        {
            //KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(null, null);
            KeyValuePair<ItemObject, GameObject> item = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(GetSize() - 1).Key, inventory.ElementAt(GetSize() - 1).Value);
            selectedItem = item;
            return item;
        }
    }

    /// <summary>
    /// Returns index of given KeyValuePair in inventory.
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public int GetIndexOfItem(KeyValuePair<ItemObject, GameObject> _item)
    {
        return inventory.IndexOf(_item);
    }

    /// <summary>
    /// Returns current selected item.
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<ItemObject, GameObject> GetSelectedItem()
    {
        return selectedItem;
    }

    /// <summary>
    /// Check if given ItemObject is in inventory. Returns true if inventory contains the item.
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public bool HasItem(ItemObject _item)
    {
        if (_item == null)
        {
            return false;
        }
        else
        {
            foreach (var item in inventory)
            {
                if (item.Key == _item)
                {
                    return true;
                }
            }
            return false;
        }      
    }

    /// <summary>
    /// Fills inventory with null items
    /// </summary>
    public void Fill()
    {
        for (int i = 0; i < GetSize(); i++)
        {
            inventory.Add(nullItem);
        }
    }

    /// <summary>
    /// Deletes all items from inventory
    /// </summary>
    public void Clear()
    {
        inventory.Clear();
    }

    /// <summary>
    /// Checks if inventory is full.
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        int nullItems = 0;
        for (int i = 0; i < GetSize(); i++)
        {
            if (inventory[i].Key == null && inventory[i].Value == null)
            {
                nullItems += 1;
                continue;
            }
            else
            {
                continue;
            }
        }

        if (nullItems == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Writes to console every item in inventory.
    /// </summary>
    public void PrintInventory()
    {
        int index = 0;
        foreach (var item in inventory)
        {
            Debug.Log(string.Format("Id: {0}, Item: {1}, model : {2}", index, item.Key, item.Value));
            index++;
        }
    }
}
