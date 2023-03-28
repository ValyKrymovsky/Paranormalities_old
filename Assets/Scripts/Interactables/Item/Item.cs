using UnityEngine;

public class Item : MonoBehaviour, IInteractable, IInventory, IHighlight
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerBody;

    [Header("Object")]
    [SerializeField] private ItemObject item;
    [SerializeField] private Sprite image;
    private GameObject model;
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
        // toolbar = toolbarObject.GetComponent<Toolbar>();
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
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Instantiate(selectedItem, playerTransform.position + playerTransform.forward, transform.rotation);
    }

    public void PickUp()
    {
        if (inventory.inventory.AddItem(item))
        {
            Object.Destroy(this.gameObject);
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
            print("Created");
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
            print("Destroyed");
        }
        
    }
}
