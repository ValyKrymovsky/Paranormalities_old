using UnityEngine;
using MyCode.GameData;


namespace MyCode.Managers
{
    public class SettingsManager
    {
        [field: SerializeField] public GameSettingsData SettingsData { get; set; }

    }
}

