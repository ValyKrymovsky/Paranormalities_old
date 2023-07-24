using System.Collections.Generic;
using UnityEngine;
using MyCode.Data.Settings;
using MyCode.Managers;
using MyCode.Player.Interaction;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

    public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

    public async static void LoadNewManagers(DifficultyProperties _difficultyProp)
    {
        DeleteAllManagers();

        UniTask[] taskPool = new UniTask[]
        {
            SettingsManager.LoadManager(SettingsManager._instance),
            GameSaveManager.LoadManager(GameSaveManager._instance),
            PopupManager.LoadManager(PopupManager._instance),
            PlayerManager.LoadManager(PlayerManager._instance),
            PlayerSoundManager.LoadManager(PlayerSoundManager._instance),
        };

        await UniTask.WhenAll(taskPool);

        taskPool = new UniTask[]
        {
        SettingsManager.Instance.SetUpManager(_difficultyProp),
        GameSaveManager.Instance.SetUpManager(_difficultyProp),
        PlayerManager.Instance.SetUpManager(_difficultyProp),
        PopupManager.Instance.SetUpManager(_difficultyProp),
        };

        await UniTask.WhenAll(taskPool);

        // Load Main Scene
        await SceneLoader.LoadScene(Scene.DebugScene);

    }

    private static void DeleteAllManagers()
    {
        SettingsManager[] tempSM = GameObject.FindObjectsByType<SettingsManager>(FindObjectsSortMode.None);
        if (tempSM.Length != 0)
        {
            foreach (SettingsManager s in tempSM)
            {
                Destroy(s.gameObject);
            }
        }

        PlayerManager[] tempPM = GameObject.FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        if (tempPM.Length != 0)
        {
            foreach (PlayerManager p in tempPM)
            {
                Destroy(p.gameObject);
            }
        }

        PlayerSoundManager[] tempPSM = GameObject.FindObjectsByType<PlayerSoundManager>(FindObjectsSortMode.None);
        if (tempPSM.Length != 0)
        {
            foreach (PlayerSoundManager ps in tempPSM)
            {
                Destroy(ps.gameObject);
            }
        }

        PopupManager[] tempPUM = GameObject.FindObjectsByType<PopupManager>(FindObjectsSortMode.None);
        if (tempPUM.Length != 0)
        {
            foreach (PopupManager pu in tempPUM)
            {
                Destroy(pu.gameObject);
            }
        }

        InteractionPopup[] tempIP = GameObject.FindObjectsByType<InteractionPopup>(FindObjectsSortMode.None);
        if (tempIP.Length != 0)
        {
            foreach (InteractionPopup ip in tempIP)
            {
                Destroy(ip.gameObject);
            }
        }
    }
}
