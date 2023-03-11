using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable, IInventory
{
    public ItemObject item;
    public P_Inventory inventory;
    private GameObject body;

    public void Awake()
    {
        body = this.gameObject;
    }

    

    public void Interact()
    {
        PickUp();
    }

    public void Drop()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Instantiate(body, playerTransform.position + playerTransform.forward, transform.rotation);
    }

    public void PickUp()
    {
        if (inventory.inventory.AddItem(item))
        {
            Object.Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Already has the item");
        }
        
    }
}
