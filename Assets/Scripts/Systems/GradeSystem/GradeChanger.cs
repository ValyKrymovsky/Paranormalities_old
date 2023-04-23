using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GradeChanger : MonoBehaviour, IInteractable
{
    [Separator("Grade", true)]
    [SerializeField]
    private string subjectControllerTag = "";
    private SubjectController subjectController;
    [SerializeField]
    private SubjectObject subject;
    [SerializeField]
    private int grade;
    private bool gradeChanged;

    private void Awake()
    {
        subjectController = GameObject.FindGameObjectWithTag(subjectControllerTag).GetComponent<SubjectController>();
        gameObject.tag = "GradeChanger";
        // grade = subjectController.GetGrade(subject);
        gradeChanged = false;
    }

    public void Interact()
    {
        if (!gradeChanged)
        {
            grade = 1;
            gradeChanged = true;
            
        }
        Debug.Log("Grade changed");
    }

    public bool GradeChanged()
    {
        return gradeChanged;
    }

    public int GetGrade()
    {
        return grade;
    }

    public void SetGrade(int _grade)
    {
        grade = _grade;
    }

    public SubjectObject GetSubject()
    {
        return subject;
    }
    
}
