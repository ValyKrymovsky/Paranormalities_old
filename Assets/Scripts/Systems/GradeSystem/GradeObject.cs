using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GradeType
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

    public enum GradeGroup
    {
        Nothing,
        General,
        Language,
        Mathematics,
        Physical,
    }

[CreateAssetMenu(fileName = "NewGrade", menuName = "Grade System/Grade")]
public class GradeObject : ScriptableObject
{
    public GradeType gradeType;
    public GradeGroup gradeGroup;
    public GameObject gradeChanger;
}
