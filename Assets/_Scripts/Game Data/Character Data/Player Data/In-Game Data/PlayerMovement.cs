using System;
using UnityEngine;
using MyBox;
using MyCode.GameData.Player.Movement;


namespace MyCode.GameData.PlayerData
{
    public class PlayerMovement
    {

        [Space]
        [Separator("Player movement", true)]
        [Space]

        [Header("General parameters")]
        [Space]

        private float _walkSpeed;
        private float _sprintMultiplier, _sneakMultiplier;
        private MovementState _movementState;
        private MovementDirection _movementDirection;
        private bool _isMoving, _isMovingForward = false;
        private Vector3 _directionToMove;
        private Vector2 _smoothMoveValue;
        [Space]

        [Header("Movement Smoothening")]
        [Space]
        private float _smoothTime;

        [Space]
        [Separator("Gravity", true)]
        [Space]

        [Header("Gravity Parameters")]
        [Space]
        private float _gravityMultiplier = .5f;
        private bool _isGrounded;
        private bool _useCustomGravity;
        private const float _customGravity = -9.8f;

        [Space]
        [Separator("Difficulty", true)]
        [Space]

        [Header("Rules")]
        [Space]
        private bool _freezeOnInventory;


        // Events
        public event Action StartedRunning;
        public event Action StoppedRunning;


        // Movement
        public float WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
        public float SprintMultiplier { get => _sprintMultiplier; private set => _sprintMultiplier = value; }
        public float SneakMultiplier { get => _sneakMultiplier; private set => _sneakMultiplier = value; }
        public MovementState MovementState { get => _movementState; set => _movementState = value; }
        public MovementDirection MovementDirection { get => _movementDirection; set => _movementDirection = value; }
        public bool IsMoving { get => _isMoving; set => _isMoving = value; }
        public bool IsMovingForward { get => _isMovingForward; set => _isMovingForward = value; }
        public Vector3 DirectionToMove { get => _directionToMove; set => _directionToMove = value; }
        public Vector2 SmoothMoveValue { get => _smoothMoveValue; set => _smoothMoveValue = value; }
        public float SmoothTime { get => _smoothTime; set => _smoothTime = value; }

        // Gravity
        public float GravityMultiplier { get => _gravityMultiplier; set => _gravityMultiplier = value; }
        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
        public bool UseCustomGravity { get => _useCustomGravity; set => _useCustomGravity = value; }
        public float CustomGravity { get => _customGravity; }

        // Difficulty Rules
        public bool FreezeOnInventory { get => _freezeOnInventory; }


        public PlayerMovement(PlayerMovementData _data)
        {
            _walkSpeed = _data.WalkSpeed;
            _sprintMultiplier = _data.SprintMultiplier;
            _sneakMultiplier = _data.SneakMultiplier;
            _movementState = _data.MovementState;
            _movementDirection = _data.MovementDirection;
            _isMoving = _data.IsMoving;
            _isMovingForward = _data.IsMovingForward;
            _directionToMove = _data.DirectionToMove;
            _smoothMoveValue = _data.SmoothMoveValue;

            _smoothTime = _data.SmoothTime;

            _gravityMultiplier = _data.GravityMultiplier;
            _isGrounded = _data.IsGrounded;
            _useCustomGravity = _data.UseCustomGravity;
            
            _freezeOnInventory = _data.FreezeOnInventory;
        }

        public void InvokeStartedRunning()
        {
            StartedRunning?.Invoke();
        }

        public void InvokeStoppedRunning()
        {
            StoppedRunning?.Invoke();
        }

    }

}
