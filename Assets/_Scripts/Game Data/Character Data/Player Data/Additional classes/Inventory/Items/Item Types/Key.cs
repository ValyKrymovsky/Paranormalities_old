using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewKey", menuName = "Inventory System/Items/Key")]
    public class Key : Item
    {
        private GameObject target;
        public void Awake()
        {
            type = ItemType.Key;
        }
    }
}

