using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using MyCode.Data.Settings;


namespace MyCode.Managers
{
    public class SettingsManager : Manager<SettingsManager>
    {

        public override async UniTask SetUpManager(DifficultyProperties _properties)
        {
            await SetSettingsProperties(_properties);
        }

        private async UniTask SetSettingsProperties(DifficultyProperties _properties)
        {
            _instance.SettingsData.DifficultyProperties = _properties;
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

        [field: SerializeField] public GameDifficultyData SettingsData { get; set; }

    }
}

