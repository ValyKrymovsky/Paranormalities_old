using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettingsSave : MonoBehaviour
{
    private GameSettings gameSettings;

    private string path = "";
    private string persistentPath = "";

    public void CreatePlayerData(float _masterSoundVolume, float _musicSoundVolume)
    {
        gameSettings = new GameSettings(_masterSoundVolume, _musicSoundVolume);
    }

    public void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
    }

    public void SaveData()
    {
        string savePath = path;
        Debug.Log("saving data at " + savePath);

        string json = JsonUtility.ToJson(gameSettings);
        Debug.Log(json);

        StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
        writer.Close();
    }

    public GameSettings LoadData()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        GameSettings data = JsonUtility.FromJson<GameSettings>(json);
        reader.Close();
        return data;
    }

    public void ApplySettings()
    {
        MixerObject.mixer.SetFloat("MasterVolume", Mathf.Log10(gameSettings.masterSoundVolume) * 20);
        MixerObject.mixer.SetFloat("MusicVolume", Mathf.Log10(gameSettings.musicSoundVolume) * 20);
    }
}
