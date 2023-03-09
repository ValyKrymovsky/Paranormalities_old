 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ItemData")]
public class Item : ScriptableObject
{
    public enum ItemType 
    {
        Key,
        Health,
        Valuable,
        Usable,
    }

    public string objectName;
    public ItemType itemType;
    public GameObject model;
    public Sprite picture;


}
