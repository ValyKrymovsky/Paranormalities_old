using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MyBox;

public class SubjectController : MonoBehaviour
{

    private string subjectObjectsPath = "Assets" + Path.AltDirectorySeparatorChar + "Scripts" +
        Path.AltDirectorySeparatorChar + "Systems" +
        Path.AltDirectorySeparatorChar + "GradeSystem" +
        Path.AltDirectorySeparatorChar + "DefaultSubject" +
        Path.AltDirectorySeparatorChar;

    [Separator("Subject", true)]
    public List<(SubjectObject subjectType, int subject)> subjects = new List<(SubjectObject, int)>();
    [SerializeField]
    private int maxAcceptedsubject = 3;

    [Separator("Grade Changers", true)]
    [SerializeField]
    List<GameObject> gradeChangers = new List<GameObject>();

    private void Awake()
    {
        LoadAllsubjectObjects(subjectObjectsPath);
    }

    private void Start()
    {
        gradeChangers = GetAllGradeChangers();
        GenerateSubject();
    }

    private List<(SubjectObject subjectType, int subject)> GetWorstSubject()
    {
        List<(SubjectObject subjectType, int subject)> worstSubject = new List<(SubjectObject subjectType, int subject)>();

        foreach ((SubjectObject subjectType, int subject) subject in subjects)
        {
            if (subject.subject > maxAcceptedsubject)
                worstSubject.Add(subject);
        }

        return worstSubject;
    }

    private void LoadAllsubjectObjects(string _path)
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
            else if (worstsubjectGroup != SubjectGroup.Nothing && subjects[i].subjectType.subjectGroup != worstsubjectGroup)
            {
                generatedsubject = Random.Range(1, 4);
            }
            else if (worstsubjectGroup != SubjectGroup.Nothing && subjects[i].subjectType.subjectGroup == worstsubjectGroup)
            {
                generatedsubject = Random.Range(4, 6);
            }

            subjects[i] = (subjects[i].subjectType, generatedsubject);
            if (generatedsubject > worstsubject)
            {
                worstsubjectGroup = subjects[i].subjectType.subjectGroup;
                worstsubject = generatedsubject;
            }
                

        }
    }

    private void PrintAllSubject()
    {
        foreach ((SubjectObject subjectType, int subject) subject in subjects)
        {
            Debug.Log(subject);
        }
    }

    private List<GameObject> GetAllGradeChangers()
    {
        return GameObject.FindGameObjectsWithTag("GradeChanger").ToList();
    }

}
