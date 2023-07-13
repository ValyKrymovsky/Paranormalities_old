using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;


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

        public static async Task<PlayerManager> LoadManager(DifficultyProperties _properties)
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
                    _instance.Camera = _properties.playerCameraData;
                    _instance.Movement = _properties.playerMovementData;
                    _instance.Stamina = _properties.playerStaminaData;
                    _instance.Health = _properties.playerHealthData;
                    _instance.Inventory = _properties.playerInventoryData;
                    _instance.Interaction = _properties.playerInteractionData;
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

        [field: SerializeField] public PlayerCameraData Camera { get; set; }
        [field: SerializeField] public PlayerMovementData Movement { get; set; }
        [field: SerializeField] public PlayerHealthData Health { get; set; }
        [field: SerializeField] public PlayerStaminaData Stamina { get; set; }
        [field: SerializeField] public PlayerInventoryData Inventory { get; set; }
        [field: SerializeField] public PlayerInteractionData Interaction { get; set; }
    }
}

