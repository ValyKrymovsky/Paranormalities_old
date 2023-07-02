using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;

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
    private bool isMoving, isMovingForward = false;
    private float internalSpeedMultiplier;
    private float movementSensetivity;
    private Vector3 directionToMove;
    private Vector3 finalDirectionToMove;
    private Vector3 playerMoveDirection;
    private Vector2 smoothMoveValue;
    private Vector2 currentVelocity;
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
    private const float customGravity = -9.8f;
    private float gravityForce;
    private float velocity;

    private Vector2 move_value;
    private float sprint_value;
    private float sneak_value;
}
