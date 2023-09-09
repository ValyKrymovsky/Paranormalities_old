using System.Collections;
using UnityEngine;
using MyCode.Managers;
using MyBox;

namespace MyCode.PlayerComponents
{
    public class P_Stamina : MonoBehaviour
    {
        private void Awake()
        {
        }

        void Start()
        {
            PlayerManager.StaminaData.CurrentStamina = PlayerManager.StaminaData.MaxStamina;
            PlayerManager.StaminaData.ReachedLimit = false;
            PlayerManager.StaminaData.CanSprint = true;
        }

        private void OnEnable()
        {
            PlayerManager.MovementData.StartedRunning += DrainStaminaEvent;
            PlayerManager.MovementData.StoppedRunning += RegenStaminaEvent;
        }

        private void OnDisable()
        {
            PlayerManager.MovementData.StartedRunning -= DrainStaminaEvent;
            PlayerManager.MovementData.StoppedRunning -= RegenStaminaEvent;
        }

        private void DrainStaminaEvent()
        {
            if (!IsStaminaDepleted())
            {
                if (PlayerManager.StaminaData.RegenCoroutine != null && !PlayerManager.StaminaData.ReachedLimit)
                {
                    StopCoroutine(PlayerManager.StaminaData.RegenCoroutine);
                    PlayerManager.StaminaData.RegenCoroutine = null;
                }

                if (PlayerManager.StaminaData.DrainCoroutine == null)
                {
                    PlayerManager.StaminaData.DrainCoroutine = StartCoroutine(DrainStamina());
                }
            }
        }

        private void RegenStaminaEvent()
        {
            if (!IsStaminaFull())
            {
                if (PlayerManager.StaminaData.DrainCoroutine != null)
                {
                    StopCoroutine(PlayerManager.StaminaData.DrainCoroutine);
                    PlayerManager.StaminaData.DrainCoroutine = null;
                }

                if (PlayerManager.StaminaData.RegenCoroutine == null)
                {
                    PlayerManager.StaminaData.RegenCoroutine = StartCoroutine(RegenStamina(PlayerManager.StaminaData.RegenDelay));
                }
            }
        }

        private IEnumerator DrainStamina()
        {
            if (!PlayerManager.StaminaData.ReachedLimit)
            {
                while (!IsStaminaDepleted())
                {
                    PlayerManager.StaminaData.CurrentStamina -= PlayerManager.StaminaData.DepletionValue;

                    if (PlayerManager.StaminaData.CurrentStamina < PlayerManager.StaminaData.Limit)
                    {
                        PlayerManager.StaminaData.ReachedLimit = true;
                        PlayerManager.StaminaData.CanSprint = false;
                    }

                    yield return null;
                }
                PlayerManager.StaminaData.CurrentStamina = 0;
                yield break;
            }
        }

        private IEnumerator RegenStamina(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            while (!IsStaminaFull())
            {
                PlayerManager.StaminaData.CurrentStamina += PlayerManager.StaminaData.RegenValue;
                yield return null;
            }
            PlayerManager.StaminaData.CurrentStamina = PlayerManager.StaminaData.MaxStamina;
            PlayerManager.StaminaData.ReachedLimit = false;
            PlayerManager.StaminaData.CanSprint = true;
            yield break;
        }

        private bool IsStaminaDepleted()
        {
            if (PlayerManager.StaminaData.CurrentStamina <= 0)
                return true;

            return false;
        }

        private bool IsStaminaFull()
        {
            if (PlayerManager.StaminaData.CurrentStamina >= PlayerManager.StaminaData.MaxStamina)
                return true;

            return false;
        }
    }
}
