 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType 
    {
        Key,
        Health,
        Valuable,
        Usable,
    }

    public ItemType itemType;

}
