using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public enum HighlightType
{
    PickUp,
    Interact,
    Destroy
}

[RequireComponent(typeof(HighlightController))]
public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private GameObject playerBody;

    [SerializeField, Separator("Animation", true)]
    private Animator animator;
    [SerializeField]
    private string parameterName;

    [Separator("Interaction", true)]
    [SerializeField]
    private bool locked;
    [SerializeField, ConditionalField("locked")]
    private ItemObject itemForUnlock;
    public bool interactible = true;

    [Separator("Highlight", true)]
    [SerializeField]
    private HighlightController highlightController;
    // public HighlightType highlightType;
    // public GameObject highlight;
    // [HideInInspector] public TextMeshPro highlightRenderer;
    // public GameObject highlightLocation;
    // public bool highlightActive;

    [Separator("Inventory")]
    [SerializeField]
    private bool isItem;
    [SerializeField, ConditionalField("isItem")]
    private ItemObject item;
    [ConditionalField("isItem")]
    public GameObject model;
    [SerializeField, ConditionalField("isItem")]
    private P_Inventory inventory;
    
    private void Awake()
    {
        model = this.gameObject;
        // highlightActive = false;
        highlightController = GetComponent<HighlightController>();
        playerCamera = GameObject.FindGameObjectWithTag("UI Camera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
        inventory = playerBody.GetComponent<P_Inventory>();
        animator = GetComponent<Animator>();
    }

    // /// <summary>
    // /// Checks if gameObject is item. Calls PicUp() if yes, otherwise playes animation if gameObject is not locked. In that case must be unlocked by given ItemObject.
    // /// </summary>
    // public void Interact()
    // {
    //     if (interactible)
    //     {
    //         if (isItem)
    //         {
    //             inventory.PickUp(item, model);
    //         }
    //         else
    //         {
    //             if (locked)
    //             {
    //                 Debug.Log("Object locked");
    //                 locked = !inventory.inventory.HasItem(itemForUnlock);
    //             }
    //             else
    //             {   
    //                 Debug.Log("Interacted");
    //                 if (gameObject.TryGetComponent(out SubjectController subjectController))
    //                 {
    //                     Debug.Log("Is computer");
    //                 }

    //                 if (animator != null)
    //                 {
    //                     animator.SetBool(parameterName, !animator.GetBool(parameterName));
    //                 }
                    
    //             }
                
    //         }
    //     }
        
    // }
}
