using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyCode.GameData
{
    [Serializable]
    public class InventoryItem
    {
        public static InventoryItem empty = new InventoryItem(0, null, null, null);

        private int _itemId;
        private Item _item;
        private GameObject _model;
        private Sprite _image;

        public int ItemId { get => _itemId; set => _itemId = value; }
        [JsonIgnore] public GameObject Model { get => _model; set => _model = value; }
        [JsonIgnore] public Sprite Image { get => _image; private set => _image = value; }
        public Item Item { get => _item; private set => _item = value; }
        

        public InventoryItem(int id, Item item, GameObject model, Sprite image)
        {
            _itemId = id;
            _item = item;
            _model = model;
            _image = image;
        }
    }
}
