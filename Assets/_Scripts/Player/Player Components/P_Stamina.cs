using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

namespace MyCode.Player
{
    public class P_Stamina : MonoBehaviour
    {

        [Separator("Stamina", true)]
        [SerializeField]
        private float stamina;
        [SerializeField]
        private float maxStamina;
        private bool regenerating = false;
        private bool regenerate = false;
        private bool depleted = false;
        private bool reachedLimit = false;
        private float limit;

        [Separator("Delays", true)]
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

        /// <summary>
        /// </summary>
        /// <returns>True if stamina equals max stamina, false if not</returns>
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
        
        /// <summary>
        /// </summary>
        /// <returns>Current stamina value</returns>
        public float GetStamina()
        {
            return stamina;
        }

        /// <summary>
        /// Sets current stamina value to _amount.
        /// </summary>
        /// <param name="_amount"></param>
        public void SetStamina(float _amount)
        {
            stamina = _amount;
        }

        /// <summary>
        /// </summary>
        /// <returns>Max stamina rounded down</returns>
        public float GetMaxStamina()
        {
            return Mathf.Floor(maxStamina);
        }

        /// <summary>
        /// Sets max stamina to _amount.
        /// </summary>
        /// <param name="_amount"></param>
        public void SetMaxStamina(float _amount)
        {
            maxStamina = _amount;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if stamina is regenerating, false if not</returns>
        public bool IsRegenerating()
        {
            return regenerating;
        }

        /// <summary>
        /// Sets regenerating bool to _value.
        /// </summary>
        /// <param name="_value"></param>
        public void SetRegenerating(bool _value)
        {
            regenerating = _value;
        }
        
        /// <summary>
        /// </summary>
        /// <returns>True if depleted is true, false if not</returns>
        public bool IsDepleted()
        {
            return depleted;
        }

        /// <summary>
        /// Sets depleted bool to _value.
        /// </summary>
        /// <param name="_value"></param>
        public void SetDepleted(bool _value)
        {
            depleted = _value;
        }
        
        /// <summary>
        /// </summary>
        /// <returns>Limit value</returns>
        public float GetLimit()
        {
            return limit;
        }

        /// <summary>
        /// Sets limit bool to _value.
        /// </summary>
        /// <param name="_value"></param>
        public void SetLimit(float _value)
        {
            limit = _value;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns true if limit was reached, false if not</returns>
        public bool LimitReached()
        {
            return reachedLimit;
        }

        /// <summary>
        /// Sets limitReached bool to _value.
        /// </summary>
        /// <param name="_value"></param>
        public void SetLimitReached(bool _value)
        {
            reachedLimit = _value;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns true if stamina needs regenerating, false if not</returns>
        public bool NeedRegen()
        {
            return regenerate;
        }

        /// <summary>
        /// Depleting stamina by _amount every frame. Sets stamina value to 0 if stamina value > 0
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
        public IEnumerator Deplete(float _amount)
        {
            if (!IsDepleted())
            {
                SetRegenerating(false);
                while (GetStamina() > 0)
                {
                    stamina -= _amount;

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

        /// <summary>
        /// Regenerates stamina by _amount every frame. Has delay, bigger delay if reachedLimit is true.
        /// Sets stamina value to max stamina value if stamina value > max stamina value. 
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns></returns>
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
                stamina += _amount;
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
        
        /// <summary>
        /// Starts deplete stamina coroutine.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns>Deplete coroutine</returns>
        public Coroutine StartDeplete(float _amount)
        {
            Coroutine cr = StartCoroutine(Deplete(_amount));
            return cr;
        }

        /// <summary>
        /// Starts regenerate stamina coroutine.
        /// </summary>
        /// <param name="_amount"></param>
        /// <returns>Regen coroutine</returns>
        public Coroutine StartRegenerate(float _amount)
        {
            Coroutine cr = StartCoroutine(Regenerate(_amount));
            return cr;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if player stamina is depleted, false if not</returns>
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
}
