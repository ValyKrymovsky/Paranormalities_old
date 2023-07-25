using System;
using MyCode.Data.Player;
using Newtonsoft.Json;

namespace MyCode.Data.Settings
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

