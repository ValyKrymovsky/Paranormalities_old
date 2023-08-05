using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;


namespace MyCode.Managers
{
    public class SettingsManager : Manager<SettingsManager>
    {
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
}

