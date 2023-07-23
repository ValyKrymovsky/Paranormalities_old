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
        /*[SerializeField] private ItemObject _item;
        public GameObject model;
        public Sprite itemImage;
        */
        private InteractionController _intController;

        private void Awake()
        {
            _intController = GetComponent<InteractionController>();
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
                PlayerManager.Instance.InventoryData.InvokeOnAddItem(_item);
            }
        }

    }

}
