using System;
using MyCode.Data.Player;

namespace MyCode.Data.Settings
{
    [Serializable]
    public struct DifficultyProperties
    {
        public Difficulty difficulty;
        public PlayerCameraData playerCameraData;
        public PlayerHealthData playerHealthData;
        public PlayerInventoryData playerInventoryData;
        public PlayerMovementData playerMovementData;
        public PlayerStaminaData playerStaminaData;
        public PlayerInteractionData playerInteractionData;
    }
}

