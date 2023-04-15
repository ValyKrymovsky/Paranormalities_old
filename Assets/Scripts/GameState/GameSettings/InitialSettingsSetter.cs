using System.IO;
using UnityEngine;

public class InitialSettingsSetter : MonoBehaviour
{
    string path = "";
    string persistentPath = "";
    void Start()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";

        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        GameSettings data = JsonUtility.FromJson<GameSettings>(json);
        reader.Close();

        MixerObject.mixer.SetFloat("MasterVolume", Mathf.Log10(data.masterSoundVolume) * 20);
        MixerObject.mixer.SetFloat("MusicVolume", Mathf.Log10(data.musicSoundVolume) * 20);
    }

}
