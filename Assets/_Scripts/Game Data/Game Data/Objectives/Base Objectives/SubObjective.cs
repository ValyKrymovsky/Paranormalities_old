using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewSubObjective", menuName = "Game Objectives/Sub/Basic Objective")]
public class SubObjective : Objective
{
    public SuperObjective superObjective;
}
