using MyCode.GameData.GameSettings;
using System.IO;
using System;
using UnityEngine;
using Newtonsoft.Json;
using MyCode.Helper.Serializer;
using DG.Tweening.Plugins.Core.PathCore;

public class SettingsLoader : MonoBehaviour
{
    [SerializeField] private GameSettingsData _data;

    private void Awake()
    {
        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace('\\', '/') + "/My Games/Paranormalities/Settings/";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string fullPath = String.Format(@"{0}settings.json", savePath);

        if (!File.Exists(fullPath))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            StreamWriter file = File.CreateText(fullPath);
            serializer.Serialize(file, _data);
            file.Close();

            return;
        }

        GameSettingsData gameSettingsData = JsonConvert.DeserializeObject<GameSettingsData>(File.ReadAllText(fullPath));

        _data.MasterVolume = gameSettingsData.MasterVolume;
        _data.MusicVolume = gameSettingsData.MusicVolume;
        _data.EffectsVolume = gameSettingsData.EffectsVolume;

        _data.AspectRatio = gameSettingsData.AspectRatio;
        _data.Resolution = gameSettingsData.Resolution;
        _data.Vsync = gameSettingsData.Vsync;

        _data.ShadowDetail = gameSettingsData.ShadowDetail;
        _data.TextureDetail = gameSettingsData.TextureDetail;
        _data.LightingDetail = gameSettingsData.LightingDetail;

        _data.Gamma = gameSettingsData.Gamma;

        _data.FieldOfView = gameSettingsData.FieldOfView;
    }
}
