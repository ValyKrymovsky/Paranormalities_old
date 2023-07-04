using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using System;
using System.Collections;

namespace MyCode.Player
{
    public class P_Movement : MonoBehaviour
    {
        [Space]
        [Separator("Components")]
        [Space]

        [Header("Components")]
        [Space]
        private CharacterController ch_controller;
        private PlayerManager instanceRef;
        [Space]

        // Movement
        private Vector2 _moveValue;
        private float _sprintValue;
        private float _sneakValue;

        private float internalSpeedMultiplier;
        private float movementSensetivity;

        private Vector2 currentVelocity;

        // Gravity
        private float currentGravityVelocity;
        private float gravityForce;

        private void Awake()
        {
            ch_controller = GetComponent<CharacterController>();
            instanceRef = PlayerManager.Instance;
        }

        private void OnEnable()
        {
            instanceRef.Movement.WalkValueInput.action.performed += OnWalkAction;
            instanceRef.Movement.SprintValueInput.action.performed += OnSprintAction;
            instanceRef.Movement.SneakValueInput.action.performed += OnSneakAction;
            instanceRef.Movement.WalkValueInput.action.canceled += StopWalkAction;
            instanceRef.Movement.SprintValueInput.action.canceled += StopSprintAction;
            instanceRef.Movement.SneakValueInput.action.canceled += StopSneakAction;
        }

        private void OnDisable()
        {
            instanceRef.Movement.WalkValueInput.action.performed -= OnWalkAction;
            instanceRef.Movement.SprintValueInput.action.performed -= OnSprintAction;
            instanceRef.Movement.SneakValueInput.action.performed -= OnSneakAction;
            instanceRef.Movement.WalkValueInput.action.canceled -=StopWalkAction;
            instanceRef.Movement.SprintValueInput.action.canceled -= StopSprintAction;
            instanceRef.Movement.SneakValueInput.action.canceled -= StopSneakAction;
        }

        void Start()
        {
            internalSpeedMultiplier = 1;
            currentVelocity = Vector2.zero;

            instanceRef.Movement.MovementState = movementState.none;
            instanceRef.Movement.DirectionToMove = Vector3.zero;

            instanceRef.Movement.IsMoving = false;
            instanceRef.Movement.IsMovingForward = false;
            instanceRef.Movement.SmoothMoveValue = Vector2.zero;

            instanceRef.Movement.IsGrounded = false;
        }

        private void Update()
        {
            PlayerMove();
            ApplyGravity();
            ch_controller.Move(instanceRef.Movement.DirectionToMove * instanceRef.Movement.WalkSpeed * internalSpeedMultiplier * Time.deltaTime);
        }

        private void PlayerMove()
        {
            instanceRef.Movement.IsMoving = IsPlayerMoving();
            instanceRef.Movement.IsMovingForward = IsPlayerMovingForward();
            instanceRef.Movement.IsGrounded = IsGrounded();

            // sets movement sensetivity from controller stick biggest value
            movementSensetivity = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            movementSensetivity = movementSensetivity < .4f ? .4f : movementSensetivity;

            // increases initial speed over time for smooth movement start
            instanceRef.Movement.SmoothMoveValue = Vector2.SmoothDamp(instanceRef.Movement.SmoothMoveValue, _moveValue, ref currentVelocity, instanceRef.Movement.SmoothTime);


            instanceRef.Movement.DirectionToMove = (instanceRef.Movement.SmoothMoveValue.x * transform.right + instanceRef.Movement.SmoothMoveValue.y * transform.forward) * movementSensetivity;

            // clamps player movement to magnitude of 1
            instanceRef.Movement.DirectionToMove = Vector3.ClampMagnitude(instanceRef.Movement.DirectionToMove, 1);

            
        }

        private void OnWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = value.ReadValue<Vector2>();
            instanceRef.Movement.MovementState = movementState.walk;
        }

