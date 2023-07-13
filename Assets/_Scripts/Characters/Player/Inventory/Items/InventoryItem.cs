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

    [Separator("Interaction")]
    [SerializeField]
    private InteractionController _interactionController;

    public static event Action<ItemObject, GameObject, Sprite> AddedItem;

    private void Awake()
    {
        model = this.gameObject;
        _interactionController = GetComponent<InteractionController>();
    }

    private void OnEnable()
    {
        _interactionController.OnInteract += AddToInventory;
    }

    private void OnDisable()
    {
        _interactionController.OnInteract -= AddToInventory;
    }

    public void AddToInventory()
    {
        if (PlayerManager.Instance.InventoryData.Inventory.AddItem(_item, model))
        {
            AddedItem?.Invoke(_item, model, itemImage);
        }
    }
    
}
