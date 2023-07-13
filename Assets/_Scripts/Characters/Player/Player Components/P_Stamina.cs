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
            instanceRef.Stamina.CurrentStamina = instanceRef.Stamina.MaxStamina;
            instanceRef.Stamina.ReachedLimit = false;
            instanceRef.Stamina.CanSprint = true;
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
                if (instanceRef.Stamina.RegenCoroutine != null && !instanceRef.Stamina.ReachedLimit)
                {
                    StopCoroutine(instanceRef.Stamina.RegenCoroutine);
                    instanceRef.Stamina.RegenCoroutine = null;
                }

                if (instanceRef.Stamina.DrainCoroutine == null)
                {
                    instanceRef.Stamina.DrainCoroutine = StartCoroutine(DrainStamina());
                }
            }
        }

        private void RegenStaminaEvent()
        {
            if (!IsStaminaFull())
            {
                if (instanceRef.Stamina.DrainCoroutine != null)
                {
                    StopCoroutine(instanceRef.Stamina.DrainCoroutine);
                    instanceRef.Stamina.DrainCoroutine = null;
                }

                if (instanceRef.Stamina.RegenCoroutine == null)
                {
                    instanceRef.Stamina.RegenCoroutine = StartCoroutine(RegenStamina(instanceRef.Stamina.RegenDelay));
                }
            }
        }

        private IEnumerator DrainStamina()
        {
            if (!instanceRef.Stamina.ReachedLimit)
            {
                while (!IsStaminaDepleted())
                {
                    instanceRef.Stamina.CurrentStamina -= instanceRef.Stamina.DepletionValue;

                    if (instanceRef.Stamina.CurrentStamina < instanceRef.Stamina.Limit)
                    {
                        instanceRef.Stamina.ReachedLimit = true;
                        instanceRef.Stamina.CanSprint = false;
                    }

                    yield return null;
                }
                instanceRef.Stamina.CurrentStamina = 0;
                yield break;
            }
        }

        private IEnumerator RegenStamina(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            while (!IsStaminaFull())
            {
                instanceRef.Stamina.CurrentStamina += instanceRef.Stamina.RegenValue;
                yield return null;
            }
            instanceRef.Stamina.CurrentStamina = instanceRef.Stamina.MaxStamina;
            instanceRef.Stamina.ReachedLimit = false;
            instanceRef.Stamina.CanSprint = true;
            yield break;
        }

        private bool IsStaminaDepleted()
        {
            if (instanceRef.Stamina.CurrentStamina <= 0)
                return true;

            return false;
        }

        private bool IsStaminaFull()
        {
            if (instanceRef.Stamina.CurrentStamina >= instanceRef.Stamina.MaxStamina)
                return true;

            return false;
        }
    }
}
