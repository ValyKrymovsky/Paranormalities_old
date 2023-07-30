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
            if (_saveIndex.CompareTo(GameSaveManager.Instance.CurrentGameSave.SaveIndex) < 1) return;


            GameSave saveForUpdate = new GameSave();

            SaveSerializer.DeserializeGameSave(await SaveSerializer.ReadSaveFileAsync(GameSaveManager.Instance.saveFilePath), saveForUpdate);

            GameSave newSave = new GameSave();

            await UniTask.RunOnThreadPool(() =>
            {
                newSave.SetPlayer(_saveLocation,
                    PlayerManager.Instance.HealthData.CurrentHealth,
                    PlayerManager.Instance.StaminaData.CurrentStamina,
                    false,
                    PlayerManager.Instance.InventoryData.Inventory,
                    PlayerManager.Instance.InventoryData.PrimaryEquipment,
                    PlayerManager.Instance.InventoryData.SecondaryEquipment);

                newSave.SetDifficulty(GameSaveManager.Instance.CurrentGameSave.Difficulty);

                newSave.SetSaveIndex(_saveIndex);

                UniTask.SwitchToThreadPool();
            });

            await SaveSerializer.UpdateSaveAsync(newSave, saveForUpdate);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags" });

            await SaveSerializer.SerializeObjectAsync(serializer, newSave, GameSaveManager.Instance.saveFilePath);

            GameSaveManager.Instance.CurrentGameSave = newSave;
        }

    }

}
