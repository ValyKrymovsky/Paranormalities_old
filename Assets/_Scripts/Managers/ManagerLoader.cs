using MyCode.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

    public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

    public async static void LoadManagers(DifficultyProperties _difficultyProp)
    {
        await PlayerManager.LoadManager(_difficultyProp);
    }

}
