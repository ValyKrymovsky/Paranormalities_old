using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using MyBox;

public class P_Health : MonoBehaviour
{
    [SerializeField, Separator("Health", true)]
    private float maxHealth = 100;
    [SerializeField]
    private float health;
    [SerializeField]
    private bool dead = false;
    
    [SerializeField, Separator("Death Screen", true)]
    private UIDocument deathScreen;
    private VisualElement root;
    private Coroutine deathScreenCoroutine;
    
    private void Awake() {
        health = maxHealth;
        deathScreen = GameObject.Find("DeathScreen").GetComponent<UIDocument>();
        root = deathScreen.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    private void Update() {
        if (health <= 0)
        {
            dead = true;
            deathScreenCoroutine = StartCoroutine(StartDeathScreen());
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

    private IEnumerator StartDeathScreen()
    {
        root.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
}
