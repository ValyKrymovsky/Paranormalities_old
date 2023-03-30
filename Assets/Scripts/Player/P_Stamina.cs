using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class P_Stamina : MonoBehaviour
{

    [Header("Stamina")]
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private bool regenerating;
    [SerializeField] private bool regenerate;
    [SerializeField] private bool depleted;
    [SerializeField] private bool reachedLimit;
    [SerializeField] private float limit;

    [Header("Delays")]
    [SerializeField] private float regenDelay;

    void Start()
    {
        stamina = maxStamina;
        SetDepleted(false);
    }

    void Update() 
    {
         if (Mathf.Round(GetStamina()) != GetMaxStamina())
         {
            regenerate = true;
         }
         else
         {
            regenerate = false;
         }
    }

    public bool IsFull()
    {
        if (GetStamina() == GetMaxStamina())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public float GetStamina()
    {
        return stamina;
    }

    public void SetStamina(float _amount)
    {
        stamina = _amount;
    }

    public float GetMaxStamina()
    {
        return Mathf.Floor(maxStamina);
    }

    public void SetMaxStamina(float _amount)
    {
        maxStamina = _amount;
    }

    public bool IsRegenerating()
    {
        return regenerating;
    }

    public void SetRegenerating(bool _value)
    {
        regenerating = _value;
    }
    
    public bool IsDepleted()
    {
        return depleted;
    }

    public void SetDepleted(bool _value)
    {
        depleted = _value;
    }
    
    public float GetLimit()
    {
        return limit;
    }

    public void SetLimit(float _value)
    {
        limit = _value;
    }

    public bool LimitReached()
    {
        return reachedLimit;
    }

    public void SetLimitReached(bool _value)
    {
        reachedLimit = _value;
    }

    public bool NeedRegen()
    {
        return regenerate;
    }

    public IEnumerator Deplete(float _amount)
    {
        if (!IsDepleted())
        {
            SetRegenerating(false);
            while (GetStamina() > 0)
            {
                stamina -= Mathf.Round(_amount);

                if (GetStamina() < GetLimit())
                {
                    SetLimitReached(true);
                }

                yield return null;
            }

            if (GetStamina() <= 0)
            {
                SetStamina(0);
                SetDepleted(true);
            }
        } 
    }

    public IEnumerator Regenerate(float _amount)
    {
        if (LimitReached())
        {
            yield return new WaitForSeconds(regenDelay + 1);
        }
        else
        {
            yield return new WaitForSeconds(regenDelay);
        }
        
        SetRegenerating(true);
        while (GetStamina() <= GetMaxStamina())
        {
            stamina += Mathf.Round(_amount);
            yield return null;
        }

        if (GetStamina() >= GetMaxStamina())
        {
            SetStamina(GetMaxStamina());
            SetDepleted(false);
            SetRegenerating(false);
            SetLimitReached(false);
        }
        
    }
    
    public Coroutine StartDeplete(float _amount)
    {
        Coroutine cr = StartCoroutine(Deplete(_amount));
        return cr;
    }

    public Coroutine StartRegenerate(float _amount)
    {
        Coroutine cr = StartCoroutine(Regenerate(_amount));
        return cr;
    }

    public bool StaminaDepleted()
    {
        if (stamina <= 0)
        {
            reachedLimit = true;
            return true;
        }
        else if (regenerating && stamina != maxStamina && reachedLimit)
        {
            return true;
        }
        else
        {
            reachedLimit = false;
            return false;
        }
    }

}
