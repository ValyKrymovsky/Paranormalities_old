using UnityEngine;
using MyCode.GameData;


namespace MyCode.Managers
{
    public class SettingsManager
    {
        private static readonly object _lock = new object();
        private static SettingsManager _instance;
        public static SettingsManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new SettingsManager();
                }
                return _instance;
            }
        }
        [field: SerializeField] public GameSettingsData SettingsData { get; set; }

    }
}

