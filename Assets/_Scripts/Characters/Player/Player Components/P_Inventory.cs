using MyCode.GameData.Inventory;
using MyCode.Managers;
using UnityEngine;

namespace MyCode.Player.Components
{
    public class P_Inventory : MonoBehaviour
    {
        public InventoryItem[] _inventory;
        public InventoryItem _primaryEquipment;
        public InventoryItem _secondaryEquipment;

        private ManagerLoader _managerLoader;
        private void Awake()
        {

        }

        void Start()
        {
            PlayerManager.InventoryData.Inventory = PlayerManager.ReplaceAllItems(PlayerManager.InventoryData.Inventory);
            if (PlayerManager.InventoryData.PrimaryEquipment != null) PlayerManager.InventoryData.PrimaryEquipment = InventoryItemStorage.GetItem(PlayerManager.InventoryData.PrimaryEquipment.ItemId);
            if (PlayerManager.InventoryData.SecondaryEquipment != null) PlayerManager.InventoryData.SecondaryEquipment = InventoryItemStorage.GetItem(PlayerManager.InventoryData.SecondaryEquipment.ItemId);

            _inventory = PlayerManager.InventoryData.Inventory;
            _primaryEquipment = PlayerManager.InventoryData.PrimaryEquipment;
            _secondaryEquipment = PlayerManager.InventoryData.SecondaryEquipment;
        }
    }
}
