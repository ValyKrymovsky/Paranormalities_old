using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;
using MyCode.Player;

public enum movementState
{
    none,
    walk,
    sprint,
    sneak
}

public enum movementDirection
{
    none,
    forward,
    forward_right,
    forward_left,
    right,
    left,
    backward,
    backward_right,
    backward_left
}



[CreateAssetMenu(fileName = "NewMovementData", menuName = "DataObjects/Player/Movement")]
public class PlayerMovementData : ScriptableObject
{

    [Space]
    [Separator("Player movement", true)]
    [Space]

    [Header("General parameters")]
    [Space]
    
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier, sneakMultiplier;
    [SerializeField] private movementState movementState;
    [SerializeField] private movementDirection movementDirection;
    [SerializeField, ReadOnly] private bool isMoving, isMovingForward = false;
    [SerializeField, ReadOnly] private Vector3 directionToMove;
    [SerializeField, ReadOnly] private Vector2 smoothMoveValue;
    
    [Space]

    [Header("Movement Smoothening")]
    [Space]
    [SerializeField, Tooltip("Time before reaching full speed")] private float smoothTime;

    [Space]
    [Separator("Gravity", true)]
    [Space]

    [Header("Gravity Parameters")]
    [Space]
    [SerializeField] private float gravityMultiplier = .5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool useCustomGravity;
    private const float customGravity = -9.8f;
    private float gravityForce;
    private float velocity;

    [Space]
    [Separator("Inputs")]
    [Space]

    [Header("Input Action")]
    [Space]
    [SerializeField] private InputActionReference input_WalkValue;
    [SerializeField] private InputActionReference input_SprintValue;
    [SerializeField] private InputActionReference input_SneakValue;


    // Movement
    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float SprintMultiplier { get => sprintMultiplier; private set => sprintMultiplier = value; }
    public float SneakMultiplier { get => sneakMultiplier; private set => sneakMultiplier = value; }
    public movementState MovementState { get => movementState; set => movementState = value; }
    public movementDirection MovementDirection { get => movementDirection; set => movementDirection = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public bool IsMovingForward { get => isMovingForward; set => isMovingForward = value; }
    public Vector3 DirectionToMove { get => directionToMove; set => directionToMove = value; }
    public Vector2 SmoothMoveValue { get => smoothMoveValue; set => smoothMoveValue = value; }
    public float SmoothTime { get => smoothTime; set => smoothTime = value; }

    // Gravity
    public float GravityMultiplier { get => gravityMultiplier; set => gravityMultiplier = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool UseCustomGravity { get => useCustomGravity; set => useCustomGravity = value; }
    public float CustomGravity { get => customGravity; }

    // Inputs
    public InputActionReference WalkValueInput { get => input_WalkValue; }
    public InputActionReference SprintValueInput { get => input_SprintValue; }
    public InputActionReference SneakValueInput { get => input_SneakValue; }
    

    private void OnEnable()
    {
        input_WalkValue.action.Enable();
        input_SprintValue.action.Enable();
        input_SneakValue.action.Enable();
    }

    private void OnDisable()
    {
        input_WalkValue.action.Disable();
        input_SprintValue.action.Disable();
        input_SneakValue.action.Disable();
    }
}
