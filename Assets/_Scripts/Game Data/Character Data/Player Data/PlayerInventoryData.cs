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
    [SerializeField] private InputActionReference input_ToggleInventory;

    private GameObject inventoryUI;
    private InventoryHandler inventoryHandler;
    private bool inventoryOpen;


    public InventoryObject Inventory { get => inventory; set => inventory = value; }
    public float DropRange { get => dropRange; set => dropRange = value; }
    public InputActionReference DropItemInput { get => input_DropItem; }
    public InputActionReference ToggleInventoryInput { get => input_ToggleInventory; }
    public GameObject InventoryUI { get => inventoryUI; set => inventoryUI = value; }
    public InventoryHandler InventoryEventHandler { get => inventoryHandler; set => inventoryHandler = value; }
    public bool InventoryOpen { get => inventoryOpen; set => inventoryOpen = value; }
    

    private void OnEnable()
    {
        input_DropItem.action.Enable();
        input_ToggleInventory.action.Enable();
    }

    private void OnDisable()
    {
        input_DropItem.action.Disable();
        input_ToggleInventory.action.Disable();
    }
}
