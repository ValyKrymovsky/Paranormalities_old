using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;

public class P_Inventory : MonoBehaviour
{

    [Separator("Inventory", true)]
    public InventoryObject inventory;
    public (ItemObject item, GameObject model) selectedItem;

    [SerializeField, Separator("Drop", true)]
    private float dropRange;

    [Separator("Inventory UI", true)]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField, ReadOnly] private int slotCount;
    private bool inventoryOpen;

    [Space]
    [Header("Visual Eelements")]
    public VisualElement root;
    public VisualElement inventoryGrid;
    public List<InventorySlot> inventorySlots;

    private P_Controls p_input;

    private GameObject placeholderModel;

    private CharacterController ch_controller;

    private P_Movement p_movement;
    private P_Camera p_camera;

    private GameObject cameraObject;
    private Camera playerCamera;
    

    public void Awake()
    {
        //                 //
        // Inventory stuff //
        //                 //
        if (inventory.GetSize() <= 0)
        {
            inventory.SetSize(1);
        }
        inventory.Clear();

        //            //
        // Components //
        //            //
        p_input = new P_Controls();
        ch_controller = GetComponent<CharacterController>();
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        playerCamera = cameraObject.GetComponent<Camera>();

        p_movement = GetComponent<P_Movement>();
        p_camera = cameraObject.GetComponent<P_Camera>();

        //                    //
        // Inventory UI stuff //
        //                    //

        root = inventoryUI.GetComponent<UIDocument>().rootVisualElement;
        inventoryGrid = root.Q<VisualElement>("InventoryGrid");

        inventorySlots = new List<InventorySlot>();
        foreach(InventorySlot inventorySlot in inventoryGrid.Children().Where(slot => slot.GetType().Equals(typeof(InventorySlot))))
        {
            inventorySlots.Add(inventorySlot);
        }
        slotCount = inventoryGrid.childCount;

        root.style.display = DisplayStyle.None;

    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }

    public InventoryObject Inventory { get; set; }

    public void PickUp(ItemObject _item, GameObject _model, Sprite _itemImage)
    {
        if (!inventoryOpen)
        {
            if (!(_item, _model).Equals((null, null)))
            {
                if (inventory.AddItem(_item, _model))
                {
                    InventorySlot tmp = GetFirstEmptySlot();

                    tmp.SetItemParameters(_item, _model, _itemImage);
                }
                else
                {
                    Debug.Log("Item already in inventory");
                }
                
            }
        }
    }
    
    public void DropItem(InputAction.CallbackContext context)
    {
        
    }

    private string GetCurrentRoomName()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);
        string roomName = null;
        if (Physics.Raycast(ray,  out RaycastHit hitInfo, ch_controller.height / 2))
        {
            if (hitInfo.collider.TryGetComponent(out SurfaceMaterialData surfaceMaterialData))
            {
                roomName = hitInfo.transform.parent.name;
            }
        }

        return roomName;
    }

    //              //
    //              //
    //              //
    //              //
    // Inventory UI //
    //              //
    //              //
    //              //
    //              //

    public void OpenInventory(InputAction.CallbackContext _context)
    {
        if (root.style.display == DisplayStyle.None)
        {
            inventoryOpen = true;
            p_movement.SetCanMove(false);
            p_camera.SetCanLook(false);
            root.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            
        }
        else
        {
            inventoryOpen = false;
            p_movement.SetCanMove(true);
            p_camera.SetCanLook(true);
            root.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            
        }
    }

    public InventorySlot GetFirstEmptySlot()
    {
        foreach(InventorySlot inventorySlot in inventorySlots)
        {
            if (inventorySlot.item.Equals((null, null)))
            {
                return inventorySlot;
            }
            continue;
        }
        return null;
    }
}
