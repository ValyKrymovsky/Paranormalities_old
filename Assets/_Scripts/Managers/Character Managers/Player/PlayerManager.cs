using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;
using MyCode.GameData.PlayerData;
using MyCode.GameData.Inventory;
using System;

namespace MyCode.Managers
{
    public class PlayerManager : Manager<PlayerManager>
    {

        public override async UniTask SetUpManager(DifficultyProperties _properties)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                SetPlayerProperties(_properties);
                ResetInventoryEquipment();
            });
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

        public void OverrideInventory(InventoryObject _newInventory)
        {
            _instance.InventoryData.Inventory.inventory = _newInventory.inventory;
            _instance.InventoryData.Inventory.size = _newInventory.size;
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
    }
}

