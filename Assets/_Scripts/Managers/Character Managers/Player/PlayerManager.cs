using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;


namespace MyCode.Player
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerManager>();
                }
                    

                if (_instance == null)
                {
                    Time.timeScale = 0;
                    Addressables.LoadAssetAsync<GameObject>("Assets/_Scripts/Managers/PlayerManager.prefab").Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            _instance = Instantiate(handle.Result).GetComponent<PlayerManager>();
                            Time.timeScale = 1;
                        }
                        else
                        {
                        }
                    };
                }

                    
                return _instance;
            }
        }

        public static async UniTask<PlayerManager> LoadManager(DifficultyProperties _properties)
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
            }


            if (_instance == null)
            {
                AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("PlayerManager");

                await managerHandle.Task;

                if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
                {
                    _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<PlayerManager>();
                    _instance.CameraData = _properties.playerCameraData;
                    _instance.MovementData = _properties.playerMovementData;
                    _instance.StaminaData = _properties.playerStaminaData;
                    _instance.HealthData = _properties.playerHealthData;
                    _instance.InventoryData = _properties.playerInventoryData;
                    _instance.InteractionData = _properties.playerInteractionData;
                    return _instance;
                }
                return null;
            }

            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this )
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

