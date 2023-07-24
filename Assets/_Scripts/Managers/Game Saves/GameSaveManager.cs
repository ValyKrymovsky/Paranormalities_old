using Cysharp.Threading.Tasks;
using MyCode.Data.GameSave;
using MyCode.Data.Settings;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MyCode.Managers;
using System.Collections.Generic;

public class GameSaveManager : Manager<GameSaveManager>
{

    public override async UniTask SetUpManager(DifficultyProperties _properties)
    {
        CreateNewSave(_properties);
    }

    private void CreateNewSave(DifficultyProperties _properties)
    {
        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace('\\', '/') + "/My Games/Paranormalities/Saves/";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        (Difficulty _dif, int _index)[] difficultyInteger = new (Difficulty _dif, int _index)[]
        {
            (Difficulty.easy, 0),
            (Difficulty.normal, 1),
            (Difficulty.hard, 2),
            (Difficulty.insane, 3),
            (Difficulty.nightmare, 4),
        };

        int _index = difficultyInteger.Where(dif => dif._dif == _properties.difficulty).First()._index;

        string[] saveFiles = Directory.GetFiles(savePath).Where(file => Regex.IsMatch(file, String.Format("(Save\\d+_{0}\\.json)", _index))).ToArray();

        GameSave gs = new GameSave();

        gs.SetPlayer((0, 0, 0),
            PlayerManager.Instance.HealthData.OriginalMaxHealth,
            PlayerManager.Instance.StaminaData.MaxStamina,
            false,
            PlayerManager.Instance.InventoryData.Inventory,
            null,
            null);

        gs.SetDifficulty(_properties);

        using (StreamWriter file = File.CreateText(String.Format(savePath + @"Save{0}_{1}.json", saveFiles.Count() + 1, _index)))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;

            serializer.Serialize(file, gs);
        }
        
    }

    public GameSave CurrentGameSave { get; private set; }
}

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    [JsonIgnore]
    public Vector3 UnityVector
    {
        get
        {
            return new Vector3(x, y, z);
        }
    }

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public static List<SerializableVector3> GetSerializableList(List<Vector3> vList)
    {
        List<SerializableVector3> list = new List<SerializableVector3>(vList.Count);
        for (int i = 0; i < vList.Count; i++)
        {
            list.Add(new SerializableVector3(vList[i]));
        }
        return list;
    }

    public static List<Vector3> GetSerializableList(List<SerializableVector3> vList)
    {
        List<Vector3> list = new List<Vector3>(vList.Count);
        for (int i = 0; i < vList.Count; i++)
        {
            list.Add(vList[i].UnityVector);
        }
        return list;
    }
}
