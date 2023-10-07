using System;
using UnityEngine;
using MyBox;


namespace MyCode.GameData
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
        [SerializeField, ReadOnly] private bool _isMoving, _isMovingForward = false;
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

        public event Action OnStoppedRunning;
        public event Action OnStartedRunning;


        // Movement
        public float WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
        public float SprintMultiplier { get => _sprintMultiplier; private set => _sprintMultiplier = value; }
        public float SneakMultiplier { get => _sneakMultiplier; private set => _sneakMultiplier = value; }
        public bool IsMoving { get => _isMoving; set => _isMoving = value; }
        public bool IsMovingForward { get => _isMovingForward; set => _isMovingForward = value; }
        public float SmoothTime { get => _smoothTime; set => _smoothTime = value; }

        // Gravity
        public float GravityMultiplier { get => _gravityMultiplier; set => _gravityMultiplier = value; }
        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
        public bool UseCustomGravity { get => _useCustomGravity; set => _useCustomGravity = value; }
        public float CustomGravity { get => _customGravity; }

        // Difficulty Rules
        public bool FreezeOnInventory { get => freezeOnInventory; }


        public void InvokeOnStoppedRunning()
        {
            OnStoppedRunning?.Invoke();
        }

        public void InvokeOnStartedRunning()
        {
            OnStartedRunning?.Invoke();
        }
    }
}
