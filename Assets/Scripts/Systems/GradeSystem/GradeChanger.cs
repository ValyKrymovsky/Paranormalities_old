using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class GradeChanger : MonoBehaviour, IInteractable
{
    [Separator("Grade", true)]
    [SerializeField]
    private string subjectControllerTag = "";
    [SerializeField]
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

    private void Start()
    {
        grade = subjectController.GetGrade(subject);
        Debug.Log(subjectController.GetGrade(subject));
    }

    public void Interact()
    {
        if (!gradeChanged)
        {
            grade = 1;
            gradeChanged = true;
            subjectController.SetGrade(subject, grade);
            Debug.Log("Grade changed");
        }
        subjectController.PrintAllSubject();
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
