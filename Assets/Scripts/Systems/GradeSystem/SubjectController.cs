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
    public List<(SubjectObject subject, int grade)> subjects = new List<(SubjectObject, int)>();
    [SerializeField]
    private int maxAcceptedGrade = 3;

    [Separator("Grade Changers", true)]
    [SerializeField]
    List<GameObject> gradeChangers = new List<GameObject>();
    [SerializeField]
    private float gradeStatusCheckInterval = .5f;

    private void Awake()
    {
        LoadAllSubjectObjects(subjectObjectsPath);
    }

    private void Start()
    {
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

    private void LoadAllSubjectObjects(string _path)
    {
        List<Object> subjectObjects = new List<Object>();
        string[] temp = Directory.GetFiles(_path, "*.asset");


        foreach (string subjectName in temp)
        {
            int index = subjectName.LastIndexOf(Path.AltDirectorySeparatorChar);
            string localPath = _path;
           
            if (index > 0)
                localPath += subjectName.Substring(index + 1);

            subjectObjects.Add(AssetDatabase.LoadAssetAtPath(localPath, typeof(SubjectObject)));
        }

        foreach (Object subjectObject in subjectObjects)
        {
            subjects.Add(((SubjectObject)subjectObject, 0));
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
            if (generatedsubject > worstsubject)
            {
                worstsubjectGroup = subjects[i].subject.subjectGroup;
                worstsubject = generatedsubject;
            }
                

        }
    }

    private void PrintAllSubject()
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
                if (_subject.Equals(subject))
                    return subject.grade;
            }
        }
        return 0;
    }

    public int SetGrade(SubjectObject _subject, int _grade)
    {
        if (HasSubject(_subject))
        {
            for (int i = 0; i < subjects.Count; i++)
            {
                if (_subject.Equals(subjects[i]))
                    subjects.Insert(i, (subjects[i].subject, _grade));
            }
        }
        return 0;
    }

    public bool HasSubject(SubjectObject _subject)
    {
        return subjects.Any(s => s.subject == _subject);
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
        throw new System.NotImplementedException();
    }
}