        private void OnSprintAction(InputAction.CallbackContext value)
        {
            if (IsPlayerMoving())
            {
                _sprintValue = value.ReadValue<float>();
                internalSpeedMultiplier = instanceRef.Movement.SprintMultiplier;
                instanceRef.Movement.MovementState = movementState.sprint;
            }
        }

        private void OnSneakAction(InputAction.CallbackContext value)
        {
            if (instanceRef.Movement.MovementState != movementState.sprint)
            {
                _sneakValue = value.ReadValue<float>();
                internalSpeedMultiplier = instanceRef.Movement.SneakMultiplier;
                instanceRef.Movement.MovementState = movementState.sneak;
            }
        }

        private void StopWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = Vector2.zero;
            _sprintValue = 0;
            _sneakValue = 0;
            instanceRef.Movement.MovementState = movementState.none;
        }

        private void StopSprintAction(InputAction.CallbackContext value)
        {
            _sprintValue = 0;
            if (_sneakValue != 0)
            {
                internalSpeedMultiplier = instanceRef.Movement.SneakMultiplier;
                instanceRef.Movement.MovementState = movementState.sneak;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                internalSpeedMultiplier = 1;
                instanceRef.Movement.MovementState = movementState.walk;
                return;
            }

            internalSpeedMultiplier = 1;
            instanceRef.Movement.MovementState = movementState.none;
            return;
        }

        private void StopSneakAction(InputAction.CallbackContext value)
        {
            _sneakValue = 0;
            if (_sprintValue != 0)
            {
                internalSpeedMultiplier = instanceRef.Movement.SprintMultiplier;
                instanceRef.Movement.MovementState = movementState.sprint;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                internalSpeedMultiplier = 1;
                instanceRef.Movement.MovementState = movementState.walk;
                return;
            }

            internalSpeedMultiplier = 1;
            instanceRef.Movement.MovementState = movementState.none;
            return;
        }

        private void ApplyGravity()
        {
            if (IsGrounded())
            {
                currentGravityVelocity = -1.0f;
            }
            else
            {
                if (instanceRef.Movement.UseCustomGravity)
                    currentGravityVelocity += (instanceRef.Movement.CustomGravity * instanceRef.Movement.GravityMultiplier) * Time.deltaTime;
                else
                    currentGravityVelocity += (Physics.gravity.y * instanceRef.Movement.GravityMultiplier) * Time.deltaTime;
            }   
                    

            gravityForce = currentGravityVelocity;
            instanceRef.Movement.DirectionToMove = new Vector3(instanceRef.Movement.DirectionToMove.x, gravityForce, instanceRef.Movement.DirectionToMove.z);
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

        /*

        /// <summary>
        /// Sets up necessary parameters for OnWalkActioning.
        /// </summary>
        public void OnWalkAction()
        {
            OnWalkActioning = true;
            sneaking = false;
            sprinting = false;

            CheckStaminaState();
            if (move_value.y < 0)
            {
                internalSpeedMultiplier = .75f;
            }
            else
            {
                internalSpeedMultiplier = 1;
            }
            
        }
        
        /// <summary>
        /// Sets up necessary parameters for sprinting. If useStaminaSystem is true, handles logic for depleting and regenerating stamina.
        /// </summary>
        public void Sprint()
        {
            sprinting = true;
            sneaking = false;
            OnWalkActioning = false;

            if (useStaminaSystem)
            {
                if ((!p_stamina.IsDepleted() && !p_stamina.LimitReached()) && depleteCoroutine == null)
                {
                    if (regenCoroutine != null)
                    {
                        StopCoroutine(regenCoroutine);
                        regenCoroutine = null;
                    }
                    depleteCoroutine = p_stamina.StartDeplete(depletionValue);
                    internalSpeedMultiplier = sprintMultiplier;
                }

                if (p_stamina.IsDepleted() && depleteCoroutine != null)
                {
                    depleteCoroutine = null;
                    regenCoroutine = p_stamina.StartRegenerate(regenValue);
                    internalSpeedMultiplier = 1;
                }
            }
            else
            {
                internalSpeedMultiplier = sprintMultiplier;
            }
        }

        /// <summary>
        /// Sets up necessary parameters from sneaking.
        /// </summary>
        public void Sneak()
        {
            sneaking = true;
            sprinting = false;
            OnWalkActioning = false;

            CheckStaminaState();
            internalSpeedMultiplier = sneakMultiplier;
        }

        /// <summary>
        /// Starts regeneration coroutine if necessary and stops and sets deplete coroutine to null if it is not already null.
        /// </summary>
        private void CheckStaminaState()
        {
            if (depleteCoroutine != null)
            {
                StopCoroutine(depleteCoroutine);
                depleteCoroutine = null;
            }

            if ((p_stamina.IsDepleted() || p_stamina.NeedRegen()) && regenCoroutine == null)
            {
                regenCoroutine = p_stamina.StartRegenerate(regenValue);
            }

            if (p_stamina.IsFull() && regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
                regenCoroutine = null;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>current players move movementState</returns>
        public movementState GetMovementAction()
        {
            return movementState;
        }

        public movementDirection GetMovementDirection()
        {
            return movementDirection;
        }

        private void UpdateMovementAction()
        {
            isMovingForward = move_value.y > 0 ? true : false;

            if (((!isMovingForward && isMoving) || (isMovingForward && (sprint_value == 0 || p_stamina.IsDepleted()))) && sneak_value == 0)
            {
                movementState = movementState.OnWalkAction;
            }
            else if (isMovingForward && sprint_value != 0 && sneak_value == 0 && ((!p_stamina.IsDepleted() && !p_stamina.LimitReached())))
            {
                movementState = movementState.sprint;
            }
            else if (isMoving && sneak_value != 0)
            {
                movementState = movementState.sneak;
            }
            else if (!isMoving)
            {
                OnWalkActioning = false;
                sprinting = false;
                sneaking = false;
                movementState = movementState.none;
            }
        }

        private void UpdateMovementDirection()
        {
            if (move_value == Vector2.zero)
            {
            movementDirection = movementDirection.none; 
            }
            else if (move_value.y > 0 && (move_value.x < .5 && move_value.x > -.5))
            {
                movementDirection = movementDirection.forward;
            }
            else if (move_value.y < 0 && (move_value.x < .5 && move_value.x > -.5))
            {
                movementDirection = movementDirection.backward;
            }
            else if ((move_value.y > .5) && (move_value.x > .5))
            {
                movementDirection = movementDirection.forward_right;
            }
            else if ((move_value.y > .5) && (move_value.x < -.5))
            {
                movementDirection = movementDirection.forward_left;
            }
            else if (move_value.x > 0 && (move_value.y < .5 && move_value.y > -.5))
            {
                movementDirection = movementDirection.right;
            }
            else if (move_value.x < 0 && (move_value.y < .5 && move_value.y > -.5))
            {
                movementDirection = movementDirection.left;
            }
            else if ((move_value.y < -.5) && (move_value.x > .5))
            {
                movementDirection = movementDirection.backward_right;
            }
            else if ((move_value.y < -.5) && (move_value.x < -.5))
            {
                movementDirection = movementDirection.backward_left;
            }
        }

        public bool IsPlayerMoving()
        {
            if (move_value.x != 0 || move_value.y != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Applies gravity to player movement. Sets y coordinate of player movement to gravity force.
        /// If player is grounded, sets gravity to -1, otherwise adds negative force each frame.
        /// </summary>
        private void ApplyGravity()
        {
            if (IsGrounded())
            {
                velocity = -1.0f;
            }
            else
            {
                velocity += (gravity * gravityMultiplier) * Time.deltaTime;
            }

            gravityForce = velocity;
            directionToMove.y = gravityForce;
        }

        /// <summary>
        /// Checks if player is grounded by casting ray to the ground from players position.
        /// </summary>
        /// <returns>True if player is grounded, false if not</returns>
        public bool IsGrounded()
        {
            if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hitInfo, .1f))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
            return isGrounded;
        }
        */
    }
}
