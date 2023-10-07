using UnityEngine;
using System;
using MyCode.GameData;
using System.Linq;

namespace MyCode.Managers
{
    public class PlayerManager
    {
        private static readonly object _lock = new object();
        private static PlayerManager _instance;
        public static PlayerManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new PlayerManager();
                }
                return _instance;
            }
        }

        [field: SerializeField] public PlayerCameraData CameraData { get; set; }
        [field: SerializeField] public PlayerMovementData MovementData { get; set; }
        [field: SerializeField] public PlayerInventoryData InventoryData { get; set; }
        [field: SerializeField] public PlayerInteractionData InteractionData { get; set; }

        public event Action<Vector3> OnPlayerTeleport;

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

        public void InvokeOnPlayerTeleport(GameSave _save)
        {
            OnPlayerTeleport?.Invoke(_save.CheckpointLocation.ToVector3());
        }
    }
}

