using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using MyCode.Player.Inventory;

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
    }
}

