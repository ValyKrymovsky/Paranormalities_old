using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewUsable", menuName = "Inventory System/Items/Usable")]
    public class Usable : Item
    {
        [SerializeField] private bool hasDuration;
        [SerializeField] private float duration;
        public void Awake()
        {
            itemType = ItemType.Usable;
        }
    }
}

