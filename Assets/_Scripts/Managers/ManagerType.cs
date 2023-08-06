using System;
using UnityEngine;

namespace MyCode.Managers
{
    public enum ManagerTypes
    {
        PlayerManager,
        PlayerSoundManager,
        GameSaveManager,
        PopupManager,
        SettingsManager,
    }

    public class ManagerType : MonoBehaviour
    {
        public ManagerTypes managerType;
    }

}


