using UnityEngine;
using MyBox;

namespace MyCode.PlayerComponents
{
    public class P_Health : MonoBehaviour
    {
        [SerializeField, Separator("Health", true)]
        private float maxHealth = 100;
        [SerializeField]
        private float health;
        [SerializeField]
        private bool dead = false;

        private Coroutine deathScreen;

        
        private void Awake() {
            health = maxHealth;
        }

        private void Update() {
            if (health <= 0)
            {
                dead = true;
                if (deathScreen == null)
                {
                }
                
            }
                
        }

        /// <summary>
        /// </summary>
        /// <returns>max health value</returns>
        public float GetMaxHealth()
        {
            return maxHealth;
        }

        /// <summary>
        /// Sets max health to _maxHealth.
        /// </summary>
        /// <param name="_maxHealth"></param>
        public void SetMaxHealth(float _maxHealth)
        {
            maxHealth = _maxHealth;
        }

        /// <summary>
        /// </summary>
        /// <returns>health value</returns>
        public float GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Sets health to _health.
        /// </summary>
        /// <param name="_health"></param>
        public void SetHealth(float _health)
        {
            health = _health;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if health is below 0, otherwise false</returns>
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

        /// <summary>
        /// Sets dead boolean to _value.
        /// </summary>
        /// <param name="_value"></param>
        public void SetIsDead(bool _value)
        {
            dead = _value;
        }

        /// <summary>
        /// Decreases health by _amount and sets health to 0 if below 0.
        /// </summary>
        /// <param name="_amount"></param>
        public void DealDamage(float _amount)
        {
            health -= _amount;
            if (health < 0)
            {
                health = 0;
            }
        }

        /// <summary>
        /// Increses health by _amount and sets health to maxHealth if bigger than maxHealth value.
        /// </summary>
        /// <param name="_amount"></param>
        public void Heal(float _amount)
        {
            health += _amount;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }
}