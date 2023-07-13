using System;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;


namespace MyCode.Player
{
    public class P_Movement : MonoBehaviour
    {
        [Space]
        [Separator("Components")]
        [Space]

        [Header("Components")]
        [Space]
        private CharacterController _characterController;
        private PlayerManager _pm;
        [Space]

        // Movement
        private Vector2 _moveValue;
        private float _sprintValue;
        private float _sneakValue;

        private float _internalSpeedMultiplier;
        private float _movementSensetivity;

        private Vector2 _currentVelocity;

        // Gravity
        private float _currentGravityVelocity;
        private float _gravityForce;

        // Events
        public static event Action startedRunning;
        public static event Action stoppedRunning;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _pm = PlayerManager.Instance;
        }

        private void OnEnable()
        {
            _pm.Movement.WalkValueInput.action.performed += OnWalkAction;
            _pm.Movement.SprintValueInput.action.performed += OnSprintAction;
            _pm.Movement.SneakValueInput.action.performed += OnSneakAction;
            _pm.Movement.WalkValueInput.action.canceled += StopWalkAction;
            _pm.Movement.SprintValueInput.action.canceled += StopSprintAction;
            _pm.Movement.SneakValueInput.action.canceled += StopSneakAction;
        }

        private void OnDisable()
        {
            _pm.Movement.WalkValueInput.action.performed -= OnWalkAction;
            _pm.Movement.SprintValueInput.action.performed -= OnSprintAction;
            _pm.Movement.SneakValueInput.action.performed -= OnSneakAction;
            _pm.Movement.WalkValueInput.action.canceled -=StopWalkAction;
            _pm.Movement.SprintValueInput.action.canceled -= StopSprintAction;
            _pm.Movement.SneakValueInput.action.canceled -= StopSneakAction;
        }

        void Start()
        {
            _internalSpeedMultiplier = 1;
            _currentVelocity = Vector2.zero;

            _pm.Movement.MovementState = MovementState.none;
            _pm.Movement.DirectionToMove = Vector3.zero;

            _pm.Movement.IsMoving = false;
            _pm.Movement.IsMovingForward = false;
            _pm.Movement.SmoothMoveValue = Vector2.zero;

            _pm.Movement.IsGrounded = false;
        }

        private void Update()
        {
            PlayerMove();
            ApplyGravity();
            CorrectMovement();

            _characterController.Move(_pm.Movement.DirectionToMove * _pm.Movement.WalkSpeed * _internalSpeedMultiplier * Time.deltaTime);
        }

        private void PlayerMove()
        {
            _pm.Movement.IsMoving = IsPlayerMoving();
            _pm.Movement.IsMovingForward = IsPlayerMovingForward();
            _pm.Movement.IsGrounded = IsGrounded();
            UpdateMovementDirection();

            // sets movement sensetivity from controller stick biggest value
            _movementSensetivity = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            _movementSensetivity = _movementSensetivity < .4f ? .4f : _movementSensetivity;

            // increases initial speed over time for smooth movement start
            _pm.Movement.SmoothMoveValue = Vector2.SmoothDamp(_pm.Movement.SmoothMoveValue, _moveValue, ref _currentVelocity, _pm.Movement.SmoothTime);


            _pm.Movement.DirectionToMove = (_pm.Movement.SmoothMoveValue.x * transform.right + _pm.Movement.SmoothMoveValue.y * transform.forward) * _movementSensetivity;

            // clamps player movement to magnitude of 1
            _pm.Movement.DirectionToMove = Vector3.ClampMagnitude(_pm.Movement.DirectionToMove, 1);

            
        }

        private void OnWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = value.ReadValue<Vector2>();
            if (_sneakValue != 0)
            {
                _pm.Movement.MovementState = MovementState.sneak;
            }
            else
            {
                _pm.Movement.MovementState = MovementState.walk;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext value)
        {
            if (IsPlayerMovingForward())
            {
                if (_pm.Stamina.UseStaminaSystem)
                    startedRunning?.Invoke();

                if (CanSprint())
                {
                    _sprintValue = value.ReadValue<float>();
                    _internalSpeedMultiplier = _pm.Movement.SprintMultiplier;
                    _pm.Movement.MovementState = MovementState.sprint;
                }
                
            }
        }

