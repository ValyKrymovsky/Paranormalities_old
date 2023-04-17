using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public enum moveAction
{
    nothing,
    walk,
    sprint,
    sneak
}

public class P_Movement : MonoBehaviour
{
    [SerializeField, Separator("Player movement", true)]
    private float speed;
    [SerializeField]
    private float sprintMultiplier, sneakMultiplier;
    [SerializeField]
    private moveAction action;
    private bool moving, movingForward = false;
    private bool walking, sprinting, sneaking = false;
    private float internalMultiplier;
    private Vector3 moveDirection;
    private Vector2 currentMoveValue;
    private Vector2 currentVelocity;
    [SerializeField, Tooltip("Time before reaching full speed"), Header("Movement Smoothening")]
    private float smoothTime;

    [SerializeField, Separator("Stamina", true)]
    private bool useStaminaSystem;
    [SerializeField, ConditionalField("useStaminaSystem")]
    private float depletionValue;
    [SerializeField, ConditionalField("useStaminaSystem")]
    private float regenValue;

    [SerializeField, Separator("Gravity", true)]
    private float gravityForce;
    [SerializeField]
    private float gravityMultiplier = .5f;
    [SerializeField]
    private bool isGrounded;
    private static float gravity = -9.8f;
    private float velocity;

    [SerializeField, Separator("Rigidbody manipulation")]
    private float forceAmount = 1;
    // Coroutines //
    private Coroutine regenCoroutine;
    private Coroutine depleteCoroutine;



    // Components //
    private CharacterController ch_controller;    // Character Controller object
    private P_Controls p_input;
    private P_Stamina p_stamina;


    // Input Actions //
    private InputAction ac_move;    // input action for moving
    private InputAction ac_sprint;  // input action for sprinting
    private InputAction ac_sneak;  // input action for sprinting


    // Internal //
    private Vector2 move_value;
    private float sprint_value;
    private float sneak_value;

    private void Awake()
    {
        ch_controller = GetComponent<CharacterController>();
        p_stamina = GetComponent<P_Stamina>();
        p_input = new P_Controls();

        ac_move = p_input.Player.Move;
        ac_sprint = p_input.Player.Sprint;
        ac_sneak = p_input.Player.Sneak;
    }

    void Start()
    {
        internalMultiplier = 1;
        action = moveAction.nothing;
        sprinting = false;
        walking = false;
        sneaking = false;

        regenCoroutine = null;
        depleteCoroutine = null;
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

    /// <summary>
    /// Reads value from InputAction Move.
    /// </summary>
    /// <param name="_ctx"></param>
    public void GetMoveValue(InputAction.CallbackContext _ctx)
    {
        move_value = _ctx.ReadValue<Vector2>();
    }

    /// <summary>
    /// Reads value from InputAction Sprint.
    /// </summary>
    /// <param name="_ctx"></param>
    public void GetSprintValue(InputAction.CallbackContext _ctx)
    {
        sprint_value = _ctx.ReadValue<float>();
    }

    /// <summary>
    /// Reads value from InputAction Sneak.
    /// </summary>
    /// <param name="_ctx"></param>
    public void GetSneakValue(InputAction.CallbackContext _ctx)
    {
        sneak_value = _ctx.ReadValue<float>();
    }

    /// <summary>
    /// </summary>
    /// <returns>Current player move value</returns>
    public Vector2 GetMoveValue()
    {
        return currentMoveValue;
    }

    /// <summary>
    /// Is responsible for player movement. Gets values from GetMoveValue(), GetSprintValue() and GetSneakValue(), calculates how the player should move.
    /// </summary>
    public void PlayerMove()
    {
        float controllerMoveSensetivity = Mathf.Max(Mathf.Abs(move_value.x), Mathf.Abs(move_value.y));
        currentMoveValue = Vector2.SmoothDamp(currentMoveValue, move_value, ref currentVelocity, smoothTime);
        moveDirection = (currentMoveValue.x * transform.right + currentMoveValue.y * transform.forward) * controllerMoveSensetivity;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1);

        if (move_value.x != 0 || move_value.y != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        movingForward = move_value.y > 0 ? true : false;

        if (((!movingForward && moving) || (movingForward && (sprint_value == 0 || p_stamina.IsDepleted()))) && sneak_value == 0)
        {
            action = moveAction.walk;
        }
        else if (movingForward && sprint_value != 0 && sneak_value == 0 && ((!p_stamina.IsDepleted() && !p_stamina.LimitReached())))
        {
            action = moveAction.sprint;
        }
        else if (moving && sneak_value != 0)
        {
            action = moveAction.sneak;
        }
        else if (!moving)
        {
            walking = false;
            sprinting = false;
            sneaking = false;
            action = moveAction.nothing;
        }

        switch (action)
        {
            case moveAction.walk:
                Walk();
                break;

            case moveAction.sprint:
                Sprint();
                break;

            case moveAction.sneak:
                Sneak();
                break;

            case moveAction.nothing:
                CheckStaminaState();
                break;
        }
        ApplyGravity();
        ch_controller.Move(moveDirection * (speed * internalMultiplier) * Time.deltaTime);
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
            internalMultiplier = .75f;
        }
        else
        {
            internalMultiplier = 1;
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
                internalMultiplier = sprintMultiplier;
            }

            if (p_stamina.IsDepleted() && depleteCoroutine != null)
            {
                depleteCoroutine = null;
                regenCoroutine = p_stamina.StartRegenerate(regenValue);
                internalMultiplier = 1;
            }
        }
        else
        {
            internalMultiplier = sprintMultiplier;
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
        internalMultiplier = sneakMultiplier;
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
    /// <returns>current players move action</returns>
    public moveAction GetMoveAction()
    {
        return action;
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
        moveDirection.y = gravityForce;
    }

    /// <summary>
    /// Checks if player is grounded by casting ray to the ground from players position.
    /// </summary>
    /// <returns>True if player is grounded, false if not</returns>
    public bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, transform.up * -1, out RaycastHit hitInfo, .1f))
        {
            isGrounded = hitInfo.collider.gameObject.tag == "Floor" ? true : false;
        }
        else
        {
            isGrounded = false;
        }
        return isGrounded;
    }

    /// <summary>
    /// Adds force to rigidbodies when in contact.
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        if (rigidbody != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            rigidbody.AddForceAtPosition(forceDirection * forceAmount, transform.position, ForceMode.Impulse);
        }
    }

}

