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
        // Player Manager
        await PlayerManager.LoadManager(_difficultyProp);

        // Popup Manager
        await PopupManager.LoadManager();

        // Sound Manager
        await PlayerSoundManager.LoadManager();
        if (PlayerSoundManager.Instance.SoundData.SoundObjects.Count > 0)
        {
            PlayerSoundManager.Instance.SoundData.SoundObjects.Clear();
        }
        PlayerSoundManager.Instance.LoadSoundObjects();

        // Load Main Scene
        await SceneLoader.LoadScene(Scene.DebugScene);

        Debug.DrawLine(new Vector3(0, 2, 0), new Vector3(1, 2, 0) * 10, Color.yellow, 500f);
    }

}
