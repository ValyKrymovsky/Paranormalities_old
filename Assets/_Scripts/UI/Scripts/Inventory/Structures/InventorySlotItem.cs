using System;
using UnityEngine;
using MyCode.Player.Inventory;

[Serializable]
public struct InventorySlotItem
{
    public InventoryItem item;

    public InventorySlotItem(InventoryItem _item)
    {
        this.item = _item;
    }
}
