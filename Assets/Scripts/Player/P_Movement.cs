using UnityEngine;
using UnityEngine.InputSystem;

public class P_Movement : MonoBehaviour
{
    // Speed //
    [Header("Player Speed")]
    [SerializeField]private float speed;    // walking speed
    [SerializeField]private float multiplier;    // sprint speed multiplier

    // Gravity //
    [Header("Gravity")]
    [SerializeField] private float velocity;
    [SerializeField] private float gravity;

    // Booleans //
    private bool isSprinting;
    private bool isMoving;
    private bool isMovingForward;

    // Direction //
    private Vector3 moveDir;    // direction where player should move
    private Vector2 p_movement;    // movement value from Input System

    // Components //
    private CharacterController ch_controller;    // Character Controller object
    private P_Controls p_input;

    // Input Actions //
    private InputAction ac_move;
    private InputAction ac_sprint;

    private void Awake()
    {
        ch_controller = GetComponent<CharacterController>();
        p_input = new P_Controls();

        ac_move = p_input.Player.Move;
        ac_sprint = p_input.Player.Sprint;
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }

    private void Update()
    {
        PlayerMove();
        Sprint();
    }


    private void PlayerMove() 
    {
        Vector2 input_value = ac_move.ReadValue<Vector2>();
        moveDir = (input_value.y * transform.forward + input_value.x * transform.right).normalized;

        // Checks if player is grounded, if yes sets velocity to -1.0f, otherwise 
        if (ch_controller.isGrounded && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
            
        }
        moveDir.y = velocity;
        ch_controller.Move(moveDir * (speed * multiplier) * Time.deltaTime);

        isMoving = (moveDir.x + moveDir.z) != 0 ? true : false;

        Debug.Log("isMoving: " + isMoving);
        Debug.Log("isMovingForward: " + isMovingForward);
        Debug.Log(moveDir);
        // Debug.Log(velocity);
        // Debug.Log(ch_controller.isGrounded);
    }

    private void Sprint()
    {
        float input_value = ac_sprint.ReadValue<float>();

        multiplier = input_value > 0 ? 1.5f : 1;
    }
}

    
