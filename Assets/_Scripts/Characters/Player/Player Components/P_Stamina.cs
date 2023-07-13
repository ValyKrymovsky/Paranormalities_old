using System.Collections;
using UnityEngine;

using MyBox;

namespace MyCode.Player
{
    public class P_Stamina : MonoBehaviour
    {
        private PlayerManager instanceRef;

        private void Awake()
        {
            instanceRef = PlayerManager.Instance;
        }

        void Start()
        {
            instanceRef.StaminaData.CurrentStamina = instanceRef.StaminaData.MaxStamina;
            instanceRef.StaminaData.ReachedLimit = false;
            instanceRef.StaminaData.CanSprint = true;
        }

        private void OnEnable()
        {
            P_Movement.startedRunning += DrainStaminaEvent;
            P_Movement.stoppedRunning += RegenStaminaEvent;
        }

        private void OnDisable()
        {
            P_Movement.startedRunning -= DrainStaminaEvent;
            P_Movement.stoppedRunning -= RegenStaminaEvent;
        }

        private void DrainStaminaEvent()
        {
            if (!IsStaminaDepleted())
            {
                if (instanceRef.StaminaData.RegenCoroutine != null && !instanceRef.StaminaData.ReachedLimit)
                {
                    StopCoroutine(instanceRef.StaminaData.RegenCoroutine);
                    instanceRef.StaminaData.RegenCoroutine = null;
                }

                if (instanceRef.StaminaData.DrainCoroutine == null)
                {
                    instanceRef.StaminaData.DrainCoroutine = StartCoroutine(DrainStamina());
                }
            }
        }

        private void RegenStaminaEvent()
        {
            if (!IsStaminaFull())
            {
                if (instanceRef.StaminaData.DrainCoroutine != null)
                {
                    StopCoroutine(instanceRef.StaminaData.DrainCoroutine);
                    instanceRef.StaminaData.DrainCoroutine = null;
                }

                if (instanceRef.StaminaData.RegenCoroutine == null)
                {
                    instanceRef.StaminaData.RegenCoroutine = StartCoroutine(RegenStamina(instanceRef.StaminaData.RegenDelay));
                }
            }
        }

        private IEnumerator DrainStamina()
        {
            if (!instanceRef.StaminaData.ReachedLimit)
            {
                while (!IsStaminaDepleted())
                {
                    instanceRef.StaminaData.CurrentStamina -= instanceRef.StaminaData.DepletionValue;

                    if (instanceRef.StaminaData.CurrentStamina < instanceRef.StaminaData.Limit)
                    {
                        instanceRef.StaminaData.ReachedLimit = true;
                        instanceRef.StaminaData.CanSprint = false;
                    }

                    yield return null;
                }
                instanceRef.StaminaData.CurrentStamina = 0;
                yield break;
            }
        }

        private IEnumerator RegenStamina(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            while (!IsStaminaFull())
            {
                instanceRef.StaminaData.CurrentStamina += instanceRef.StaminaData.RegenValue;
                yield return null;
            }
            instanceRef.StaminaData.CurrentStamina = instanceRef.StaminaData.MaxStamina;
            instanceRef.StaminaData.ReachedLimit = false;
            instanceRef.StaminaData.CanSprint = true;
            yield break;
        }

        private bool IsStaminaDepleted()
        {
            if (instanceRef.StaminaData.CurrentStamina <= 0)
                return true;

            return false;
        }

        private bool IsStaminaFull()
        {
            if (instanceRef.StaminaData.CurrentStamina >= instanceRef.StaminaData.MaxStamina)
                return true;

            return false;
        }
    }
}
