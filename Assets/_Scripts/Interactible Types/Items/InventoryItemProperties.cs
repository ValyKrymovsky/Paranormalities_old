using UnityEngine;
using MyBox;
using MyCode.Managers;
using MyCode.Player.Interaction;

namespace MyCode.Player.Inventory
{
    public class InventoryItemProperties : MonoBehaviour
    {
        [Space]
        [Separator("Inventory")]
        [Space]

        [Header("Inventory Item")]
        [Space]
        [SerializeField] private InventoryItem _item;
        private InteractionController _intController;

        private void Awake()
        {
            _intController = GetComponent<InteractionController>();
        }

        private void Start()
        {
            _item.Model = gameObject;

            _item.ItemId = _item.ItemId == 0 ? UnityEngine.Random.Range(1000, 10000) : _item.ItemId;
        }

        private void OnEnable()
        {
            _intController.OnInteracted += AddToInventory;
        }

        private void OnDisable()
        {
            _intController.OnInteracted -= AddToInventory;
        }

        public void AddToInventory()
        {
            if (PlayerManager.Instance.InventoryData.Inventory.AddItem(_item))
            {
                Debug.Log("Added item to inventory");
                PlayerManager.Instance.InventoryData.InvokeOnAddItem(_item);
                Debug.Log("Invoked add to inventory action");

                this.gameObject.SetActive(false);

                
            }
        }

    }

}
