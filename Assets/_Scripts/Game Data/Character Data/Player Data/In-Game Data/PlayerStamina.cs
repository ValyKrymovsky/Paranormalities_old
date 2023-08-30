using UnityEngine;
using MyBox;


namespace MyCode.GameData.PlayerData
{
    public class PlayerStamina
    {
        [Space]
        [Separator("Stamina", true)]
        [Space]

        [Header("Stamina Properties")]
        [Space]
        private bool _useStaminaSystem;
        private float _currentStamina;
        private float _maxStamina;
        private float _depletionValue;
        private float _regenValue;
        private float _limit;
        private bool _reachedLimit;
        private bool _canSprint;

        [Space]
        [Separator("Delays", true)]
        [Space]

        [Header("Delays")]
        [Space]
        private float _regenDelay;

        private Coroutine _regenCoroutine;
        private Coroutine _drainCoroutine;


        public bool UseStaminaSystem { get => _useStaminaSystem; set => _useStaminaSystem = value; }
        public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }
        public float MaxStamina { get => _maxStamina; set => _maxStamina = value; }
        public float DepletionValue { get => _depletionValue; set => _depletionValue = value; }
        public float RegenValue { get => _regenValue; set => _regenValue = value; }
        public float Limit { get => _limit; set => _limit = value; }
        public bool ReachedLimit { get => _reachedLimit; set => _reachedLimit = value; }
        public bool CanSprint { get => _canSprint; set => _canSprint = value; }
        public float RegenDelay { get => _regenDelay; set => _regenDelay = value; }
        public Coroutine RegenCoroutine { get => _regenCoroutine; set => _regenCoroutine = value; }
        public Coroutine DrainCoroutine { get => _drainCoroutine; set => _drainCoroutine = value; }

        public PlayerStamina(PlayerStaminaData _data)
        {
            _useStaminaSystem = _data.UseStaminaSystem;
            _currentStamina = _data.CurrentStamina;
            _maxStamina = _data.MaxStamina;
            _depletionValue = _data.DepletionValue;
            _regenValue = _data.RegenValue;
            _limit = _data.Limit;
            _reachedLimit = _data.ReachedLimit;
            _canSprint = _data.CanSprint;

            _regenDelay = _data.RegenDelay;

            _regenCoroutine = null;
            _drainCoroutine = null;
        }
    }
}
