using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class InteractionController : MonoBehaviour, IInteractable, IInventory, IHighlight
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
    public GameObject highlight;
    [HideInInspector] public TextMeshPro highlightRenderer;
    public GameObject highlightLocation;
    public bool highlightActive;

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
        highlightActive = false;
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

        if (highlightLocation == null)
        {
            highlightLocation = gameObject;
        }
    }

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
                    animator.SetBool(parameterName, !animator.GetBool(parameterName));
                }
                
            }
        }
        
    }

    public void PickUp()
    {
        if (inventory.Get().AddItem(item, model))
        {
            highlight = null;
            highlightRenderer = null;
            highlightActive = false;
            if (transform.childCount == 1)
            {
                Transform childHighlight = transform.Find(string.Format("{0} highlight", name));
                Object.Destroy(childHighlight.gameObject);
            }
            
            int placeholderIndex = inventory.Get().GetIndexOfItem(new KeyValuePair<ItemObject, GameObject>(item, model));
            // this.gameObject.SetActive(false);

            Rigidbody itemRigidbody = GetComponent<Rigidbody>();

            itemRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            transform.parent = itemPlaceholders[placeholderIndex].transform;
            transform.position = itemPlaceholders[placeholderIndex].transform.position;
            
            // Collider itemCollider = GetComponent<Collider>();
            // itemCollider.enabled = false;

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

    public void SpawnHighlight(GameObject _target, string _name)
    {
        if (!highlightActive)
        {
            // _target.transform.localScale = new Vector3(_target.transform.localScale.x / gameObject.transform.localScale.x, _target.transform.localScale.y / gameObject.transform.localScale.y, _target.transform.localScale.z / gameObject.transform.localScale.z);
            _target.transform.localScale = new Vector3(.25f, .25f, .25f);
            highlight = Instantiate(_target, highlightLocation.transform.position, transform.rotation, gameObject.transform);
            highlightRenderer = highlight.GetComponentInChildren<TextMeshPro>();
            highlight.name = string.Format("{0} highlight", name);
            highlightActive = true;
        }
    }

    public void DestroyHighlight()
    {
        if (highlightActive)
        {
            Object.Destroy(highlight);
            highlight = null;
            highlightRenderer = null;
            highlightActive = false;
        }
    }

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
