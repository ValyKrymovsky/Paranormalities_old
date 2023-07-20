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

        // MovementData
        private Vector2 _moveValue;
        private float _sprintValue;
        private float _sneakValue;

        private float _internalSpeedMultiplier;
        private float _movementSensetivity;

        private Vector2 _currentVelocity;

        // Gravity
        private float _currentGravityVelocity;
        private float _gravityForce;

        

        private bool canMove = true;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _pm = PlayerManager.Instance;
        }

        private void OnEnable()
        {
            _pm.MovementData.WalkValueInput.action.performed += OnWalkAction;
            _pm.MovementData.SprintValueInput.action.performed += OnSprintAction;
            _pm.MovementData.SneakValueInput.action.performed += OnSneakAction;
            _pm.MovementData.WalkValueInput.action.canceled += StopWalkAction;
            _pm.MovementData.SprintValueInput.action.canceled += StopSprintAction;
            _pm.MovementData.SneakValueInput.action.canceled += StopSneakAction;

            _pm.InventoryData.OnInventoryStatusChange += value => canMove = !value;
        }

        private void OnDisable()
        {
            _pm.MovementData.WalkValueInput.action.performed -= OnWalkAction;
            _pm.MovementData.SprintValueInput.action.performed -= OnSprintAction;
            _pm.MovementData.SneakValueInput.action.performed -= OnSneakAction;
            _pm.MovementData.WalkValueInput.action.canceled -=StopWalkAction;
            _pm.MovementData.SprintValueInput.action.canceled -= StopSprintAction;
            _pm.MovementData.SneakValueInput.action.canceled -= StopSneakAction;

            _pm.InventoryData.OnInventoryStatusChange -= value => canMove = !value;
        }

        void Start()
        {
            _internalSpeedMultiplier = 1;
            _currentVelocity = Vector2.zero;

            _pm.MovementData.MovementState = MovementState.none;
            _pm.MovementData.DirectionToMove = Vector3.zero;

            _pm.MovementData.IsMoving = false;
            _pm.MovementData.IsMovingForward = false;
            _pm.MovementData.SmoothMoveValue = Vector2.zero;

            _pm.MovementData.IsGrounded = false;
        }

        private void Update()
        {
            if (!canMove)
                return;

            PlayerMove();
            ApplyGravity();
            CorrectMovement();

            _characterController.Move(_pm.MovementData.DirectionToMove * _pm.MovementData.WalkSpeed * _internalSpeedMultiplier * Time.deltaTime);
        }

        private void PlayerMove()
        {
            _pm.MovementData.IsMoving = IsPlayerMoving();
            _pm.MovementData.IsMovingForward = IsPlayerMovingForward();
            _pm.MovementData.IsGrounded = IsGrounded();
            UpdateMovementDirection();

            // sets movement sensetivity from controller stick biggest value
            _movementSensetivity = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            _movementSensetivity = _movementSensetivity < .4f ? .4f : _movementSensetivity;

            // increases initial speed over time for smooth movement start
            _pm.MovementData.SmoothMoveValue = Vector2.SmoothDamp(_pm.MovementData.SmoothMoveValue, _moveValue, ref _currentVelocity, _pm.MovementData.SmoothTime);


            _pm.MovementData.DirectionToMove = (_pm.MovementData.SmoothMoveValue.x * transform.right + _pm.MovementData.SmoothMoveValue.y * transform.forward) * _movementSensetivity;

            // clamps player movement to magnitude of 1
            _pm.MovementData.DirectionToMove = Vector3.ClampMagnitude(_pm.MovementData.DirectionToMove, 1);

            
        }

        private void OnWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = value.ReadValue<Vector2>();
            if (_sneakValue != 0)
            {
                _pm.MovementData.MovementState = MovementState.sneak;
            }
            else
            {
                _pm.MovementData.MovementState = MovementState.walk;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext value)
        {
            if (IsPlayerMovingForward())
            {
                if (_pm.StaminaData.UseStaminaSystem)
                {
                    _pm.MovementData.InvokeStartedRunning();
                }
                    
                if (CanSprint())
                {
                    _sprintValue = value.ReadValue<float>();
                    _internalSpeedMultiplier = _pm.MovementData.SprintMultiplier;
                    _pm.MovementData.MovementState = MovementState.sprint;
                }
                
            }
        }

        private void OnSneakAction(InputAction.CallbackContext value)
        {
            if (_pm.MovementData.MovementState != MovementState.sprint)
            {
                if (_pm.StaminaData.UseStaminaSystem)
                {
                    _pm.MovementData.InvokeStartedRunning();
                    Debug.Log("Stamina system on");
                }
                    
                _sneakValue = value.ReadValue<float>();
                _internalSpeedMultiplier = _pm.MovementData.SneakMultiplier;
                _pm.MovementData.MovementState = MovementState.sneak;
            }
        }

        private void StopWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = Vector2.zero;
            _sprintValue = 0;
            _sneakValue = 0;
            _pm.MovementData.MovementState = MovementState.none;
        }

        private void StopSprintAction(InputAction.CallbackContext value)
        {
            _sprintValue = 0;

            if (_pm.StaminaData.UseStaminaSystem)
                {
                    _pm.MovementData.InvokeStoppedRunning();
                    Debug.Log("Stamina system on");
                }
            
            if (_sneakValue != 0)
            {
                _internalSpeedMultiplier = _pm.MovementData.SneakMultiplier;
                _pm.MovementData.MovementState = MovementState.sneak;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _pm.MovementData.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _pm.MovementData.MovementState = MovementState.none;
            return;
        }

        private void StopSneakAction(InputAction.CallbackContext value)
        {
            _sneakValue = 0;
            if (_sprintValue != 0)
            {
                if (_pm.StaminaData.UseStaminaSystem)
                {
                    _pm.MovementData.InvokeStartedRunning();
                    Debug.Log("Stamina system on");
                }

                _internalSpeedMultiplier = _pm.MovementData.SprintMultiplier;
                _pm.MovementData.MovementState = MovementState.sprint;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _pm.MovementData.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _pm.MovementData.MovementState = MovementState.none;
            return;
        }

        private bool CanSprint()
        {
            if (IsPlayerMovingForward() && _pm.StaminaData.CanSprint)
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
                if (_pm.MovementData.UseCustomGravity)
                    _currentGravityVelocity += (_pm.MovementData.CustomGravity * _pm.MovementData.GravityMultiplier) * Time.deltaTime;
                else
                    _currentGravityVelocity += (Physics.gravity.y * _pm.MovementData.GravityMultiplier) * Time.deltaTime;
            }   
                    

            _gravityForce = _currentGravityVelocity;
            _pm.MovementData.DirectionToMove = new Vector3(_pm.MovementData.DirectionToMove.x, _gravityForce, _pm.MovementData.DirectionToMove.z);
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
                _pm.MovementData.MovementDirection = MovementDirection.none;
            }
            else if (_moveValue.y > 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.forward;
            }
            else if (_moveValue.y < 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.backward;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x > .5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.forward_right;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x < -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.forward_left;
            }
            else if (_moveValue.x > 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.right;
            }
            else if (_moveValue.x < 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.left;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x > .5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.backward_right;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x < -.5))
            {
                _pm.MovementData.MovementDirection = MovementDirection.backward_left;
            }
        }

        private void CorrectMovement()
        {
            if (!CanSprint() && _sprintValue != 0)
            {
                if (_sneakValue != 0)
                {
                    if (_pm.StaminaData.UseStaminaSystem)
                    {
                        _pm.MovementData.InvokeStoppedRunning();
                        Debug.Log("Stamina system on");
                    }

                    _internalSpeedMultiplier = _pm.MovementData.SneakMultiplier;
                    _pm.MovementData.MovementState = MovementState.sneak;
                }
                else
                {
                    if (_pm.StaminaData.UseStaminaSystem)
                    {
                        _pm.MovementData.InvokeStoppedRunning();
                        Debug.Log("Stamina system on");
                    }

                    _internalSpeedMultiplier = 1;
                    _pm.MovementData.MovementState = MovementState.walk;
                }
            }
            else if (CanSprint() && _sprintValue != 0)
            {
                if (_pm.StaminaData.UseStaminaSystem)
                {
                    _pm.MovementData.InvokeStartedRunning();
                }

                _internalSpeedMultiplier = _pm.MovementData.SprintMultiplier;
                _pm.MovementData.MovementState = MovementState.sprint;
            }
        }
    }
}
