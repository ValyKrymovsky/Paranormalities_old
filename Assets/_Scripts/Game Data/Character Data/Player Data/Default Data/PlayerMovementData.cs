using System;
using UnityEngine;
using MyBox;
using MyCode.GameData.Player.Movement;


namespace MyCode.GameData.PlayerData
{
    [CreateAssetMenu(fileName = "NewMovementData", menuName = "DataObjects/Player/Movement")]
    public class PlayerMovementData : ScriptableObject
    {

        [Space]
        [Separator("Player movement", true)]
        [Space]

        [Header("General parameters")]
        [Space]

        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintMultiplier, _sneakMultiplier;
        [SerializeField] private MovementState _movementState;
        [SerializeField] private MovementDirection _movementDirection;
        [SerializeField, ReadOnly] private bool _isMoving, _isMovingForward = false;
        [SerializeField, ReadOnly] private Vector3 _directionToMove;
        [SerializeField, ReadOnly] private Vector2 _smoothMoveValue;
        [Space]

        [Header("Movement Smoothening")]
        [Space]
        [SerializeField, Tooltip("Time before reaching full speed")] private float _smoothTime;

        [Space]
        [Separator("Gravity", true)]
        [Space]

        [Header("Gravity Parameters")]
        [Space]
        [SerializeField] private float _gravityMultiplier = .5f;
        [SerializeField] private bool _isGrounded;
        [SerializeField] private bool _useCustomGravity;
        private const float _customGravity = -9.8f;

        [Space]
        [Separator("Difficulty", true)]
        [Space]

        [Header("Rules")]
        [Space]
        [SerializeField] private bool freezeOnInventory;


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
        public bool FreezeOnInventory { get => freezeOnInventory; }

    }
}
