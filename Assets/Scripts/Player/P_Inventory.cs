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
        /*float input_value = ac_selection.ReadValue<float>();
        if (input_value != 0)
        {
            Debug.Log("Binding: " + ac_selection.activeControl.displayName + ", phase: " + ac_selection.phase);
        }*/
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
                GameObject droppedItem = Instantiate(selectedItem.Item2, positionToSpawn, transform.rotation, GameObject.Find("Items").transform);
                droppedItem.name = selectedItem.Item2.name;
                droppedItem.GetComponent<Item>().highlightActive = false;
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
