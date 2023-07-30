using UnityEngine;
using MyBox;

namespace MyCode.GameData.PlayerData
{
    [CreateAssetMenu(fileName = "NewHealthData", menuName = "DataObjects/Player/Health")]
    public class PlayerHealthData : ScriptableObject
    {
        [Space]
        [Separator("Health", true)]
        [Space]

        [Header("Health")]
        [Space]
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _currentHealth;
        [SerializeField] private bool _dead = false;


        public float OriginalMaxHealth { get => _maxHealth; private set => _maxHealth = value; }
        public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        public bool Dead { get => _dead; set => _dead = value; }

    }
}

