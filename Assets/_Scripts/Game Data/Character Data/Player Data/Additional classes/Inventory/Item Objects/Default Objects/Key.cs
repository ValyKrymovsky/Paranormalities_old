using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.Player.Inventory
{
    [CreateAssetMenu(fileName = "NewKey", menuName = "Inventory System/Items/Key")]
    public class Key : ItemObject
    {
        private GameObject target;
        public void Awake()
        {
            itemType = ItemType.Key;
        }
    }
}

