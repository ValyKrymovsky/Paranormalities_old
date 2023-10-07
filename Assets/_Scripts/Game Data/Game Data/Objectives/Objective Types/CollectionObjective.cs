using MyCode.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [CreateAssetMenu(fileName = "NewCollectionObjective", menuName = "DataObjects/Objectives/Sub/Collection Objective")]
    public class CollectionObjective : SubObjective
    {
        public Item[] requiredItems;


    }
}
