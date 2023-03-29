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
    //public Dictionary<ItemObject, GameObject> inventory = new Dictionary<ItemObject, GameObject>();
    public List<KeyValuePair<ItemObject, GameObject>> inventory = new List<KeyValuePair<ItemObject, GameObject>>();
    public static KeyValuePair<ItemObject, GameObject> nullItem = new KeyValuePair<ItemObject, GameObject>(null, null);
    public int inventorySize;
    public bool inventoryFull;
    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    public bool AddItem(ItemObject _item, GameObject _model)
    {
        if (!IsInventoryFull())
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

            // IsInventoryFull();
            if (!hasItem)
            {
                var itemToAdd = new KeyValuePair<ItemObject, GameObject>(_item, _model);
                //int indexForReplace = 0;
                int itemToReplace = inventory.IndexOf(inventory.Where(index => index.Equals(nullItem)).First());
                inventory[itemToReplace] = itemToAdd;
                //inventory.Add(itemToAdd);
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
            //int itemIndex = inventory.Keys.ToList().IndexOf(_item);
            var temp = new KeyValuePair<ItemObject, GameObject>(_item, _model);
            var temp2 = new KeyValuePair<ItemObject, GameObject>(null, null);
            int itemIndex = inventory.IndexOf(temp);
            inventory.RemoveAt(itemIndex);
            inventory.Insert(itemIndex, temp2);
            PrintInventory();
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
        int index = 0;
        foreach (var item in inventory)
        {
            Debug.Log(string.Format("Id: {0}, Item: {1}, model : {2}", index, item.Key, item.Value));
            index++;
        }
    }

    public KeyValuePair<ItemObject, GameObject> SelectItem(int _index)
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
            KeyValuePair<ItemObject, GameObject> temp = new KeyValuePair<ItemObject, GameObject>(inventory.ElementAt(_index).Key, inventory.ElementAt(_index).Value);
            return temp;
        }
        catch (ArgumentOutOfRangeException)
        {
            KeyValuePair<ItemObject, GameObject> temp = new KeyValuePair<ItemObject, GameObject>(null, null);
            return temp;
        }  
    }

    public bool HasItem(ItemObject _item, GameObject _model)
    {
        if (_item == null && _model == null)
        {
            return false;
        }
        else
        {
            foreach (var item in inventory)
            {
                if (item.Key == _item && item.Value == _model)
                {
                    return true;
                }
            }
            return false;
        }      
    }

    public void SetUpInventory()
    {
        for (int i = 0; i < GetInventorySize(); i++)
        {
            inventory.Add(nullItem);
        }
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }
}