        private void OnSneakAction(InputAction.CallbackContext value)
        {
            if (_pm.Movement.MovementState != MovementState.sprint)
            {
                _sneakValue = value.ReadValue<float>();
                _internalSpeedMultiplier = _pm.Movement.SneakMultiplier;
                _pm.Movement.MovementState = MovementState.sneak;
            }
        }

        private void StopWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = Vector2.zero;
            _sprintValue = 0;
            _sneakValue = 0;
            _pm.Movement.MovementState = MovementState.none;
        }

        private void StopSprintAction(InputAction.CallbackContext value)
        {
            _sprintValue = 0;

            if (_pm.Stamina.UseStaminaSystem)
                stoppedRunning?.Invoke();
            
            if (_sneakValue != 0)
            {
                _internalSpeedMultiplier = _pm.Movement.SneakMultiplier;
                _pm.Movement.MovementState = MovementState.sneak;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _pm.Movement.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _pm.Movement.MovementState = MovementState.none;
            return;
        }

        private void StopSneakAction(InputAction.CallbackContext value)
        {
            _sneakValue = 0;
            if (_sprintValue != 0)
            {
                _internalSpeedMultiplier = _pm.Movement.SprintMultiplier;
                _pm.Movement.MovementState = MovementState.sprint;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _pm.Movement.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _pm.Movement.MovementState = MovementState.none;
            return;
        }

        private bool CanSprint()
        {
            if (IsPlayerMovingForward() && _pm.Stamina.CanSprint)
            {
                return true;
            }
            return false;
        }

        private void ApplyGravity()
        {
            if (IsGrounded())
            {
                _currentGravityVelocity = -1.0f;
            }
            else
            {
                if (_pm.Movement.UseCustomGravity)
                    _currentGravityVelocity += (_pm.Movement.CustomGravity * _pm.Movement.GravityMultiplier) * Time.deltaTime;
                else
                    _currentGravityVelocity += (Physics.gravity.y * _pm.Movement.GravityMultiplier) * Time.deltaTime;
            }   
                    

            _gravityForce = _currentGravityVelocity;
            _pm.Movement.DirectionToMove = new Vector3(_pm.Movement.DirectionToMove.x, _gravityForce, _pm.Movement.DirectionToMove.z);
        }

        public bool IsGrounded()
        {
            if (Physics.Raycast(transform.position, transform.up * -1, .1f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPlayerMoving()
        {
            if (!_moveValue.Equals(Vector2.zero))
            {
                return true;
            }

            return false;
        }

        private bool IsPlayerMovingForward()
        {
            if (IsPlayerMoving())
            {
                if (_moveValue.y > 0 && _moveValue.y >= _moveValue.x)
                    return true;
                else
                    return false;
            }
            return false;
        }

        private void UpdateMovementDirection()
        {
            if (_moveValue == Vector2.zero)
            {
                _pm.Movement.MovementDirection = MovementDirection.none;
            }
            else if (_moveValue.y > 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.forward;
            }
            else if (_moveValue.y < 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.backward;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x > .5))
            {
                _pm.Movement.MovementDirection = MovementDirection.forward_right;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x < -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.forward_left;
            }
            else if (_moveValue.x > 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.right;
            }
            else if (_moveValue.x < 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.left;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x > .5))
            {
                _pm.Movement.MovementDirection = MovementDirection.backward_right;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x < -.5))
            {
                _pm.Movement.MovementDirection = MovementDirection.backward_left;
            }
        }

        private void CorrectMovement()
        {
            if (!CanSprint() && _sprintValue != 0)
            {
                if (_sneakValue != 0)
                {
                    _internalSpeedMultiplier = _pm.Movement.SneakMultiplier;
                    _pm.Movement.MovementState = MovementState.sneak;
                    stoppedRunning?.Invoke();
                }
                else
                {
                    _internalSpeedMultiplier = 1;
                    _pm.Movement.MovementState = MovementState.walk;
                    stoppedRunning?.Invoke();
                }
            }
            else if (CanSprint() && _sprintValue != 0)
            {
                _internalSpeedMultiplier = _pm.Movement.SprintMultiplier;
                _pm.Movement.MovementState = MovementState.sprint;
                startedRunning?.Invoke();
            }
        }
    

    }
}
