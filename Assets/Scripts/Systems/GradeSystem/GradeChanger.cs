using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GradeChanger : MonoBehaviour
{
    [Separator("Grade", true)]
    [SerializeField]
    private SubjectController gradeController;
    [SerializeField]
    private SubjectObject grade;

    private void Awake()
    {
        gameObject.tag = "GradeChanger";
    }
}
