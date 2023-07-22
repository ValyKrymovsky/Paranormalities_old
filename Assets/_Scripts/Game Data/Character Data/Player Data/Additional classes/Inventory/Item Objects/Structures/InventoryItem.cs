using MyCode.Player.Inventory;
using System;
using UnityEngine;

namespace MyCode.Player.Inventory
{
    [Serializable]
    public struct InventoryItem
    {
        [field: SerializeField] public ItemObject Item { get; private set; }
        [field: SerializeField] public GameObject Model { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }

        public InventoryItem(ItemObject item, GameObject model, Sprite image)
        {
            this.Item = item;
            this.Model = model;
            this.Image = image;
        }
    }
}
