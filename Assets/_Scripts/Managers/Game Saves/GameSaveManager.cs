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
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using MyCode.Player.Inventory;

namespace MyCode.Managers
{
    public class GameSaveManager : Manager<GameSaveManager>
    {

        public override async UniTask SetUpManager(DifficultyProperties _properties)
        {
            await CreateNewSave(_properties);
        }

        private async UniTask CreateNewSave(DifficultyProperties _properties)
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

            AsyncOperationHandle loadHandle = Addressables.LoadAssetAsync<ScriptableObject>("EmptyInventory");
            await loadHandle.Task;

            EmptyInventory = loadHandle.Result as InventoryObject;

            gs.SetPlayer((0, 0, 0),
                PlayerManager.Instance.HealthData.OriginalMaxHealth,
                PlayerManager.Instance.StaminaData.MaxStamina,
                false,
                EmptyInventory,
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

        internal InventoryObject EmptyInventory { get; private set; }
        public GameSave CurrentGameSave { get; private set; }
    }

}
