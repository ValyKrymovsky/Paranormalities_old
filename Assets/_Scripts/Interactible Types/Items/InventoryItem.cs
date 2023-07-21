using UnityEngine;
using MyBox;
using MyCode.Managers;
using MyCode.Player.Inventory;
using MyCode.Player.Interaction;

namespace MyCode.Interactibles
{
    public class InventoryItem : MonoBehaviour
    {
        [Space]
        [Separator("Inventory")]
        [Space]

        [Header("Inventory Item")]
        [Space]
        [SerializeField] private ItemObject _item;
        public GameObject model;
        public Sprite itemImage;

        private InteractionController _intController;

        private void Awake()
        {
            model = this.gameObject;
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
            if (PlayerManager.Instance.InventoryData.Inventory.AddItem(_item, model))
            {
                Debug.Log("Added " + _item);
                PlayerManager.Instance.InventoryData.InvokeOnAddItem(_item, model, itemImage);
            }
        }

    }

}
