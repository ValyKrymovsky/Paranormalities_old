using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemObject item;
    public P_Inventory inventory;

    public void Interact()
    {
        inventory.inventory.AddItem(item);
        inventory.inventory.printInventory();
        Debug.Log("Interacted");
    }
}
