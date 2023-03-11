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
        p_input = new P_Controls();
        ac_selection = p_input.Player.Hotbar;

        for (int i = 1; i < inventory.GetInventorySize() + 1; i++)
        {
            ac_selection.AddBinding(string.Format("<Keyboard>/{0}", i));
        }
    }

    public void Update()
    {
        float input_value = ac_selection.ReadValue<float>();
        if (input_value != 0)
        {
            Debug.Log("Binding: " + ac_selection.activeControl.displayName + ", phase: " + ac_selection.phase);
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
