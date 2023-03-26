using UnityEngine;

public class Item : MonoBehaviour, IInteractable, IInventory, IPopUp
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
    [SerializeField] private GameObject popup;
    private SpriteRenderer popupRenderer;
    public bool popupActive;
    public float popupHideDistance;

    public void Awake()
    {
        model = this.gameObject;
        toolbarObject = GameObject.Find("Inventory Toolbar");
        // toolbar = toolbarObject.GetComponent<Toolbar>();
        popupActive = false;
        playerCamera = GameObject.FindGameObjectWithTag("UI Camera");
        playerBody = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        float distance = Vector3.Distance(this.transform.position, playerBody.transform.position);
        print("Player distance from item: " + distance);
        if (popup != null)
        {
            if (distance > popupHideDistance)
            {
                if (popupActive)
                {
                    DestroyPopup();
                }
            }
            else
            {
                popup.transform.LookAt(playerCamera.transform);
                popupRenderer.color = new Color(255, 255, 255, Mathf.InverseLerp(popupHideDistance, 0, distance));
            }
        }
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
        if (!popupActive)
        {
            popup = Instantiate(target, this.gameObject.transform);
            popupRenderer = popup.GetComponent<SpriteRenderer>();
            popup.name = string.Format("{0} popup", name);
            popupActive = true;
            print("Succ!");
        }
        
    }

    public void DestroyPopup()
    {
        if (popupActive)
        {
            Object.Destroy(popup);
            popup = null;
            popupRenderer = null;
            popupActive = false;
        }
        
    }
}
