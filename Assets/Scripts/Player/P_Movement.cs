using UnityEngine;
using UnityEngine.InputSystem;

public class P_Movement : MonoBehaviour
{
    // Speed //
    [Header("Player speed")]
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
    private P_Stamina p_stamina;

    // Input Actions //
    private InputAction ac_move;
    private InputAction ac_sprint;

    int index = 0;

    private void Awake()
    {
        ch_controller = GetComponent<CharacterController>();
        p_stamina = GetComponent<P_Stamina>();
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

        isMoving = (input_value.x + input_value.y) != 0 ? true : false;
        isMovingForward = input_value.y > 0 ? true : false;

        Sprint();

        // Debug.Log("isMoving: " + isMoving);
        // Debug.Log("isMovingForward: " + isMovingForward);
        // Debug.Log("isSprinting: " + isSprinting);
        // Debug.Log(input_value);
        // Debug.Log(p_stamina.getStamina());
    }

    private void Sprint()
    {
        if (isMovingForward && !p_stamina.staminaDepleted())
        {
            float input_value = ac_sprint.ReadValue<float>();
            (multiplier, isSprinting) = input_value == 1 ? (1.75f, true) : (1, false);

            if (isSprinting)
            {
                if (p_stamina.regenerating)
                {
                    StopCoroutine(p_stamina.regenerateStamina(.25f));
                    p_stamina.regenerating = false;
                }
                p_stamina.depleteStamina(.25f);
            }

        }
        else if (p_stamina.staminaDepleted() && !p_stamina.regenerating)
        {
            multiplier = 1;
            StartCoroutine(p_stamina.regenerateStamina(.25f));
            index += 1;
            Debug.Log(index);
        }
        else
        {
            multiplier = 1;
        }

        Debug.Log("startedRegenerating: " + p_stamina.regenerating);
        Debug.Log("stamineDepleted: " + p_stamina.staminaDepleted());
    }
}

    
