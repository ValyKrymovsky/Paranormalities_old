using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData.Inventory
{
    [CreateAssetMenu(fileName = "NewHealth", menuName = "Inventory System/Items/Health")]
    public class Health : ItemObject
    {
        [SerializeField] private int healValue;
        public void Awake()
        {
            itemType = ItemType.Health;
        }
    }
}

