using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Stamina : MonoBehaviour
{

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] public bool regenerating;

    void Start()
    {
        currentStamina = maxStamina;
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
        if (currentStamina < maxStamina)
        {
            yield return new WaitForSeconds(3);
            regenerating = true;
            currentStamina += amount;

            if (currentStamina > maxStamina)
            {
                regenerating = false;
                currentStamina = maxStamina;
                yield break;
            }
        }
    }

    public bool staminaDepleted()
    {
        if (currentStamina == 0)
        {
            return true;
        }
        else if (regenerating && currentStamina != maxStamina)
        {
            return true;
        }
        else
        {
            regenerating = false;
            return false;
        }
    }
}
