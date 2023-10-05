using MyCode.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectionObjective", menuName = "Game Objectives/Sub/Collection Objective")]
public class CollectionObjective : SubObjective
{
    public Item[] items;
}
