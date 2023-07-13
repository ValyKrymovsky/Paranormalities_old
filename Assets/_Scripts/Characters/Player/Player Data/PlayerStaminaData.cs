using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewStaminaData", menuName = "DataObjects/Player/Stamina")]
public class PlayerStaminaData : ScriptableObject
{   
        [Space]
        [Separator("Stamina", true)]
        [Space]

    [Header("Stamina Properties")]
    [Space]
    [SerializeField] private bool _useStaminaSystem;
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _depletionValue;
    [SerializeField] private float _regenValue;
    [SerializeField] private float _limit;
    [SerializeField, ReadOnly] private bool _reachedLimit;
    [SerializeField, ReadOnly] private bool _canSprint;
    
        [Space]
        [Separator("Delays", true)]
        [Space]

    [Header("Delays")]
    [Space]
    [SerializeField] private float _regenDelay;

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
}
