using UnityEngine;
using MyCode.Managers;
using MyCode.GameData.GameSave;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using MyCode.Helper.Serializer;

namespace MyCode.Systems
{
    [RequireComponent(typeof(EventSystem))]
    public class GameSaveEvent : MonoBehaviour
    {
        [SerializeField] private SaveIndex _saveIndex;
        [SerializeField] private float[] _saveLocation = new float[3];

        private EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
            _saveLocation[0] = transform.position.x;
            _saveLocation[1] = transform.position.y;
            _saveLocation[2] = transform.position.z;
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

            GameSave saveForUpdate = new GameSave();

            saveForUpdate = SaveSerializer.DeserializeGameSave(await SaveSerializer.ReadSaveFileAsync(GameSaveManager.saveFilePath), saveForUpdate);

            GameSave newSave = new GameSave();

            await UniTask.RunOnThreadPool(() =>
            {
                newSave.SetPlayer(_saveLocation,
                    PlayerManager.HealthData.CurrentHealth,
                    PlayerManager.StaminaData.CurrentStamina,
                    false,
                    PlayerManager.InventoryData.Inventory,
                    PlayerManager.InventoryData.PrimaryEquipment,
                    PlayerManager.InventoryData.SecondaryEquipment);

                newSave.Difficulty = GameSaveManager.CurrentGameSave.Difficulty;

                newSave.SavePath = GameSaveManager.CurrentGameSave.SavePath;

                newSave.SaveIndex = _saveIndex;

                newSave.SaveName = saveForUpdate.SaveName;
                newSave.SaveTime = saveForUpdate.SaveTime;

                UniTask.SwitchToThreadPool();
            });

            await SaveSerializer.UpdateSaveAsync(newSave, saveForUpdate);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            await SaveSerializer.SerializeObjectAsync(serializer, newSave, GameSaveManager.saveFilePath);

            GameSaveManager.CurrentGameSave = newSave;
        }

    }

}
