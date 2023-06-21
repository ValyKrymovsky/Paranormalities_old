using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

public class PickupItem : MonoBehaviour
{
    [Separator("Player", true)]
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private GameObject playerBody;

    [Separator("Inventory")]
    [SerializeField]
    private bool isItem;
    [SerializeField, ConditionalField("isItem")]
    private ItemObject item;
    [ConditionalField("isItem")]
    public GameObject model;
    [SerializeField, ConditionalField("isItem")]
    private P_Inventory inventory;

    [Separator("Interaction")]
    [SerializeField]
    private InteractionController interactionController;

    private void Awake()
    {
        model = this.gameObject;
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
        inventory = playerBody.GetComponent<P_Inventory>();
        interactionController = GetComponent<InteractionController>();
    }

    /// <summary>
    /// Checks if gameObject is item. Calls PicUp() if yes.
    /// </summary>
    public void Interact()
    {
        if (interactionController.IsInteractible())
        {
            if (isItem)
            {
                inventory.PickUp(item, model);
            }
        }
        
    }
}
