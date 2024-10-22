using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.Player.Inventory
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

