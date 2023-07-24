using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using MyCode.Player.Inventory;
using MyCode.Data.Settings;

namespace MyCode.Data.GameSave
{
    [CreateAssetMenu(fileName = "NewGameSaveData", menuName = "DataObjects/GameSave/Save")]

    public class GameSave : ScriptableObject
    {
            [Space]
            [Separator("Player data", true)]
            [Space]

        [Header("Player location")]
        [Space]
        [SerializeField, ReadOnly] private Vector3 _playerLocation;
        [Space]

        [Header("Health")]
        [Space]
        [SerializeField] private float _health;
        [Space]

        [Header("Stamina")]
        [Space]
        [SerializeField] private float _currentStamina;
        [SerializeField] private bool _reachedLimit;
        [Space]

        [Header("Inventory")]
        [Space]
        [SerializeField] private InventoryObject _inventory;
        [SerializeField] private InventoryItem _primaryEquipment;
        [SerializeField] private InventoryItem _secondaryEquipment;

            [Space]
            [Separator("Game Settings")]
            [Space]

        [Header("Game difficulty")]
        [Space]
        [SerializeField] private DifficultyProperties _difficulty;


        //                   //
        // Player properties //
        //                   //


        // Location
        public Vector3 PlayerLocation { get => _playerLocation; set => _playerLocation = value; }

        // Health
        public float Health { get => _health; set => _health = value; }

        // Stamina
        public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }
        public bool ReachedLimit { get => _reachedLimit; set => _reachedLimit = value; }

        // Inventory
        public InventoryObject Inventory { get => _inventory; set => _inventory = value; }
        public InventoryItem PrimaryEquipment { get => _primaryEquipment; set => _primaryEquipment = value; }
        public InventoryItem SecondaryEquipment { get => _secondaryEquipment; set => _secondaryEquipment = value; }


        //               //
        // Game settings //
        //               //


        // Difficulty
        public DifficultyProperties Difficulty { get => _difficulty; set => _difficulty = value; }
    }
}

