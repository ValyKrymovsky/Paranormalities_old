using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewHealth", menuName = "Inventory System/Items/Health")]
    public class Health : Item
    {
        [SerializeField] private int healValue;
        public void Awake()
        {
            type = ItemType.Health;
        }
    }
}

