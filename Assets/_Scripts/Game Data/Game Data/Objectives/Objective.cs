using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    None, Destination, Collection, Interaction, Mixed
}

public class Objective : ScriptableObject
{
    public int id;
    public new int name;

    public event Action OnCompleted;
}
