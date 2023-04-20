using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Subject
    {
        English,
        Math,
        Music,
        CreativeWorks,
        Geography,
        Economy,
        Gymnastics,
        IT,
        Science,
        French
    }

    public enum SubjectGroup
    {
        Nothing,
        General,
        Language,
        Mathematics,
        Physical,
    }

[CreateAssetMenu(fileName = "NewSubject", menuName = "Grade System/Subject")]
public class SubjectObject : ScriptableObject
{
    public Subject subject;
    public SubjectGroup subjectGroup;
}
