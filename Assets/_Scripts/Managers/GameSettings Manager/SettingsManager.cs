using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;


public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
            }


            if (_instance == null)
            {
                AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("SettingsManager");

                if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
                {
                    _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<SettingsManager>();
                    return _instance;
                }
                return null;
            }

            return _instance;
        }
    }

    public static async UniTask<SettingsManager> LoadManager(DifficultyProperties _properties)
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<SettingsManager>();
        }


        if (_instance == null)
        {
            AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("SettingsManager");

            await managerHandle.Task;

            if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
            {
                _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<SettingsManager>();
                _instance.SettingsData.DifficultyProperties = _properties;
                return _instance;
            }
            return null;
        }

        return _instance;
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

    [field: SerializeField] public GameSettingsData SettingsData { get; set; }
}

