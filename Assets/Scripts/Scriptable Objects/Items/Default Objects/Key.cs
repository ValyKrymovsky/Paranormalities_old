using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKey", menuName = "Inventory System/Items/Key")]
public class Key : ItemObject
{
    [SerializeField] private GameObject target;
    public void Awake()
    {
        itemType = ItemType.Key;
    }
}
