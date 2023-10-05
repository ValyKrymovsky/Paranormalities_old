using UnityEngine;
using MyBox;
using MyCode.Managers;
using MyCode.GameData;

namespace MyCode.Interactibles
{
    public class InventoryItem : MonoBehaviour
    {
        [Space]
        [Separator("Inventory")]
        [Space]

        [Header("Inventory Item")]
        [Space]
        [SerializeField] private Item _item;
        private InteractionController _intController;

        private void Awake()
        {
            _intController = GetComponent<InteractionController>();
            _item.itemModel = gameObject;
            _item.itemId = _item.itemId == 0 ? UnityEngine.Random.Range(1000, 10000) : _item.itemId;
            Item.RegisterItem(_item);
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
            if (!PlayerManager.InventoryData.Inventory.AddItem(_item)) return;
            
            this.gameObject.SetActive(false);
        }


    }
}
