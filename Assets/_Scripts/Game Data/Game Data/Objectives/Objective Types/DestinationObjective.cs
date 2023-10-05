using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDestinationObjective", menuName = "Game Objectives/Sub/Destination Objective")]
public class DestinationObjective : SubObjective
{
    [SerializeField] private Transform destination;
    [Tooltip("Minimal distance to the destination for the objective to be completed")]
    [SerializeField] private float distance;
}
