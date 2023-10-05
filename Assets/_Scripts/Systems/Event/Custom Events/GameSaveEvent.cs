using UnityEngine;
using MyCode.Managers;
using MyCode.GameData;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using MyCode.Helper.Serializer;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MyCode.Systems
{
    [RequireComponent(typeof(EventSystem))]
    public class GameSaveEvent : MonoBehaviour
    {
        [SerializeField] private SaveIndex _saveIndex;
        [SerializeField] private SerializableVector3 _saveLocation;

        private EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
            _saveLocation = new SerializableVector3(transform.position);
        }

        private void OnEnable()
        {
            _eventSystem.OnEventStart += SaveGame;
        }

        private void OnDisable()
        {
            _eventSystem.OnEventStart -= SaveGame;
        }

        private async void SaveGame()
        {
            if (_saveIndex.CompareTo(GameSaveManager.CurrentGameSave.SaveIndex) < 1) return;

            GameSave oldSave = new GameSave();

            oldSave = SaveSerializer.DeserializeGameSave(await SaveSerializer.ReadSaveFileAsync(GameSaveManager.saveFilePath), oldSave);

            List<int> itemIds = new List<int>();
            foreach(Item item in PlayerManager.InventoryData.Inventory.InventoryArray.Where(i => i != Item.empty))
            {
                itemIds.Add(item.id);
            }

            if (PlayerManager.InventoryData.Inventory.PrimaryEquipment != Item.empty)
                itemIds.Add(PlayerManager.InventoryData.Inventory.PrimaryEquipment.id);
            if (PlayerManager.InventoryData.Inventory.SecondaryEquipment != Item.empty)
                itemIds.Add(PlayerManager.InventoryData.Inventory.SecondaryEquipment.id);

            GameSave newSave = new GameSave()
            {
                CheckpointLocation = _saveLocation,
                Health = 100,
                Inventory = itemIds.ToArray(),
                GameDifficulty = GameSaveManager.CurrentGameSave.GameDifficulty,
                SaveIndex = _saveIndex,
                SaveName = oldSave.SaveName,
                SaveTime = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                SavePath = GameSaveManager.CurrentGameSave.SavePath
            };

            SaveSerializer.UpdateSaveAsync(newSave, ref oldSave);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            await SaveSerializer.SerializeObjectAsync(serializer, newSave, GameSaveManager.saveFilePath);

            GameSaveManager.CurrentGameSave = newSave;
        }

    }

}
