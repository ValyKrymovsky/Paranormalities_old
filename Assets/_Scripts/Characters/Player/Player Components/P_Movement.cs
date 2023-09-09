using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;
using MyCode.Managers;
using MyCode.GameData.Player.Movement;
using System.Collections;

namespace MyCode.PlayerComponents
{
    public class P_Movement : MonoBehaviour
    {
        [Space]
        [Separator("Components")]
        [Space]

        [Header("Components")]
        [Space]
        private CharacterController _characterController;
        [Space]

        private Coroutine _managerCoroutine;

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

        [SerializeField] private InputActionReference _input_WalkValue;
        [SerializeField] private InputActionReference _input_SprintValue;
        [SerializeField] private InputActionReference _input_SneakValue;

        private bool canMove = true;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            _input_WalkValue.action.Enable();
            _input_SprintValue.action.Enable();
            _input_SneakValue.action.Enable();

            _input_WalkValue.action.performed += OnWalkAction;
            _input_SprintValue.action.performed += OnSprintAction;
            _input_SneakValue.action.performed += OnSneakAction;
            _input_WalkValue.action.canceled += StopWalkAction;
            _input_SprintValue.action.canceled += StopSprintAction;
            _input_SneakValue.action.canceled += StopSneakAction;

            PlayerManager.OnPlayerTeleport += (position) =>
            {
                transform.position = position;
            };

            PlayerManager.InventoryData.OnInventoryStatusChange += value => canMove = !value;
        }

        private void OnDisable()
        {
            _input_WalkValue.action.Disable();
            _input_SprintValue.action.Disable();
            _input_SneakValue.action.Disable();

            _input_WalkValue.action.performed -= OnWalkAction;
            _input_SprintValue.action.performed -= OnSprintAction;
            _input_SneakValue.action.performed -= OnSneakAction;
            _input_WalkValue.action.canceled -= StopWalkAction;
            _input_SprintValue.action.canceled -= StopSprintAction;
            _input_SneakValue.action.canceled -= StopSneakAction;

            PlayerManager.InventoryData.OnInventoryStatusChange -= value => canMove = !value;
        }

        void Start()
        {
            _internalSpeedMultiplier = 1;
            _currentVelocity = Vector2.zero;

            PlayerManager.MovementData.MovementState = MovementState.none;
            PlayerManager.MovementData.DirectionToMove = Vector3.zero;

            PlayerManager.MovementData.IsMoving = false;
            PlayerManager.MovementData.IsMovingForward = false;
            PlayerManager.MovementData.SmoothMoveValue = Vector2.zero;

            PlayerManager.MovementData.IsGrounded = false;
        }

        private void Update()
        {
            if (!canMove)
                return;

            PlayerMove();
            ApplyGravity();
            CorrectMovement();

            _characterController.Move(PlayerManager.MovementData.DirectionToMove * PlayerManager.MovementData.WalkSpeed * _internalSpeedMultiplier * Time.deltaTime);
        }

        private void PlayerMove()
        {
            PlayerManager.MovementData.IsMoving = IsPlayerMoving();
            PlayerManager.MovementData.IsMovingForward = IsPlayerMovingForward();
            PlayerManager.MovementData.IsGrounded = IsGrounded();
            UpdateMovementDirection();

            // sets movement sensetivity from controller stick biggest value
            _movementSensetivity = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            _movementSensetivity = _movementSensetivity < .4f ? .4f : _movementSensetivity;

            // increases initial speed over time for smooth movement start
            PlayerManager.MovementData.SmoothMoveValue = Vector2.SmoothDamp(PlayerManager.MovementData.SmoothMoveValue, _moveValue, ref _currentVelocity, PlayerManager.MovementData.SmoothTime);


            PlayerManager.MovementData.DirectionToMove = (PlayerManager.MovementData.SmoothMoveValue.x * transform.right + PlayerManager.MovementData.SmoothMoveValue.y * transform.forward) * _movementSensetivity;

            // clamps player movement to magnitude of 1
            PlayerManager.MovementData.DirectionToMove = Vector3.ClampMagnitude(PlayerManager.MovementData.DirectionToMove, 1);

            
        }

        private void OnWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = value.ReadValue<Vector2>();
            if (_sneakValue != 0)
            {
                PlayerManager.MovementData.MovementState = MovementState.sneak;
            }
            else
            {
                PlayerManager.MovementData.MovementState = MovementState.walk;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext value)
        {
            if (IsPlayerMovingForward())
            {
                if (PlayerManager.StaminaData.UseStaminaSystem)
                {
                    PlayerManager.MovementData.InvokeStartedRunning();
                }
                    
                if (CanSprint())
                {
                    _sprintValue = value.ReadValue<float>();
                    _internalSpeedMultiplier = PlayerManager.MovementData.SprintMultiplier;
                    PlayerManager.MovementData.MovementState = MovementState.sprint;
                }
                
            }
        }

