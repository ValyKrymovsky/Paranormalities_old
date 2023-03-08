using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable {
    public void Interact();
}

public class P_Interactor : MonoBehaviour
{
    private Transform interactorSource;
    [SerializeField] private float range;

    private Inventory inventory;

    // Components //
    private P_Controls p_input;


    // Input Actions //
    private InputAction ac_interact;  // input action for interacting

    void Awake()
    {
        p_input = new P_Controls();
        ac_interact = p_input.Player.Interact;
        interactorSource = transform;

        inventory = new Inventory(4);
        inventory.AddItem(new Item{ itemType = Item.ItemType.Key});
    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (((int)context.phase) == 2)
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            
            if (Physics.Raycast(r, out RaycastHit hitInfo, range)) 
            {
                Debug.DrawLine(interactorSource.position, hitInfo.transform.position, Color.yellow, 5f);
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
