using MyCode.Player.Inventory;
using System;
using UnityEngine;

namespace MyCode.Player.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        private ItemObject _item;
        [NonSerialized] private GameObject _model;
        [NonSerialized] private Sprite _image;
        public GameObject Model { get => _model; private set => _model = value; }
        public Sprite Image { get => _image; private set => _image = value; }
        public ItemObject Item { get => _item; private set => _item = value; }

        public InventoryItem(ItemObject item, GameObject model, Sprite image)
        {
            this._item = item;
            this._model = model;
            this._image = image;
        }
    }
}
