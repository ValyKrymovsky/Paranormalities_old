using Cysharp.Threading.Tasks;
using MyCode.GameData;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using MyCode.Helper.Serializer;

namespace MyCode.Managers
{
    public class GameSaveManager
    {
        private static readonly object _lock = new object();
        private static GameSaveManager _instance;
        public static GameSaveManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new GameSaveManager();
                }
                return _instance;
            }
        }

        public void SetSave(GameSave _save)
        {
            CurrentGameSave = _save;
            saveFilePath = _save.SavePath;
        }

        public async UniTaskVoid CreateNewSave(DifficultyProperties _properties)
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
            string fullPath = String.Format(savePath + @"Save{0}_{1}.json", saveFiles.Count() + 1, _index);

            GameSave gs = new GameSave()
            {
                CheckpointLocation = new SerializableVector3(0, 0, 0),
                Health = 100,
                Inventory = null,
                GameDifficulty = _properties.difficulty,
                SaveIndex = SaveIndex.entrance,
                SaveName = String.Format("{0}_{1}", difficultyInteger[_index]._dif.ToString(), saveFiles.Count()),
                SaveTime = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                SavePath = fullPath
        };

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            await SaveSerializer.SerializeObjectAsync(serializer, gs, fullPath);

            saveFilePath = fullPath;
            CurrentGameSave = gs;
        }

        [field: SerializeField] public GameSave CurrentGameSave { get; set; }
        public string saveFilePath;
    }

}
