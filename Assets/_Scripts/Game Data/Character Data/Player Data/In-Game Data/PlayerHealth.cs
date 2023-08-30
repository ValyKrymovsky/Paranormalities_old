using UnityEngine;
using MyBox;
using System;

namespace MyCode.GameData.PlayerData
{
    public class PlayerHealth
    {
        [Space]
        [Separator("Health", true)]
        [Space]

        [Header("Health")]
        [Space]
        private float _maxHealth;
        private float _currentHealth;
        private bool _dead = false;


        public float OriginalMaxHealth { get => _maxHealth; private set => _maxHealth = value; }
        public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        public bool Dead { get => _dead; set => _dead = value; }


        public PlayerHealth(PlayerHealthData _data)
        {
            _maxHealth = _data.MaxHealth;
            _currentHealth = _data.CurrentHealth;
            _dead = _data.Dead;
        }
    }
}

