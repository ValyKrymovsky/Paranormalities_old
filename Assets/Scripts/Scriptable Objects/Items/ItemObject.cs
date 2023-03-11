 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDefault", menuName = "Inventory System")]
public abstract class ItemObject : ScriptableObject
{
    public enum ItemType 
    {
        Key,
        Health,
        Usable,
    }

    public string objectName;
    public ItemType itemType;
    public Sprite picture;
    [TextArea(15,20)]
    public string description;


}
