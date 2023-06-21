using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Door : MonoBehaviour
{
    private GameObject playerBody;

    [Separator("Interaction")]
    [SerializeField]
    private InteractionController interactionController;
    [SerializeField]
    private bool locked;
    [SerializeField, ConditionalField("locked")]
    private ItemObject itemForUnlock;
    private P_Inventory inventory;

    [SerializeField, Separator("Animation", true)]
    private Animator animator;
    [SerializeField]
    private string parameterName;
    

    private void Awake()
    {
        interactionController = GetComponent<InteractionController>();
        playerBody = GameObject.FindGameObjectWithTag("Player");
        inventory = playerBody.GetComponent<P_Inventory>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Checks if gameObject is item. Calls PicUp() if yes, otherwise playes animation if gameObject is not locked. In that case must be unlocked by given ItemObject.
    /// </summary>
    public void Interact()
    {
        if (interactionController.IsInteractible())
        {
            if (locked)
            {
                Debug.Log("Object locked");
                locked = !inventory.inventory.HasItem(itemForUnlock);
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool(parameterName, !animator.GetBool(parameterName));
                }
                
            }
                
        }
        
    }
}
