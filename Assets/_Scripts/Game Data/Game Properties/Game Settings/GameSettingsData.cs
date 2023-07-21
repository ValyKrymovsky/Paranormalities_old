using UnityEngine;
using MyBox;

namespace MyCode.Data.Settings
{
    [CreateAssetMenu(fileName = "NewGameSettingsData", menuName = "DataObjects/GameSettings/Settings")]
    public class GameSettingsData : ScriptableObject
    {
        [Space]
        [Separator("Game Save properties")]
        [Space]

        [Header("Difficulty")]
        [Space]
        [SerializeField, ReadOnly] private DifficultyProperties _difficultyProperties;

        public DifficultyProperties DifficultyProperties { get => _difficultyProperties; set => _difficultyProperties = value; }
    }
}


