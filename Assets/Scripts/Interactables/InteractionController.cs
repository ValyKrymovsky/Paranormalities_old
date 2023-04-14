using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Separator("Highlight", true)]
    public GameObject highlight;
    [HideInInspector] public SpriteRenderer highlightRenderer;
    public GameObject highlightLocation;
    public bool highlightActive;

    [Separator("Inventory")]
    [SerializeField] private bool isItem;
    [SerializeField, ConditionalField("isItem")] private ItemObject item;
    [ConditionalField("isItem")]public GameObject model;
    [SerializeField, ConditionalField("isItem")] private P_Inventory inventory;
    
    private void Awake()
    {
        model = this.gameObject;
        highlightActive = false;
        playerCamera = GameObject.FindGameObjectWithTag("UI Camera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
        inventory = playerBody.GetComponent<P_Inventory>();
        animator = GetComponent<Animator>();

        if (highlightLocation == null)
        {
            highlightLocation = gameObject;
        }
    }

    public void Interact()
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
            
            this.gameObject.SetActive(false);
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
            _target.transform.localScale = new Vector3(.01f, .01f, .01f);
            highlight = Instantiate(_target, highlightLocation.transform.position, transform.rotation, gameObject.transform);
            highlightRenderer = highlight.GetComponent<SpriteRenderer>();
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
}
