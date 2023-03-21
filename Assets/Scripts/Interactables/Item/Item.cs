using UnityEngine;

public class Item : MonoBehaviour, IInteractable, IInventory
{
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
    
    

    public void Awake()
    {
        model = this.gameObject;
        toolbarObject = GameObject.Find("Inventory Toolbar");
        toolbar = toolbarObject.GetComponent<Toolbar>();
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
}
