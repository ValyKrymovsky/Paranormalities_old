using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewSuperObjective", menuName = "DataObjects/Objectives/Super/Basic Objective")]
public class SuperObjective : Objective
{
    public SubObjective[] subObjectives;
}
