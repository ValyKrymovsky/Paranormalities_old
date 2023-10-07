using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewSubObjective", menuName = "DataObjects/Objectives/Sub/Basic Objective")]
    public class SubObjective : Objective
    {
        public SuperObjective superObjective;
    }
}
