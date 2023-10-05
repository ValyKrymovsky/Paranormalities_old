using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory System/Items/Equipment")]
    public class Equipment : Item
    {
        public void Awake()
        {
            type = ItemType.Equipment;
        }
    }
}

