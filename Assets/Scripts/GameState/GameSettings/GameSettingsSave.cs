using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettingsSave
{
    private GameSettings gameSettings;

    private string path = "";
    private string persistentPath = "";

    public void CreatePlayerData(float _masterSoundVolume, float _musicSoundVolume)
    {
        gameSettings = new GameSettings(_masterSoundVolume, _musicSoundVolume);
    }

    /// <summary>
    /// Sets up paths for both editor app and built game.
    /// </summary>
    public void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
    }

    /// <summary>
    /// Saves gameSettings object to json file
    /// </summary>
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

    /// <summary>
    /// Returns data from json file.
    /// </summary>
    /// <returns></returns>
    public GameSettings LoadData()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        GameSettings data = JsonUtility.FromJson<GameSettings>(json);
        reader.Close();
        return data;
    }

    /// <summary>
    /// Applies volume settings.
    /// </summary>
    public void ApplySettings()
    {
        MixerObject.mixer.SetFloat("MasterVolume", Mathf.Log10(gameSettings.masterSoundVolume) * 20);
        MixerObject.mixer.SetFloat("MusicVolume", Mathf.Log10(gameSettings.musicSoundVolume) * 20);
    }
}