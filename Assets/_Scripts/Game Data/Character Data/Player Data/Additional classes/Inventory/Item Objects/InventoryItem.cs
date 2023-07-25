using MyCode.Player.Inventory;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyCode.Player.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        public static InventoryItem empty = new InventoryItem(0, null, null, null);

        [SerializeField][Tooltip("A 4-digit unique number associated to the InventoryItem")] private int _itemId;
        [SerializeField] private ItemObject _item;
        [JsonIgnore] private GameObject _model;
        [SerializeField] private Sprite _image;

        public int ItemId { get => _itemId; set => _itemId = value; }
        [JsonIgnore] public GameObject Model { get => _model; set => _model = value; }
        [JsonIgnore] public Sprite Image { get => _image; private set => _image = value; }
        public ItemObject Item { get => _item; private set => _item = value; }
        

        public InventoryItem(int id, ItemObject item, GameObject model, Sprite image)
        {
            _itemId = id;
            _item = item;
            _model = model;
            _image = image;
        }
    }
}
