using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyCode.GameData
{
    public enum ObjectiveCompletionType
    {
        Indexed, Concurrent, Random
    }


    [Serializable]
    [CreateAssetMenu(fileName = "NewSuperObjective", menuName = "DataObjects/Objectives/Super/Basic Objective")]
    public class SuperObjective : Objective
    {
        public ObjectiveCompletionType completionType;
        public SubObjective[] subObjectives;

        public void RandomizeObjectives()
        {
            subObjectives = subObjectives.OrderBy(_ => new System.Random().Next()).ToArray();
        }
    }
}

