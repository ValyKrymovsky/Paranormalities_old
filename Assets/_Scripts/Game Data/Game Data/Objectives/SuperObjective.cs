using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuperObjective", menuName = "Game Objectives/Super/Objective")]
public class SuperObjective : Objective
{
    public Objective[] subObjectives;
}
