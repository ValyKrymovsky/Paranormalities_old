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
public class InteractionController : MonoBehaviour, IInteractable, IInventory
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
    [SerializeField]
    private GameObject placeholderParent;
    [SerializeField]
    private GameObject[] itemPlaceholders;
    
    private void Awake()
    {
        model = this.gameObject;
        // highlightActive = false;
        highlightController = GetComponent<HighlightController>();
        playerCamera = GameObject.FindGameObjectWithTag("UI Camera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
        inventory = playerBody.GetComponent<P_Inventory>();
        animator = GetComponent<Animator>();

        itemPlaceholders = new GameObject[inventory.inventory.GetSize()];

        placeholderParent = GameObject.Find("Item Placeholders");

        for (int i = 0; i < placeholderParent.transform.childCount; i++)
        {
            itemPlaceholders[i] = placeholderParent.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Checks if gameObject is item. Calls PicUp() if yes, otherwise playes animation if gameObject is not locked. In that case must be unlocked by given ItemObject.
    /// </summary>
    public void Interact()
    {
        if (interactible)
        {
            if (isItem)
            {
                PickUp();
            }
            else
            {
                if (locked)
                {
                    Debug.Log("Object locked");
                    locked = !inventory.inventory.HasItem(itemForUnlock);
                }
                else
                {   
                    Debug.Log("Interacted");
                    if (gameObject.TryGetComponent(out SubjectController subjectController))
                    {
                        Debug.Log("Is computer");
                    }

                    if (animator != null)
                    {
                        animator.SetBool(parameterName, !animator.GetBool(parameterName));
                    }
                    
                }
                
            }
        }
        
    }

    /// <summary>
    /// Tries to add item to inventory.
    /// </summary>
    public void PickUp()
    {
        if (inventory.Get().AddItem(item, model))
        {
            highlightController.TurnOffHighlight();
            if (transform.childCount == 1)
            {
                Transform childHighlight = transform.Find(string.Format("{0} highlight", name));
                Object.Destroy(childHighlight.gameObject);
            }
            
            int placeholderIndex = inventory.Get().GetIndexOfItem(new KeyValuePair<ItemObject, GameObject>(item, model));

            Rigidbody itemRigidbody = GetComponent<Rigidbody>();

            itemRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            transform.parent = itemPlaceholders[placeholderIndex].transform;
            transform.position = itemPlaceholders[placeholderIndex].transform.position;

            interactible = false;
        }
        else if (!inventory.Get().AddItem(item, model) && inventory.Get().IsFull())
        {
            Debug.Log("Inventory full");
        }
        else
        {
            Debug.Log("Already has the item");
        }
    }

    /// <summary>
    /// Teleports picked up item to empty item placeholder.
    /// </summary>
    /// <returns></returns>
    private GameObject TeleportToPlaceholder()
    {
        foreach (GameObject placeholder in itemPlaceholders)
        {
            if (placeholder.transform.childCount == 0)
            {
                return placeholder;
            }
        }
        return null;
    }
}
