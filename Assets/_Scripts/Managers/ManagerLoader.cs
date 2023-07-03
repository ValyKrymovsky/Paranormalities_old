using MyCode.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

    public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

    public static void LoadManagers(DifficultyProperties _difficultyProp)
    {
        Debug.Log("active diff: " + _difficultyProp.difficulty);
    }

}
