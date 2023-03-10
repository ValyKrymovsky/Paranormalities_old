using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> inventory = new List<InventorySlot>();
    public int inventorySize;
    public bool inventoryFull;

    public void AddItem(ItemObject _item)
    {
        bool hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == _item)
            {
                hasItem = true;
                break;
            }
        }

        // IsInventoryFull();
        if (!hasItem)
        {
            inventory.Add(new InventorySlot(_item));
        }
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

    public void printInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            Debug.Log(inventory[i].item);
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;

    public InventorySlot(ItemObject _item)
    {
        item = _item;
    }

    
}
