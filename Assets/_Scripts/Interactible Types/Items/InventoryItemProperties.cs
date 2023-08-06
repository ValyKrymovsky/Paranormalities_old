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
            if (PlayerManager.Instance.InventoryData.Inventory.AddItem(_item))
            {
                PlayerManager.Instance.InventoryData.InvokeOnAddItem(_item);

                if (_item.Item.itemType == ItemObject.ItemType.Equipment)
                {
                    if (PlayerManager.Instance.InventoryData.PrimaryEquipment == InventoryItem.empty)
                    {
                        PlayerManager.Instance.InventoryData.PrimaryEquipment = _item;
                    }
                    else if (PlayerManager.Instance.InventoryData.SecondaryEquipment == InventoryItem.empty)
                    {
                        PlayerManager.Instance.InventoryData.SecondaryEquipment = _item;
                    }
                }

                this.gameObject.SetActive(false);

            }
        }


    }
}
