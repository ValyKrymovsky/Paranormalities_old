using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using MyBox;

public class GradeController : MonoBehaviour
{

    private string gradeObjectsPath = "Assets" + Path.AltDirectorySeparatorChar + "Scripts" +
        Path.AltDirectorySeparatorChar + "Systems" +
        Path.AltDirectorySeparatorChar + "GradeSystem" +
        Path.AltDirectorySeparatorChar + "DefaultGrades" +
        Path.AltDirectorySeparatorChar;

    [Separator("Grades", true)]
    public List<(GradeObject gradeType, int grade)> grades = new List<(GradeObject, int)>();
    [SerializeField]
    private int maxAcceptedGrade = 3;

    private void Awake()
    {
        LoadAllGradeObjects(gradeObjectsPath);
    }

    private void Start()
    {
        GenerateGrades();
        // PrintAllGrades();
        foreach ((GradeObject gradeType, int grade) grade in GetWorstGrades())
        {
            Debug.Log(grade);
        }
    }

    private List<(GradeObject gradeType, int grade)> GetWorstGrades()
    {
        List<(GradeObject gradeType, int grade)> worstGrades = new List<(GradeObject gradeType, int grade)>();

        foreach ((GradeObject gradeType, int grade) grade in grades)
        {
            if (grade.grade > maxAcceptedGrade)
                worstGrades.Add(grade);
        }

        return worstGrades;
    }

    private void LoadAllGradeObjects(string _path)
    {
        List<Object> gradeObjects = new List<Object>();
        string[] temp = Directory.GetFiles(_path, "*.asset");


        foreach (string gradeName in temp)
        {
            int index = gradeName.LastIndexOf(Path.AltDirectorySeparatorChar);
            string localPath = _path;
           
            if (index > 0)
                localPath += gradeName.Substring(index + 1);

            gradeObjects.Add(AssetDatabase.LoadAssetAtPath(localPath, typeof(GradeObject)));
            // Debug.Log(localPath);
        }

        foreach (Object gradeObject in gradeObjects)
        {
            grades.Add(((GradeObject)gradeObject, 0));
        }
    }

    private void GenerateGrades()
    {
        GradeGroup worstGradeGroup = GradeGroup.Nothing;
        int worstGrade = 1;

        for (int i = 0; i < grades.Count; i++)
        {
            int generatedGrade = 0;
            if (worstGradeGroup == GradeGroup.Nothing)
            {
                generatedGrade = Random.Range(1, 6);
            }
            else if (worstGradeGroup != GradeGroup.Nothing && grades[i].gradeType.gradeGroup != worstGradeGroup)
            {
                generatedGrade = Random.Range(1, 4);
            }
            else if (worstGradeGroup != GradeGroup.Nothing && grades[i].gradeType.gradeGroup == worstGradeGroup)
            {
                generatedGrade = Random.Range(4, 6);
            }

            grades[i] = (grades[i].gradeType, generatedGrade);
            if (generatedGrade > worstGrade)
            {
                worstGradeGroup = grades[i].gradeType.gradeGroup;
                worstGrade = generatedGrade;
            }
                

        }
    }

    private void PrintAllGrades()
    {
        foreach ((GradeObject gradeType, int grade) grade in grades)
        {
            Debug.Log(grade);
        }
    }
}
