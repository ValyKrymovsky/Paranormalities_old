using UnityEngine;
using UnityEngine.InputSystem;

public class P_Movement : MonoBehaviour
{
    // Speed //
    [Header("Player speed")]
    [SerializeField]private float speed;    // walking speed
    [SerializeField]private float sprintMultiplier;
    private float internalMultiplier;   // speed multiplier


    // Gravity //
    [Header("Gravity")]
    [SerializeField] private float velocity;
    private float gravity = -9.8f;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private bool isGrounded;


    // Stamina //
    [Header("Stamina")]
     [SerializeField] private bool useStamina;  // use stamina system or not
     [SerializeField] private float depletionValue; // float value for stamina depletion
     [SerializeField] private float regenValue; // float value for stamina regeneration
    private bool isSprinting;
    private bool isMoving;
    private bool isMovingForward;


    // Direction //
    private Vector3 moveDir;    // Vector3 for player movement
    private float moveDirY;    // float for gravity
    private Vector2 p_movement;    // movement value from Input System


    // Components //
    private CharacterController ch_controller;    // Character Controller object
    private P_Controls p_input;
    private P_Stamina p_stamina;


    // Input Actions //
    private InputAction ac_move;    // input action for moving
    private InputAction ac_sprint;  // input action for sprinting


    // Coroutines //
    [SerializeField] private Coroutine s_regen;    // coroutine for stamina regeneration

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
        internalMultiplier = 1;
        isSprinting = false;
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
        float controllerMoveSensetivity = Mathf.Max(Mathf.Abs(input_value.x), Mathf.Abs(input_value.y));
        moveDir = (input_value.x * transform.right + input_value.y * transform.forward).normalized * controllerMoveSensetivity;

        isMoving = (input_value.x + input_value.y) != 0 ? true : false;
        isMovingForward = input_value.y > 0 ? true : false;

        // Checks if player should sprint or not
        switch (useStamina)
        {
            case true:
                if (isMovingForward)
                {
                    float sprint_input_value = ac_sprint.ReadValue<float>();
                    if (sprint_input_value > 0)
                    {
                        if ((p_stamina.staminaDepleted() && !p_stamina.staminaRegenerating()))
                        {
                            internalMultiplier = 1;
                            isSprinting = false;
                            s_regen = p_stamina.startRegen(regenValue);
                        }
                        else if (!p_stamina.staminaDepleted())
                        {
                            // (internalMultiplier, isSprinting) = sprint_input_value > 0 ? (sprintMultiplier, true) : (1, false);
                            if (p_stamina.staminaRegenerating())
                            {
                                StopCoroutine(s_regen);
                                p_stamina.isRegenerating = false;
                            }
                            internalMultiplier = sprintMultiplier;
                            isSprinting = true;
                            p_stamina.depleteStamina(depletionValue);

                        }
                    }
                    else
                    {
                        internalMultiplier = 1;
                        if (!p_stamina.staminaRegenerating() && p_stamina.regenNeeded())
                        {
                            s_regen = p_stamina.startRegen(regenValue);
                        }
                    }
                
                }
                else
                {
                    internalMultiplier = 1;
                    if (!p_stamina.staminaRegenerating() && p_stamina.regenNeeded())
                    {
                        s_regen = p_stamina.startRegen(regenValue);
                    }
                }
                break;

            case false:
                break;

        }

        ApplyGravity();

        moveDir.y = moveDirY;

        ch_controller.Move(moveDir * (speed * internalMultiplier) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        // Checks if player is grounded
        if (CheckIsGrounded())
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += (gravity * gravityMultiplier) * Time.deltaTime;
        }

        moveDirY = velocity;
    }

    private bool CheckIsGrounded()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 boxSize = new Vector3(.1f, .1f, .1f);
        
        bool checkSuc = Physics.BoxCast(position, boxSize, -(transform.up), out RaycastHit hit, transform.rotation, ch_controller.height / 2);

        if (checkSuc)
        {
            isGrounded = hit.collider.gameObject.tag == "Floor" ? true : false;
        }
        else
        {
            isGrounded = false;
        }
        
        return isGrounded;
    }


    private void OnDrawGizmos()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        Vector3 boxSize = new Vector3(.1f, .1f, .1f);
        if (isGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position, boxSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position, boxSize);
        }
    }
}

    
