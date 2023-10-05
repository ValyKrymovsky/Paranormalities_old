using UnityEngine;
using System;
using MyCode.GameData;
using System.Linq;

namespace MyCode.Managers
{
    public class PlayerManager
    {
        [field: SerializeField] public static PlayerCameraData CameraData { get; set; }
        [field: SerializeField] public static PlayerMovementData MovementData { get; set; }
        [field: SerializeField] public static PlayerInventoryData InventoryData { get; set; }
        [field: SerializeField] public static PlayerInteractionData InteractionData { get; set; }

        public static event Action<Vector3> OnPlayerTeleport;

        public ManagerLoader Loader { get; set; }

        public void SetUpExistingManager(GameSave _save)
        {
            Loader = GameObject.FindAnyObjectByType<ManagerLoader>();

            SetPlayerProperties(Loader.DifficultyProperties.Where(diff => diff.difficulty == _save.GameDifficulty).First());
        }

        public static Item[] ReplaceAllItems(Item[] _newItems)
        {
            Item[] newArray = new Item[_newItems.Length];
            for (int i = 0; i < _newItems.Length; i++)
            {
                newArray[i] = Item.GetItem(_newItems[i].id);
            }

            return newArray;
        }

        public void SetPlayerProperties(DifficultyProperties _properties)
        {
            CameraData = _properties.playerCameraData;
            MovementData = _properties.playerMovementData;
            InventoryData = _properties.playerInventoryData;

            InventoryData.Inventory = new Inventory(15);

            InventoryData.Inventory.FillWithNull();

            InteractionData = _properties.playerInteractionData;
        }

        public static void InvokeOnPlayerTeleport(GameSave _save)
        {
            OnPlayerTeleport?.Invoke(_save.CheckpointLocation.ToVector3());
        }
    }
}

