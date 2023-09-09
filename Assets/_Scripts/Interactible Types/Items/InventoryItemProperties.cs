using UnityEngine;
using MyBox;
using MyCode.Managers;
using MyCode.GameData.Inventory;
using MyCode.GameData.Interaction;

namespace MyCode.Interactibles
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
            InventoryItemStorage.AddItem(_item);
        }

        private void Start()
        {
            _item.Model = gameObject;

            _item.ItemId = _item.ItemId == 0 ? UnityEngine.Random.Range(1000, 10000) : _item.ItemId;
        }

        private void OnEnable()
        {
            _intController.OnInteracted += AddToInventory;
            if (_item != InventoryItem.empty)
                InventoryItemStorage.AddItem(_item);
        }

        private void OnDisable()
        {
            _intController.OnInteracted -= AddToInventory;
        }

        public void AddToInventory()
        {
            if (!PlayerManager.InventoryData.AddItem(_item)) return;
            
            this.gameObject.SetActive(false);
        }


    }
}
