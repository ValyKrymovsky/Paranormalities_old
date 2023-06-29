using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<(ItemObject item, GameObject model)> inventory = new List<(ItemObject item, GameObject model)>();
    public (ItemObject item, GameObject model) selectedItem;
    public int size = 15;
    public bool inventoryFull;

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
            if (!HasItem(_item))
            {
                inventory.Append((_item, _model));
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
        if (HasItem(_item))
        {
            inventory.Remove((_item, _model));
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>Inventory size</returns>
    public int GetSize()
    {
        return size;
    }

    public void SetSize(int _size)
    {
        size = _size;
    }

    /// <summary>
    /// Returns KeyValuePair from inventory at given index.
    /// </summary>
    /// <param name="_index"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns index of given KeyValuePair in inventory.
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    public int GetIndexOfItem((ItemObject item, GameObject model) _item)
    {
        return inventory.IndexOf(_item);
    }

    /// <summary>
    /// Returns current selected item.
    /// </summary>
    /// <returns></returns>
    public (ItemObject item, GameObject model) GetSelectedItem()
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
            foreach ((ItemObject item, GameObject model) item in inventory)
            {
                if (item.item.Equals(_item))
                {
                    return true;
                }
            }
            return false;
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
        if (inventory.Count >= size)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Writes to console every item in inventory.
    /// </summary>
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
