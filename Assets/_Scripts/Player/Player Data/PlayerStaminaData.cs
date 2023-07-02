using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
    [SerializeField] private bool useStaminaSystem;
    [SerializeField] private float currentStamina;
    [SerializeField] private float originalMaxStamina;
    [SerializeField] private float temporaryMaxStamina;
    [SerializeField] private float depletionValue;
    [SerializeField] private float regenValue;
    private float limit;
    private bool isRegenerating = false;
    private bool needRegeneration = false;
    private bool isDepleted = false;
    private bool reachedLimit = false;
    
    [Space]
    [Separator("Delays", true)]
    [Space]

    [Header("Delays")]
    [Space]
    [SerializeField] private float regenDelay;

    private Coroutine regenCoroutine;
    private Coroutine depleteCoroutine;


    public bool UseStaminaSystem { get => useStaminaSystem; set => useStaminaSystem = value; }
    public float CurrentStamina { get => currentStamina; set => currentStamina = value; }
    public float OriginalMaxStamina { get => originalMaxStamina; set => originalMaxStamina = value; }
    public float TemporaryMaxStamina { get => temporaryMaxStamina; set => temporaryMaxStamina = value; }
    public float DepletionValue { get => depletionValue; set => depletionValue = value; }
    public float RegenValue { get => regenValue; set => regenValue = value; }
    public float Limit { get => limit; set => limit = value; }
    public bool IsRegenerating { get => isRegenerating; set => isRegenerating = value; }
    public bool NeedRegenerate { get => needRegeneration; set => needRegeneration = value; }
    public bool IsDepleted { get => isDepleted; set => isDepleted = value; }
    public bool ReachedLimit { get => reachedLimit; set => reachedLimit = value; }
    public float RegenDelay { get => regenDelay; set => regenDelay = value; }
    public Coroutine RegenCoroutine { get => regenCoroutine; set => regenCoroutine = value; }
    public Coroutine DepleteCoroutine { get => depleteCoroutine; set => depleteCoroutine = value; }
}
