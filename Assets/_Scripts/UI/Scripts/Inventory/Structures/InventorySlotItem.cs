using System;
using UnityEngine;
using MyCode.Player.Inventory;

[Serializable]
public struct InventorySlotItem
{
    public ItemObject _itemObject;
    public GameObject _model;
    public Sprite _sprite;

    public InventorySlotItem(ItemObject itemObject, GameObject model, Sprite sprite)
    {
        _itemObject = itemObject;
        _sprite = sprite;
        _model = model;
    }
}
