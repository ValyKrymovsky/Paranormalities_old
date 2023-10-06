using UnityEngine;
using MyCode.Managers;
using MyCode.GameData;
using Newtonsoft.Json;
using MyCode.Helper.Serializer;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace MyCode.Systems
{
    public class GameSaveEvent : EventSystem
    {
        [SerializeField] private SaveIndex _saveIndex;
        [SerializeField] private SerializableVector3 _saveLocation;
        private Thread _saveThread;

        private void Awake()
        {
            _saveLocation = new SerializableVector3(transform.position);
        }

        private void OnEnable()
        {
            OnEventStart += StartSaveProcess;
        }

        private void OnDisable()
        {
            OnEventStart -= StartSaveProcess;
        }

        private void StartSaveProcess()
        {
            _saveThread = new Thread(new ThreadStart(SaveGame));

            _saveThread.Start();
        }

        private void SaveGame()
        {
            // checks if current save index is smaller than this events save index
            if (_saveIndex <= GameSaveManager.CurrentGameSave.SaveIndex) return;

            GameSave oldSave = new GameSave();

            // Reads save file into a string
            string saveString = SaveSerializer.ReadSaveFile(GameSaveManager.saveFilePath);

            // Deserializes save string into a GameSave object
            oldSave = SaveSerializer.DeserializeGameSave(saveString, oldSave);

            List<int> itemIds = new List<int>();
            foreach (Item item in PlayerManager.InventoryData.Inventory.InventoryArray.Where(i => i != Item.empty))
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

            SaveSerializer.UpdateSave(newSave, ref oldSave);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            SaveSerializer.SerializeObject(serializer, newSave, GameSaveManager.saveFilePath);

            GameSaveManager.CurrentGameSave = newSave;
        }

    }

}
