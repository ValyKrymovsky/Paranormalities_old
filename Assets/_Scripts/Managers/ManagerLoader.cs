using MyCode.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyCode.Data.Settings;
using MyCode.Managers;
using MyCode.Player.Interaction;


public class ManagerLoader : MonoBehaviour
{
    [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

    public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

    public async static void LoadManagers(DifficultyProperties _difficultyProp)
    {
        DeleteAllGarbage();

        // Game Settings setter
        await SettingsManager.LoadManager(_difficultyProp);

        // Player Manager
        await PlayerManager.LoadManager(_difficultyProp);

        // Sound Manager
        await PlayerSoundManager.LoadManager();

        if (PlayerSoundManager.Instance.SoundData.SoundObjects.Count > 0)
        {
            PlayerSoundManager.Instance.SoundData.SoundObjects.Clear();
        }
        PlayerSoundManager.Instance.LoadSoundObjects();

        // Popup Manager
        await PopupManager.LoadManager();

        // Load Main Scene
        await SceneLoader.LoadScene(Scene.DebugScene);

        
    }

    private static void DeleteAllGarbage()
    {
        SettingsManager[] tempSM = GameObject.FindObjectsByType<SettingsManager>(FindObjectsSortMode.None);
        if (tempSM.Length != 0)
        {
            foreach (SettingsManager s in tempSM)
            {
                Destroy(s);
            }
        }

        PlayerManager[] tempPM = GameObject.FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        if (tempPM.Length != 0)
        {
            foreach (PlayerManager p in tempPM)
            {
                Destroy(p);
            }
        }

        PlayerSoundManager[] tempPSM = GameObject.FindObjectsByType<PlayerSoundManager>(FindObjectsSortMode.None);
        if (tempPSM.Length != 0)
        {
            foreach (PlayerSoundManager ps in tempPSM)
            {
                Destroy(ps);
            }
        }

        PopupManager[] tempPUM = GameObject.FindObjectsByType<PopupManager>(FindObjectsSortMode.None);
        if (tempPUM.Length != 0)
        {
            foreach (PopupManager pu in tempPUM)
            {
                Destroy(pu);
            }
        }

        InteractionPopup[] tempIP = GameObject.FindObjectsByType<InteractionPopup>(FindObjectsSortMode.None);
        if (tempIP.Length != 0)
        {
            foreach (InteractionPopup ip in tempIP)
            {
                Destroy(ip);
            }
        }
    }
}
