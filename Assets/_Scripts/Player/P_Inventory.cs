using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using MyBox;

public class P_Inventory : MonoBehaviour
{

    [Separator("Inventory", true)]
    public InventoryObject inventory;
    public KeyValuePair<ItemObject, GameObject> selectedItem;

    [SerializeField, Separator("Drop", true)]
    private float dropRange;

    [Separator("Inventory UI", true)]
    [SerializeField] private GameObject inventoryUI;

    private P_Controls p_input;

    private GameObject placeholderModel;

    private CharacterController ch_controller;

    private P_Movement p_movement;
    private P_Camera p_camera;

    private GameObject cameraObject;
    private Camera playerCamera;
    

    public void Awake()
    {
        if (inventory.GetSize() <= 0)
        {
            inventory.SetSize(1);
        }

        inventory.Clear();

        p_input = new P_Controls();
        ch_controller = GetComponent<CharacterController>();
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        playerCamera = cameraObject.GetComponent<Camera>();

        p_movement = GetComponent<P_Movement>();
        p_camera = cameraObject.GetComponent<P_Camera>();
    }

    /// <summary>
    /// Selects item from inventory based on pressed keyboard or controller button.
    /// </summary>
    /// <param name="context"></param>
    public void SelectItem(InputAction.CallbackContext context)
    {
        
    }

    /// <summary>
    /// Tries to add item to inventory.
    /// </summary>
    public void PickUp(ItemObject _item, GameObject _model)
    {
        
    }
    
    /// <summary>
    /// Drops item. Casts ray from playerCamera and spawns the item on hitInfo.point location.
    /// </summary>
    /// <param name="context"></param>
    public void DropItem(InputAction.CallbackContext context)
    {
        
    }

    /// <summary>
    /// Casts ray from player position down to the ground. Returns parents name to which the ground mesh belongs to.
    /// </summary>
    /// <returns>string roomName</returns>
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

    /// <summary>
    /// Returns inventory object that is currently used.
    /// </summary>
    /// <returns>Active InventoryObject object</returns>
    public InventoryObject Get()
    {
        return inventory;
    }

    /// <summary>
    /// Sets active inventory object to specified _inventory.
    /// </summary>
    /// <param name="_inventory"></param>
    public void Set(InventoryObject _inventory)
    {
        inventory = _inventory;
    }

    public void OpenInventory(InputAction.CallbackContext _context)
    {
        if (!inventoryUI.activeSelf)
        {
            p_movement.SetCanMove(false);
            p_camera.SetCanLook(false);
            inventoryUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            
        }
        else
        {
            p_movement.SetCanMove(true);
            p_camera.SetCanLook(true);
            inventoryUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            
        }
    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }
}
