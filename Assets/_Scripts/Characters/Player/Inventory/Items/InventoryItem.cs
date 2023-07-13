using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(InteractionController))]
public class InventoryItem : MonoBehaviour /*IInteraction*/
{
    /*
    [Separator("Player", true)]
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private GameObject player;

    [Separator("Inventory")]
    [SerializeField]
    private ItemObject item;
    public GameObject model;
    public Sprite itemImage;

    [SerializeField]
    private MyCode.Player.P_Inventory inventory;

    [Separator("Interaction")]
    [SerializeField]
    private InteractionController interactionController;

    private void Awake()
    {
        model = this.gameObject;
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<MyCode.Player.P_Inventory>();
        interactionController = GetComponent<InteractionController>();
    }

    public void Interact()
    {
        if (interactionController.IsInteractible())
        {
            inventory.PickUp(item, model, itemImage);
        }

    }
    */
}
