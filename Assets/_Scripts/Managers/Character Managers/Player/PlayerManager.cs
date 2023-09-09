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
        [field: SerializeField] public static PlayerCamera CameraData { get; set; }
        [field: SerializeField] public static PlayerMovement MovementData { get; set; }
        [field: SerializeField] public static PlayerHealth HealthData { get; set; }
        [field: SerializeField] public static PlayerStamina StaminaData { get; set; }
        [field: SerializeField] public static PlayerInventory InventoryData { get; set; }
        [field: SerializeField] public static PlayerInteraction InteractionData { get; set; }

        public static event Action<Vector3> OnPlayerTeleport;

        public ManagerLoader Loader { get; set; }

        public override async UniTask SetUpNewManager(DifficultyProperties _properties)
        {
            SetPlayerProperties(_properties);
        }

        public override async UniTask SetUpExistingManager(GameSave _save)
        {
            Loader = GameObject.FindAnyObjectByType<ManagerLoader>();

            AsyncOperationHandle loadHandle = Addressables.LoadAssetAsync<ScriptableObject>("EmptyInventory");
            await loadHandle.Task;

            await UniTask.RunOnThreadPool(() =>
            {
                SetPlayerProperties(Loader.DifficultyProperties.Where(diff => diff.difficulty == _save.Difficulty.difficulty).First());

                HealthData.CurrentHealth = _save.Health;

                StaminaData.CurrentStamina = _save.CurrentStamina;
                StaminaData.ReachedLimit = _save.ReachedLimit;

                SetInventoryEquipment(_save.PrimaryEquipment, _save.SecondaryEquipment);
            });


        }

        public static InventoryItem[] ReplaceAllItems(InventoryItem[] _newItems)
        {
            InventoryItem[] newArray = new InventoryItem[_newItems.Length];
            for (int i = 0; i < _newItems.Length; i++)
            {
                newArray[i] = InventoryItemStorage.GetItem(_newItems[i].ItemId);
            }

            return newArray;
        }

        private void SetPlayerProperties(DifficultyProperties _properties)
        {
            CameraData = new PlayerCamera(_properties.playerCameraData);
            MovementData = new PlayerMovement(_properties.playerMovementData);
            StaminaData = new PlayerStamina(_properties.playerStaminaData);
            HealthData = new PlayerHealth(_properties.playerHealthData);
            InventoryData = new PlayerInventory(_properties.playerInventoryData);
            InventoryData.PrimaryEquipment = InventoryItem.empty;
            InventoryData.SecondaryEquipment = InventoryItem.empty;
            InteractionData = new PlayerInteraction(_properties.playerInteractionData);
        }

        private void SetInventoryEquipment(InventoryItem _primary, InventoryItem _secondary)
        {
            InventoryData.PrimaryEquipment = _primary;
            InventoryData.SecondaryEquipment = _secondary;
        }

        public static void InvokeOnPlayerTeleport(GameSave _save)
        {
            OnPlayerTeleport?.Invoke(new Vector3(_save.CheckpointLocation[0], _save.CheckpointLocation[1], _save.CheckpointLocation[2]));
        }
    }
}

