using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

public class P_Inventory : MonoBehaviour
{

    [Header("Inventory")]
    public InventoryObject inventory;
    public KeyValuePair<ItemObject, GameObject> selectedItem;
    //public static KeyValuePair<ItemObject, GameObject> nullItem = new KeyValuePair<ItemObject, GameObject>(null, null);

    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    private GameObject placeholderModel;
    

    public void Awake()
    {
        inventory.inventorySize = inventory.GetInventorySize() <= 0 ? 1 : inventory.GetInventorySize();
        inventory.ClearInventory();

        inventory.SetUpInventory();

        inventory.PrintInventory();

        p_input = new P_Controls();
        ac_selection = p_input.Player.Selectitem;
        ac_pickUp = p_input.Player.Interact;

    }

    public void Update()
    {
    }

    public void SelectItem(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            selectedItem = inventory.SelectItem(int.Parse(context.control.displayName) - 1);
            if (selectedItem.Key != null && selectedItem.Value != null)
            {
                placeholderModel = selectedItem.Value;
                print(selectedItem);
            }
            
        }
        
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            if (inventory.HasItem(selectedItem.Key, selectedItem.Value))
            {
                inventory.RemoveItem(selectedItem.Key, selectedItem.Value);
                Vector3 positionToSpawn = transform.position + (transform.forward * (2));
                /*print(placeholderModel);
                inventory.PrintInventory();*/
                GameObject droppedItem = Instantiate(placeholderModel, positionToSpawn, transform.rotation, GameObject.Find("Items").transform);
                droppedItem.name = selectedItem.Value.name;
                droppedItem.GetComponent<Item>().highlightActive = false;
                droppedItem.GetComponent<Item>().highlight = null;
                droppedItem.GetComponent<Item>().highlightRenderer = null;
                droppedItem.SetActive(true);
                Object.Destroy(selectedItem.Value);
            }
            
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
