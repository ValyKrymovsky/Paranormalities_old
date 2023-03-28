using UnityEngine;
using UnityEngine.InputSystem;

public class P_Inventory : MonoBehaviour
{
    public InventoryObject inventory;

    private P_Controls p_input;

    private InputAction ac_pickUp;
    private InputAction ac_selection;
    

    public void Awake()
    {
        inventory.inventorySize = inventory.GetInventorySize() <= 0 ? 1 : inventory.GetInventorySize();

        p_input = new P_Controls();
        ac_selection = p_input.Player.Selectitem;
        ac_pickUp = p_input.Player.Interact;

        p_input.Disable();
        for (int i = 1; i < inventory.GetInventorySize() + 1; i++)
        {
            ac_selection.AddBinding(string.Format("<Keyboard>/{0}", i), groups: "Keyboard&Mouse");
        }
        p_input.Enable();
        foreach (InputBinding binding in ac_selection.bindings)
        {
            print(binding);
        }

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
        float value = context.ReadValue<float>();
        if ((int)context.phase == 2)
        {
            print(value);
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
