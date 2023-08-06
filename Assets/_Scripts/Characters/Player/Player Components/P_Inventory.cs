using MyCode.GameData.Inventory;
using MyCode.Managers;
using UnityEngine;

namespace MyCode.Player.Components
{
    public class P_Inventory : MonoBehaviour
    {
        void Start()
        {
            PlayerManager.Instance.InventoryData.Inventory.inventory = PlayerManager.Instance.ReplaceAllItems(PlayerManager.Instance.InventoryData.Inventory.inventory);
            if (PlayerManager.Instance.InventoryData.PrimaryEquipment != null) PlayerManager.Instance.InventoryData.PrimaryEquipment = InventoryItemStorage.GetItem(PlayerManager.Instance.InventoryData.PrimaryEquipment.ItemId);
            if (PlayerManager.Instance.InventoryData.SecondaryEquipment != null) PlayerManager.Instance.InventoryData.SecondaryEquipment = InventoryItemStorage.GetItem(PlayerManager.Instance.InventoryData.SecondaryEquipment.ItemId);
        }
    }
}
