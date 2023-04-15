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

    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    private GameObject placeholderModel;

    private CharacterController ch_controller;

    private GameObject cameraObject;
    private Camera p_camera;
    

    public void Awake()
    {
        if (inventory.GetSize() <= 0)
        {
            inventory.SetSize(1);
        }

        inventory.Clear();
        inventory.Fill();

        p_input = new P_Controls();
        ch_controller = GetComponent<CharacterController>();
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        p_camera = cameraObject.GetComponent<Camera>();
        ac_selection = p_input.Player.Selectitem;
        ac_pickUp = p_input.Player.Interact;

    }

    public void SelectItem(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            if (context.control.displayName == "D-Pad Right")
            {
                selectedItem = inventory.GetNextItem(inventory.GetIndexOfItem(inventory.GetSelectedItem()));
                if (selectedItem.Key != null && selectedItem.Value != null)
                {
                    placeholderModel = selectedItem.Value;
                    print(selectedItem);
                }
            }
            else if (context.control.displayName == "D-Pad Left")
            {
                selectedItem = inventory.GetPreviousItem(inventory.GetIndexOfItem(inventory.GetSelectedItem()));
                if (selectedItem.Key != null && selectedItem.Value != null)
                {
                    placeholderModel = selectedItem.Value;
                    print(selectedItem);
                }
            }
            else
            {
                selectedItem = inventory.GetItem(int.Parse(context.control.displayName) - 1);
                if (selectedItem.Key != null && selectedItem.Value != null)
                {
                    placeholderModel = selectedItem.Value;
                    print(selectedItem);
                }
            }
        }
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            if (inventory.HasItem(selectedItem.Key))
            {
                if (Physics.Raycast(p_camera.transform.position, p_camera.transform.forward, out RaycastHit hitInfo, dropRange))
                {
                    inventory.RemoveItem(selectedItem.Key, selectedItem.Value);
                    // Vector3 positionToSpawn = transform.position + (transform.forward * 2) + (transform.up * 2);
                    Vector3 dropPosition = new Vector3(hitInfo.point.x, hitInfo.point.y + .1f, hitInfo.point.z);
                    GameObject droppedItem = Instantiate(placeholderModel, dropPosition, transform.rotation, GameObject.Find("Items").transform);
                    droppedItem.name = selectedItem.Value.name;
                    InteractionController interactionController = droppedItem.GetComponent<InteractionController>();
                    Collider itemCollider = droppedItem.GetComponent<Collider>();
                    itemCollider.enabled = true;
                    interactionController.highlightActive = false;
                    interactionController.highlight = null;
                    interactionController.highlightRenderer = null;
                    interactionController.interactible = true;
                    Rigidbody rigidbody = droppedItem.GetComponent<Rigidbody>();
                    RigidbodyConstraints constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                    rigidbody.ToggleConstraints(constraints, false);
                    droppedItem.SetActive(true);
                    Object.Destroy(selectedItem.Value);
                }
                
            }
            
        }
    }

    /// <summary>
    /// Returns inventory object that is currently used
    /// </summary>
    /// <returns>Active InventoryObject object</returns>
    public InventoryObject Get()
    {
        return inventory;
    }

    /// <summary>
    /// Sets active inventory object to specified _inventory
    /// </summary>
    /// <param name="_inventory"></param>
    public void Set(InventoryObject _inventory)
    {
        inventory = _inventory;
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
