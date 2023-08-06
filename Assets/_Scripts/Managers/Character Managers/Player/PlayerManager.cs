using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;
using MyCode.GameData.PlayerData;
using MyCode.GameData.Inventory;
using System;
using MyCode.GameData.GameSave;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyCode.Managers
{
    public class PlayerManager : Manager<PlayerManager>
    {

        public override async UniTask SetUpNewManager(DifficultyProperties _properties)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                SetPlayerProperties(_properties);
                ResetInventoryEquipment();
            });

            AsyncOperationHandle loadHandle = Addressables.LoadAssetAsync<ScriptableObject>("EmptyInventory");
            await loadHandle.Task;

            InventoryData.Inventory = Instantiate(loadHandle.Result as InventoryObject);
            InventoryData.Inventory.name = "PlayerInventory";
        }

        public override async UniTask SetUpExistingManager(GameSave _save)
        {
            Loader = GameObject.FindAnyObjectByType<ManagerLoader>();

            AsyncOperationHandle loadHandle = Addressables.LoadAssetAsync<ScriptableObject>("EmptyInventory");
            await loadHandle.Task;

            

            await UniTask.RunOnThreadPool(() =>
            {
                SetPlayerProperties(Loader.DifficultyProperties.Where(diff => diff.difficulty == _save.Difficulty.difficulty).First());

                _instance.HealthData.CurrentHealth = _save.Health;

                _instance.StaminaData.CurrentStamina = _save.CurrentStamina;
                _instance.StaminaData.ReachedLimit = _save.ReachedLimit;

                SetInventoryEquipment(_save.PrimaryEquipment, _save.SecondaryEquipment);
            });

            _instance.InventoryData.Inventory = Instantiate(loadHandle.Result as InventoryObject);
            _instance.InventoryData.Inventory.inventory = _save.Inventory.inventory;
            InventoryData.Inventory.name = "PlayerInventory";


        }

        private void SetPlayerProperties(DifficultyProperties _properties)
        {
            _instance.CameraData = _properties.playerCameraData;
            _instance.MovementData = _properties.playerMovementData;
            _instance.StaminaData = _properties.playerStaminaData;
            _instance.HealthData = _properties.playerHealthData;
            _instance.InventoryData = _properties.playerInventoryData;
            _instance.InteractionData = _properties.playerInteractionData;
        }

        private void ResetInventoryEquipment()
        {
            InventoryData.PrimaryEquipment = InventoryItem.empty;
            InventoryData.SecondaryEquipment = InventoryItem.empty;
        }

        private void SetInventoryEquipment(InventoryItem _primary, InventoryItem _secondary)
        {
            InventoryData.PrimaryEquipment = _primary;
            InventoryData.SecondaryEquipment = _secondary;
        }

        public void OverrideInventory(InventoryObject _newInventory)
        {
            _instance.InventoryData.Inventory.inventory = _newInventory.inventory;
            _instance.InventoryData.Inventory.size = _newInventory.size;
        }

        public static void InvokeOnPlayerTeleport(GameSave _save)
        {
            OnPlayerTeleport?.Invoke(new Vector3(_save.CheckpointLocation[0], _save.CheckpointLocation[1], _save.CheckpointLocation[2]));
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        [field: SerializeField] public PlayerCameraData CameraData { get; set; }
        [field: SerializeField] public PlayerMovementData MovementData { get; set; }
        [field: SerializeField] public PlayerHealthData HealthData { get; set; }
        [field: SerializeField] public PlayerStaminaData StaminaData { get; set; }
        [field: SerializeField] public PlayerInventoryData InventoryData { get; set; }
        [field: SerializeField] public PlayerInteractionData InteractionData { get; set; }

        public static event Action<Vector3> OnPlayerTeleport;

        public ManagerLoader Loader { get; set; }
    }
}

