using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> inventory;
    private int inventorySize;
    private bool inventoryFull;

    public Inventory(int size)
    {
        inventory = new List<Item>();
        inventorySize = size;
        inventoryFull = false;
    }

    public void AddItem(Item item)
    {
        if (inventory.Count >= inventorySize)
        {
            inventoryFull = true;
            Debug.Log("Inventory full!");
        }
        else
        {
            inventory.Add(item);
            Debug.Log("Item added to inventory");
            if (inventory.Count >= inventorySize)
            {
               inventoryFull = true; 
               Debug.Log("Inventory full!");
            }
            else
            {
                inventoryFull = false; 
            }
        }
        
    }

    public int inventoryCount()
    {
        return inventory.Count;
    }

    public void RemoveItem(Item item)
    {
        
    }
}
