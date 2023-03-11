using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Stamina : MonoBehaviour
{

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] public bool isRegenerating;
    [SerializeField] public bool regenIsNeeded;
    [SerializeField] public bool staminaWasDepleted;
    [SerializeField] private float regenDelay;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update() 
    {
         regenIsNeeded = regenNeeded();
    }

    public float getStamina()
    {
        return currentStamina;
    }

    public float setStamina(float amount)
    {
        currentStamina = amount;
        return currentStamina;
    }

    public float getMaxStamina()
    {
        return maxStamina;
    }

    public float setMaxStamina(float amount)
    {
        maxStamina = amount;
        return maxStamina;
    }

    public float getCurrentStamina()
    {
        return currentStamina;
    }

    public float setCurrentStamina(float amount)
    {
        currentStamina = amount;
        return currentStamina;
    }

    public float depleteStamina(float amount)
    {
        if (currentStamina <= maxStamina && currentStamina > 0)
        {
            currentStamina -= amount;
        }

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }

        return currentStamina;
    }

    public IEnumerator regenerateStamina(float amount)
    {
        yield return new WaitForSeconds(regenDelay);
        while (currentStamina <= maxStamina)
        {
            currentStamina += amount;
            yield return null;
        }

        isRegenerating = false;
        staminaWasDepleted = false;
        currentStamina = maxStamina;
    }

    public Coroutine startRegen(float regenValue)
    {
        isRegenerating = true;
        return StartCoroutine(regenerateStamina(regenValue));
    }

    public bool regenNeeded()
    {
        if (currentStamina != maxStamina)
        {
            return true;
        }
        return false;
    }

    public bool staminaRegenerating()
    {
        return isRegenerating;
    }
    public bool staminaDepleted()
    {
        if (currentStamina <= 0)
        {
            staminaWasDepleted = true;
            return true;
        }
        else if (isRegenerating && currentStamina != maxStamina && staminaWasDepleted)
        {
            return true;
        }
        else
        {
            staminaWasDepleted = false;
            return false;
        }
    }
}
