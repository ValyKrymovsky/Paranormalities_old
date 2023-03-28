using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

interface IInventory
{
    public void PickUp();
    public void Drop();
}

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public Dictionary<ItemObject, GameObject> inventory = new Dictionary<ItemObject, GameObject>();
    public int inventorySize;
    public bool inventoryFull;
    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    public bool AddItem(ItemObject _item, GameObject model)
    {
        bool hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory.ContainsKey(_item))
            {
                hasItem = true;
                break;
            }
        }

        // IsInventoryFull();
        if (!hasItem)
        {
            inventory.Add(_item, model);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItem(ItemObject _item, GameObject _model)
    {
        bool hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory.ContainsKey(_item) && inventory[_item] == _model)
            {
                hasItem = true;
                break;
            }
        }

        if (hasItem)
        {
            inventory.Remove(_item);
        }
    }

    public int GetInventorySize()
    {
        return inventorySize;
    }

    public bool IsInventoryFull()
    {
        if (inventory.Count >= inventorySize)
        {
            inventoryFull = true;
            return true;
        }
        else
        {
            inventoryFull = false;
            return false;
        }
    }

    public void PrintInventory()
    {
        foreach (var item in inventory)
        {
            Debug.Log(string.Format("Item: {0}, model : {1}", item.Key, item.Value));
        }
    }

    public (ItemObject, GameObject) SelectItem(int _index)
    {
        // try
        // {
        //     return inventory.ElementAt(index);
        // }
        // catch (ArgumentOutOfRangeException e)
        // {
        //     return new KeyValuePair<ItemObject, GameObject>(null, null);
        // }

        // if (index > GetInventorySize())
        // {
        //     // return new KeyValuePair<ItemObject, GameObject>(null, null);
        //     return (null, null);
        // }
        // else
        // {
        //     return (inventory.ElementAt(index).Key, inventory.ElementAt(index).Value);
        // }

        try
        {
            return (inventory.ElementAt(_index).Key, inventory.ElementAt(_index).Value);
        }
        catch (ArgumentOutOfRangeException)
        {
            return (null, null);
        }  
    }

    public bool HasItem(ItemObject _item, GameObject _model)
    {
        if (_item == null || _model == null)
        {
            return false;
        }

        if (inventory.ContainsKey(_item) && inventory[_item] == _model)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
