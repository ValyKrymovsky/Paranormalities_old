using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInventory
{
    public void PickUp();
    public void Drop();
}

[CreateAssetMenu(fileName = "NewInventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<ItemObject> inventory = new List<ItemObject>();
    public int inventorySize;
    public bool inventoryFull;
    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    public bool AddItem(ItemObject _item)
    {
        bool hasItem = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == _item)
            {
                hasItem = true;
                break;
            }
        }

        // IsInventoryFull();
        if (!hasItem)
        {
            inventory.Add(_item);
            return true;
        }
        else
        {
            return false;
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

    public void DropItem(ItemObject _item)
    {
        bool hasItem = false;
        int itemIndex = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == _item)
            {
                hasItem = true;
                itemIndex = i;
                break;
            }
        }

        if (hasItem)
        {
            inventory.RemoveAt(itemIndex);
        }
    }

    public void PrintInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            Debug.Log(inventory[i]);
        }
    }

    public int GetInventorySize()
    {
        return inventorySize;
    }

    public ItemObject SelectItem(int index)
    {
        return inventory[index];
    }
}

/**[System.Serializable]
public class InventorySlot
{
    public ItemObject item;

    public InventorySlot(ItemObject _item)
    {
        item = _item;
    }

    
}**/
