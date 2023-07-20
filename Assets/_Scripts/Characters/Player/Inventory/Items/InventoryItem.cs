using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;
using MyCode.Player;
using System;

[RequireComponent(typeof(InteractionController))]
public class InventoryItem : MonoBehaviour
{
    [Space]
    [Separator("Inventory")]
    [Space]

    [Header("Inventory Item")]
    [Space]
    [SerializeField] private ItemObject _item;
    public GameObject model;
    public Sprite itemImage;

    private void Awake()
    {
        model = this.gameObject;
    }

    private void OnEnable()
    {
        PlayerManager.Instance.InteractionData.OnInteract += AddToInventory;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InteractionData.OnInteract -= AddToInventory;
    }

    public void AddToInventory()
    {
        if (PlayerManager.Instance.InventoryData.Inventory.AddItem(_item, model))
        {
            PlayerManager.Instance.InventoryData.InvokeOnAddItem(_item, model, itemImage);
        }
    }
    
}
