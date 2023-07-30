using UnityEngine;

namespace MyCode.GameData.Inventory
{
    [CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory System/Items/Equipment")]
    public class Equipment : ItemObject
    {
        public void Awake()
        {
            itemType = ItemType.Equipment;
        }
    }
}

