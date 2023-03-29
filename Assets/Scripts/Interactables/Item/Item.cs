using UnityEngine;
using UnityEditor;


public class Item : MonoBehaviour, IInteractable, IInventory, IHighlight
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerBody;

    [Header("Object")]
    [SerializeField] private ItemObject item;
    [SerializeField] private Sprite image;
    public GameObject model;
    private GameObject selectedItem;

    [Header("Inventory")]
    [SerializeField] private P_Inventory inventory;

    [Header("Toolbar")]
    [SerializeField] private GameObject toolbarObject;
    [SerializeField] private Toolbar toolbar;

    [Header("Popup")]
    public GameObject highlight;
    [HideInInspector] public SpriteRenderer highlightRenderer;
    public bool highlightActive;

    public void Awake()
    {
        model = this.gameObject;
        toolbarObject = GameObject.Find("Inventory Toolbar");
        highlightActive = false;
        playerCamera = GameObject.FindGameObjectWithTag("UI Camera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        PickUp();
    }

    public void Drop()
    {
    }

    public void PickUp()
    {
        if (inventory.inventory.AddItem(item, model))
        {
            highlight = null;
            highlightRenderer = null;
            highlightActive = false;
            Transform childHighlight = transform.Find("pick up highlight");
            if (transform != null)
            {
                Object.Destroy(childHighlight.gameObject);
            }
            
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Already has the item");
        }
        
    }

    public void InstantiatePopup(GameObject target, string name)
    {
        if (!highlightActive)
        {
            highlight = Instantiate(target, this.gameObject.transform);
            highlightRenderer = highlight.GetComponent<SpriteRenderer>();
            highlight.name = string.Format("{0} highlight", name);
            highlightActive = true;
        }
        
    }

    public void DestroyPopup()
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
