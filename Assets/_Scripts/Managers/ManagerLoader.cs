using MyCode.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

    public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

    public async static void LoadManagers(DifficultyProperties _difficultyProp)
    {
        await PlayerManager.LoadManager(_difficultyProp);
        await PopupManager.LoadManager();
        await SceneLoader.LoadScene(Scene.DebugScene);
    }

}
