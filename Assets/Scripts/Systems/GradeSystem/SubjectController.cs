using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MyBox;

public class SubjectController : MonoBehaviour, IInteractable
{

    private string subjectObjectsPath = "Assets" + Path.AltDirectorySeparatorChar + "Scripts" +
        Path.AltDirectorySeparatorChar + "Systems" +
        Path.AltDirectorySeparatorChar + "GradeSystem" +
        Path.AltDirectorySeparatorChar + "DefaultSubject" +
        Path.AltDirectorySeparatorChar;

    [Separator("Subject", true)]
    [SerializeField]
    private List<SubjectObject> subjectObjects = new List<SubjectObject>();
    public List<(SubjectObject subject, int grade)> subjects = new List<(SubjectObject, int)>();
    [SerializeField]
    private int maxAcceptedGrade = 3;

    [Separator("Grade Changers", true)]
    [SerializeField]
    List<GameObject> gradeChangers = new List<GameObject>();
    [SerializeField]
    private float gradeStatusCheckInterval = .5f;

    private EndGame endGame;

    private void Awake()
    {
        endGame = new EndGame("MainMenu");
        LoadAllSubjectObjects();
        gradeChangers = GetAllGradeChangers();
        GenerateSubject();
    }

    private List<(SubjectObject subject, int grade)> GetWorstSubject()
    {
        List<(SubjectObject subject, int grade)> worstSubject = new List<(SubjectObject subject, int grade)>();

        foreach ((SubjectObject subject, int grade) subject in subjects)
        {
            if (subject.grade > maxAcceptedGrade)
                worstSubject.Add(subject);
        }

        return worstSubject;
    }

    private void LoadAllSubjectObjects()
    {
        foreach (SubjectObject subjectObject in subjectObjects)
        {
            subjects.Add((subjectObject, 0));
        }
    }

    private void GenerateSubject()
    {
        SubjectGroup worstsubjectGroup = SubjectGroup.Nothing;
        int worstsubject = 1;

        for (int i = 0; i < subjects.Count; i++)
        {
            int generatedsubject = 0;
            if (worstsubjectGroup == SubjectGroup.Nothing)
            {
                generatedsubject = Random.Range(1, 6);
            }
            else if (worstsubjectGroup != SubjectGroup.Nothing && subjects[i].subject.subjectGroup != worstsubjectGroup)
            {
                generatedsubject = Random.Range(1, 4);
            }
            else if (worstsubjectGroup != SubjectGroup.Nothing && subjects[i].subject.subjectGroup == worstsubjectGroup)
            {
                generatedsubject = Random.Range(4, 6);
            }

            subjects[i] = (subjects[i].subject, generatedsubject);
            Debug.Log(subjects[i].subject + ", " + generatedsubject);
            if (generatedsubject > worstsubject)
            {
                worstsubjectGroup = subjects[i].subject.subjectGroup;
                worstsubject = generatedsubject;
            }
                

        }
    }

    public void PrintAllSubject()
    {
        foreach ((SubjectObject subject, int grade) grade in subjects)
        {
            Debug.Log(grade);
        }
    }

    private List<GameObject> GetAllGradeChangers()
    {
        return GameObject.FindGameObjectsWithTag("GradeChanger").ToList();
    }

    public int GetGrade(SubjectObject _subject)
    {
        if (HasSubject(_subject))
        {
            foreach ((SubjectObject subject, int grade) subject in subjects)
            {
                if (_subject.Equals(subject.subject))
                    return subject.grade;
            }
        }
        return 0;
    }

    public void SetGrade(SubjectObject _subject, int _grade)
    {
        if (HasSubject(_subject))
        {
            for (int i = 0; i < subjects.Count; i++)
            {
                if (subjects[i].subject == _subject)
                    subjects[i] = (subjects[i].subject, _grade);
            }
        }
    }

    public bool HasSubject(SubjectObject _subject)
    {
        foreach ((SubjectObject subject, int grade) subject in subjects)
        {
            if (subject.subject == _subject)
                return true;
        }
        return false;
    }

    private IEnumerator CheckGradeChangers()
    {
        while (true)
        {
            foreach (GameObject gradeChangerObject in gradeChangers)
            {
                GradeChanger gradeChanger = gradeChangerObject.GetComponent<GradeChanger>();
                if (gradeChanger.GradeChanged())
                {
                    SetGrade(gradeChanger.GetSubject(), gradeChanger.GetGrade());
                }
            }

            yield return new WaitForSeconds(gradeStatusCheckInterval);
        }
    }

    public void Interact()
    {
        if (CanEndGame())
        {
            endGame.LoadMainMenu();
        }
    }

    private bool CanEndGame()
    {
        bool canEndGame = true;
        foreach ((SubjectObject subject, int grade) subject in subjects)
        {
            if (subject.grade > maxAcceptedGrade)
            {
                canEndGame = false;
            }
        }

        return canEndGame;
    }
}
