using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKey", menuName = "Inventory Item/Items/Key")]
public class Key : ItemObject
{
    [SerializeField] private GameObject target;
    public void Awake()
    {
        itemType = ItemType.Key;
    }
}
