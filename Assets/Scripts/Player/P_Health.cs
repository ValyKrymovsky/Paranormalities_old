using UnityEngine;
using MyBox;

public class P_Health : MonoBehaviour
{
    [SerializeField, Separator("Health", true)]
    private float maxHealth = 100;
    [SerializeField]
    private float health;
    [SerializeField]
    private bool dead = false;
    
    private void Awake() {
        health = maxHealth;
    }

    private void Update() {
        if (health <= 0)
        {
            dead = true;
        }
            
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float _health)
    {
        health = _health;
    }

    public bool IsDead()
    {
        if (health <= 0)
        {
            dead = true;
            return true;
        }
        dead = false;
        return dead;
    }

    public void SetIsDead(bool _value)
    {
        dead = _value;
    }

    public void DealDamage(float _amount)
    {
        health -= _amount;
        if (health < 0)
        {
            health = 0;
        }
    }

    public void Heal(float _amount)
    {
        health += _amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
