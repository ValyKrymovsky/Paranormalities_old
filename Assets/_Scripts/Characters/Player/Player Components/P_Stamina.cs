using System.Collections;
using UnityEngine;

using MyBox;

namespace MyCode.Player
{
    public class P_Stamina : MonoBehaviour
    {
        private PlayerManager _pm;

        private void Awake()
        {
            _pm = PlayerManager.Instance;
        }

        void Start()
        {
            _pm.StaminaData.CurrentStamina = _pm.StaminaData.MaxStamina;
            _pm.StaminaData.ReachedLimit = false;
            _pm.StaminaData.CanSprint = true;
        }

        private void OnEnable()
        {
            _pm.MovementData.StartedRunning += DrainStaminaEvent;
            _pm.MovementData.StoppedRunning += RegenStaminaEvent;
        }

        private void OnDisable()
        {
            _pm.MovementData.StartedRunning -= DrainStaminaEvent;
            _pm.MovementData.StoppedRunning -= RegenStaminaEvent;
        }

        private void DrainStaminaEvent()
        {
            if (!IsStaminaDepleted())
            {
                if (_pm.StaminaData.RegenCoroutine != null && !_pm.StaminaData.ReachedLimit)
                {
                    StopCoroutine(_pm.StaminaData.RegenCoroutine);
                    _pm.StaminaData.RegenCoroutine = null;
                }

                if (_pm.StaminaData.DrainCoroutine == null)
                {
                    _pm.StaminaData.DrainCoroutine = StartCoroutine(DrainStamina());
                }
            }
        }

        private void RegenStaminaEvent()
        {
            if (!IsStaminaFull())
            {
                if (_pm.StaminaData.DrainCoroutine != null)
                {
                    StopCoroutine(_pm.StaminaData.DrainCoroutine);
                    _pm.StaminaData.DrainCoroutine = null;
                }

                if (_pm.StaminaData.RegenCoroutine == null)
                {
                    _pm.StaminaData.RegenCoroutine = StartCoroutine(RegenStamina(_pm.StaminaData.RegenDelay));
                }
            }
        }

        private IEnumerator DrainStamina()
        {
            if (!_pm.StaminaData.ReachedLimit)
            {
                while (!IsStaminaDepleted())
                {
                    _pm.StaminaData.CurrentStamina -= _pm.StaminaData.DepletionValue;

                    if (_pm.StaminaData.CurrentStamina < _pm.StaminaData.Limit)
                    {
                        _pm.StaminaData.ReachedLimit = true;
                        _pm.StaminaData.CanSprint = false;
                    }

                    yield return null;
                }
                _pm.StaminaData.CurrentStamina = 0;
                yield break;
            }
        }

        private IEnumerator RegenStamina(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            while (!IsStaminaFull())
            {
                _pm.StaminaData.CurrentStamina += _pm.StaminaData.RegenValue;
                yield return null;
            }
            _pm.StaminaData.CurrentStamina = _pm.StaminaData.MaxStamina;
            _pm.StaminaData.ReachedLimit = false;
            _pm.StaminaData.CanSprint = true;
            yield break;
        }

        private bool IsStaminaDepleted()
        {
            if (_pm.StaminaData.CurrentStamina <= 0)
                return true;

            return false;
        }

        private bool IsStaminaFull()
        {
            if (_pm.StaminaData.CurrentStamina >= _pm.StaminaData.MaxStamina)
                return true;

            return false;
        }
    }
}
