using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;


namespace MyCode.Managers
{
    public class SettingsManager : Manager<SettingsManager>
    {
        [field: SerializeField] public GameSettingsData SettingsData { get; set; }

    }
}

