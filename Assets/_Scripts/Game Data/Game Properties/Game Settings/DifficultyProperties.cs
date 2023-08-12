using System;
using MyCode.GameData.PlayerData;
using Newtonsoft.Json;

namespace MyCode.GameData.GameSettings
{
    [Serializable]
    public struct DifficultyProperties
    {
        public Difficulty difficulty;
        [JsonIgnore] public PlayerCameraData playerCameraData;
        [JsonIgnore] public PlayerHealthData playerHealthData;
        [JsonIgnore] public PlayerInventoryData playerInventoryData;
        [JsonIgnore] public PlayerMovementData playerMovementData;
        [JsonIgnore] public PlayerStaminaData playerStaminaData;
        [JsonIgnore] public PlayerInteractionData playerInteractionData;
    }
}

