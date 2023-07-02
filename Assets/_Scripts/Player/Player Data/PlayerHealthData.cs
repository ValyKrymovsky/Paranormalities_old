using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewHealthData", menuName = "DataObjects/Player/Health")]
public class PlayerHealthData : ScriptableObject
{   
    [Space]
    [Separator("Health", true)]
    [Space]
    
    [Header("Health")]
    [Space]
    [SerializeField] private float originalMaxHealth;
    [SerializeField] private float temporaryMaxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool dead = false;

    private Coroutine deathScreen;


    public float OriginalMaxHealth { get => originalMaxHealth; private set => originalMaxHealth = value; }
    public float TemporaryMaxHealth { get => temporaryMaxHealth; set => temporaryMaxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public bool Dead { get => dead; set => dead = value; }
    public Coroutine DeathScreen { get => deathScreen; set => deathScreen = value; }
    
}
