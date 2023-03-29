using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

public class P_Inventory : MonoBehaviour
{

    [Header("Inventory")]
    public InventoryObject inventory;
    public (ItemObject, GameObject) selectedItem;

    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;

    private GameObject placeholderModel;
    

    public void Awake()
    {
        inventory.inventorySize = inventory.GetInventorySize() <= 0 ? 1 : inventory.GetInventorySize();
        inventory.inventory.Clear();

        p_input = new P_Controls();
        ac_selection = p_input.Player.Selectitem;
        ac_pickUp = p_input.Player.Interact;

    }

    public void Update()
    {
    }

    public void SelectItem(InputAction.CallbackContext context)
    {
        // selectedItem = inventory.inventory.ElementAt(int.Parse(context.control.displayName) - 1);
        // print(int.Parse(context.control.displayName) - 1);
        // print(inventory.inventory.ElementAt(int.Parse(context.control.displayName) - 1));
        if ((int)context.phase == 2)
        {
            selectedItem = inventory.SelectItem(int.Parse(context.control.displayName) - 1);
            if (selectedItem.Item1 != null && selectedItem.Item2 != null)
            {
                placeholderModel = selectedItem.Item2;
                print(selectedItem);
            }
            
        }
        
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            if (inventory.HasItem(selectedItem.Item1, selectedItem.Item2))
            {
                inventory.RemoveItem(selectedItem.Item1, selectedItem.Item2);
                Vector3 positionToSpawn = transform.position + (transform.forward * (2));
                GameObject droppedItem = Instantiate(placeholderModel, positionToSpawn, transform.rotation, GameObject.Find("Items").transform);
                droppedItem.name = selectedItem.Item2.name;
                droppedItem.GetComponent<Item>().highlightActive = false;
                droppedItem.GetComponent<Item>().highlight = null;
                droppedItem.GetComponent<Item>().highlightRenderer = null;
                droppedItem.SetActive(true);
                Object.Destroy(selectedItem.Item2);
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
