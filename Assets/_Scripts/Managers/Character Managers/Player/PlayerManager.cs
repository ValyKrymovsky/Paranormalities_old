using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using MyCode.Data.Settings;
using MyCode.Data.Player;


namespace MyCode.Managers
{
    public class PlayerManager : Manager<PlayerManager>
    {

        public override async UniTask SetUpManager(DifficultyProperties _properties)
        {
            await SetPlayerProperties(_properties);
        }

        private async UniTask SetPlayerProperties(DifficultyProperties _properties)
        {
            _instance.CameraData = _properties.playerCameraData;
            _instance.MovementData = _properties.playerMovementData;
            _instance.StaminaData = _properties.playerStaminaData;
            _instance.HealthData = _properties.playerHealthData;
            _instance.InventoryData = _properties.playerInventoryData;
            _instance.InteractionData = _properties.playerInteractionData;
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

