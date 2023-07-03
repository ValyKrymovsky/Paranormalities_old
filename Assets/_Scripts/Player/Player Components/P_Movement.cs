using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using System;

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
        [Space]

        private float internalSpeedMultiplier;
        private float movementSensetivity;

        private Vector2 currentVelocity;

        private void Awake()
        {
            ch_controller = GetComponent<CharacterController>();
        }

        void Start()
        {
            internalSpeedMultiplier = 1;

            PlayerManager.Instance.Movement.MovementState = movementState.none;

            PlayerManager.Instance.Movement.WalkValueInput.action.performed += ctx => Walk(ctx.ReadValue<Vector3>());
            PlayerManager.Instance.Movement.SprintValueInput.action.performed += ctx => Sprint(ctx.ReadValue<float>());
            PlayerManager.Instance.Movement.SneakValueInput.action.performed += ctx => Sneak(ctx.ReadValue<float>());
        }

        private void Walk(Vector3 _moveValue)
        {
            // sets movement sensetivity from controller stick biggest value
            movementSensetivity = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            movementSensetivity = movementSensetivity < .4f ? .4f : movementSensetivity;

            Debug.Log(movementSensetivity);

            /*
            // increases initial speed over time for smooth movement start
            smoothMoveValue = Vector2.SmoothDamp(smoothMoveValue, move_value, ref currentVelocity, smoothTime);

            directionToMove = (smoothMoveValue.x * transform.right + smoothMoveValue.y * transform.forward) * movementSensetivity;
            finalDirectionToMove = (move_value.x * transform.right + move_value.y * transform.forward) * movementSensetivity;

            // clamps player movement to magnitude of 1
            directionToMove = Vector3.ClampMagnitude(directionToMove, 1);
            finalDirectionToMove = Vector3.ClampMagnitude(finalDirectionToMove, 1);

            // Only for other classes
            playerMoveDirection = directionToMove;

            isMoving = IsPlayerMoving();
            */
        }

        
        private void Sprint(float v)
        {
            throw new NotImplementedException();
        }

        private void Sneak(float v)
        {
            throw new NotImplementedException();
        }

        /*
        private void Update()
        {
            if (canMove)
            {
                PlayerMove();
            }
            else
            {
                smoothMoveValue = Vector2.SmoothDamp(smoothMoveValue, Vector2.zero, ref currentVelocity, smoothTime);
            }
            
        }

        public void PlayerMove()
        {

            

            UpdateMovementAction();
            UpdateMovementDirection();

            switch (movementState)
            {
                case movementState.walk:
                    Walk();
                    break;

                case movementState.sprint:
                    Sprint();
                    break;

                case movementState.sneak:
                    Sneak();
                    break;

                case movementState.none:
                    CheckStaminaState();
                    break;
            }

            directionToMove *= (speed * internalSpeedMultiplier);

            ApplyGravity();
            ch_controller.Move(directionToMove * Time.deltaTime);
        }

        /// <summary>
        /// Sets up necessary parameters for walking.
        /// </summary>
        public void Walk()
        {
            walking = true;
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
            walking = false;

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
            walking = false;

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
                movementState = movementState.walk;
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
                walking = false;
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