        private void OnSneakAction(InputAction.CallbackContext value)
        {
            if (PlayerManager.MovementData.MovementState != MovementState.sprint)
            {
                if (PlayerManager.StaminaData.UseStaminaSystem)
                {
                    PlayerManager.MovementData.InvokeStartedRunning();
                }
                    
                _sneakValue = value.ReadValue<float>();
                _internalSpeedMultiplier = PlayerManager.MovementData.SneakMultiplier;
                PlayerManager.MovementData.MovementState = MovementState.sneak;
            }
        }

        private void StopWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = Vector2.zero;
            _sprintValue = 0;
            _sneakValue = 0;
            PlayerManager.MovementData.MovementState = MovementState.none;
        }

        private void StopSprintAction(InputAction.CallbackContext value)
        {
            _sprintValue = 0;

            if (PlayerManager.StaminaData.UseStaminaSystem)
                {
                    PlayerManager.MovementData.InvokeStoppedRunning();
                }
            
            if (_sneakValue != 0)
            {
                _internalSpeedMultiplier = PlayerManager.MovementData.SneakMultiplier;
                PlayerManager.MovementData.MovementState = MovementState.sneak;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                PlayerManager.MovementData.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            PlayerManager.MovementData.MovementState = MovementState.none;
            return;
        }

        private void StopSneakAction(InputAction.CallbackContext value)
        {
            _sneakValue = 0;
            if (_sprintValue != 0)
            {
                if (PlayerManager.StaminaData.UseStaminaSystem)
                {
                    PlayerManager.MovementData.InvokeStartedRunning();
                }

                _internalSpeedMultiplier = PlayerManager.MovementData.SprintMultiplier;
                PlayerManager.MovementData.MovementState = MovementState.sprint;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                PlayerManager.MovementData.MovementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            PlayerManager.MovementData.MovementState = MovementState.none;
            return;
        }

        private bool CanSprint()
        {
            if (IsPlayerMovingForward() && PlayerManager.StaminaData.CanSprint)
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
                if (PlayerManager.MovementData.UseCustomGravity)
                    _currentGravityVelocity += (PlayerManager.MovementData.CustomGravity * PlayerManager.MovementData.GravityMultiplier) * Time.deltaTime;
                else
                    _currentGravityVelocity += (Physics.gravity.y * PlayerManager.MovementData.GravityMultiplier) * Time.deltaTime;
            }   
                    

            _gravityForce = _currentGravityVelocity;
            PlayerManager.MovementData.DirectionToMove = new Vector3(PlayerManager.MovementData.DirectionToMove.x, _gravityForce, PlayerManager.MovementData.DirectionToMove.z);
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
                PlayerManager.MovementData.MovementDirection = MovementDirection.none;
            }
            else if (_moveValue.y > 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.forward;
            }
            else if (_moveValue.y < 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.backward;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x > .5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.forward_right;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x < -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.forward_left;
            }
            else if (_moveValue.x > 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.right;
            }
            else if (_moveValue.x < 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.left;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x > .5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.backward_right;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x < -.5))
            {
                PlayerManager.MovementData.MovementDirection = MovementDirection.backward_left;
            }
        }

        private void CorrectMovement()
        {
            if (!CanSprint() && _sprintValue != 0)
            {
                if (_sneakValue != 0)
                {
                    if (PlayerManager.StaminaData.UseStaminaSystem)
                    {
                        PlayerManager.MovementData.InvokeStoppedRunning();
                    }

                    _internalSpeedMultiplier = PlayerManager.MovementData.SneakMultiplier;
                    PlayerManager.MovementData.MovementState = MovementState.sneak;
                }
                else
                {
                    if (PlayerManager.StaminaData.UseStaminaSystem)
                    {
                        PlayerManager.MovementData.InvokeStoppedRunning();
                    }

                    _internalSpeedMultiplier = 1;
                    PlayerManager.MovementData.MovementState = MovementState.walk;
                }
            }
            else if (CanSprint() && _sprintValue != 0)
            {
                if (PlayerManager.StaminaData.UseStaminaSystem)
                {
                    PlayerManager.MovementData.InvokeStartedRunning();
                }

                _internalSpeedMultiplier = PlayerManager.MovementData.SprintMultiplier;
                PlayerManager.MovementData.MovementState = MovementState.sprint;
            }
        }
    }
}
