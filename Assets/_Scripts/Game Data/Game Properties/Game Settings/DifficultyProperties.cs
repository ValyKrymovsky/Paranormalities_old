using System;
using Newtonsoft.Json;

namespace MyCode.GameData
{
    public enum Difficulty
    {
        easy, normal, hard, insane, nightmare,
    }

    [Serializable]
    public struct DifficultyProperties
    {
        public Difficulty difficulty;
        [JsonIgnore] public PlayerCameraData playerCameraData;
        [JsonIgnore] public PlayerInventoryData playerInventoryData;
        [JsonIgnore] public PlayerMovementData playerMovementData;
        [JsonIgnore] public PlayerInteractionData playerInteractionData;
    }
}

