using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;


[CreateAssetMenu(fileName = "NewInventoryData", menuName = "DataObjects/Player/Inventory")]
public class PlayerInventoryData : ScriptableObject
{
    [Space]
    [Separator("Inventory", true)]
    [Space]

    [Header("Inventory Object")]
    [Space]
    [SerializeField] private InventoryObject inventory;

    [Space]
    [Separator("Drop", true)]
    [Space]

    [Header("Item Drop")]
    [Space]
    [SerializeField] private float dropRange;

    [Space]
    [Separator("Inputs", true)]
    [Space]

    [Header("Input Action")]
    [Space]
    [SerializeField] private InputActionReference input_DropItem;
    [SerializeField] private InputActionReference input_OpenInventory;

    private GameObject inventoryUI;
    private InventoryEventHandler inventoryEventHandler;
    private bool inventoryOpen;


    public InventoryObject Inventory { get => inventory; set => inventory = value; }
    public float DropRange { get => dropRange; set => dropRange = value; }
    public InputActionReference DropItemInput { get => input_DropItem; set => input_DropItem = value; }
    public InputActionReference OpenInventoryInput { get => input_OpenInventory; set => input_OpenInventory = value; }
    public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }
    public InventoryEventHandler InventoryEventHandler { get => inventoryEventHandler; set => inventoryEventHandler = value; }
    public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }
}
