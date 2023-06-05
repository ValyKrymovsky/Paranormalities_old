using UnityEngine;
using UnityEngine.InputSystem;
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

public class P_Movement : MonoBehaviour
{

    public GameObject directionSphere;

    [Separator("Player movement", true)]
    [Header("General parameters")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float sprintMultiplier, sneakMultiplier;
    [SerializeField]
    private movementState movementState;
    [SerializeField]
    private movementDirection movementDirection;
    private bool isMoving, isMovingForward = false;
    private bool walking, sprinting, sneaking = false;
    private float internalSpeedMultiplier;
    private float movementSensetivity;
    private Vector3 directionToMove;
    private Vector3 finalDirectionToMove;
    private Vector3 playerMoveDirection;
    private Vector2 smoothMoveValue;
    private Vector2 currentVelocity;
    [ Header("Movement Smoothening")]
    [SerializeField, Tooltip("Time before reaching full speed")]
    private float smoothTime;

    [Separator("Stamina", true)]
    [SerializeField]
    private bool useStaminaSystem;
    [SerializeField, ConditionalField("useStaminaSystem")]
    private float depletionValue;
    [SerializeField, ConditionalField("useStaminaSystem")]
    private float regenValue;

    [Separator("Gravity", true)]
    [SerializeField]
    private float gravityForce;
    [SerializeField]
    private float gravityMultiplier = .5f;
    [SerializeField]
    private bool isGrounded;
    private static float gravity = -9.8f;
    private float velocity;

    [Separator("Rigidbody manipulation")]
    [SerializeField]
    private float forceAmount = 1;
    // Coroutines //
    private Coroutine regenCoroutine;
    private Coroutine depleteCoroutine;



    // Components //
    private CharacterController ch_controller;    // Character Controller object
    private P_Controls p_input;
    private P_Stamina p_stamina;


    // Input Actions //
    private InputAction ac_move;    // input movementState for isMoving
    private InputAction ac_sprint;  // input movementState for sprinting
    private InputAction ac_sneak;  // input movementState for sprinting


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
        internalSpeedMultiplier = 1;
        movementState = movementState.none;
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

    public float GetCurrentSpeed()
    {
        return speed;
    }

    /// <summary>
    /// </summary>
    /// <returns>Current player move value</returns>
    public Vector2 GetMoveValue()
    {
        return smoothMoveValue;
    }

    public Vector3 GetSmoothMoveValue()
    {
        return smoothMoveValue;
    }

    public Vector3 GetPlayerDirectionToMoveTo()
    {
        return directionToMove;
    }

    public Vector3 GetPlayerMoveDirection()
    {
        return playerMoveDirection;
    }

    public Vector3 GetFinalPlayerDirectionToMoveTo()
    {
        return finalDirectionToMove;
    }

    /// <summary>
    /// Is responsible for player movement. Gets values from GetMoveValue(), GetSprintValue() and GetSneakValue(), calculates how the player should move.
    /// </summary>
    public void PlayerMove()
    {
        // sets movement sensetivity from on controller stick max value
        movementSensetivity = Mathf.Max(Mathf.Abs(move_value.x), Mathf.Abs(move_value.y));
        movementSensetivity = movementSensetivity < .4f ? .4f : movementSensetivity;

        // increases initial speed over time for smooth movement start
        smoothMoveValue = Vector2.SmoothDamp(smoothMoveValue, move_value, ref currentVelocity, smoothTime);

        directionToMove = (smoothMoveValue.x * transform.right + smoothMoveValue.y * transform.forward) * movementSensetivity;
        finalDirectionToMove = (move_value.x * transform.right + move_value.y * transform.forward) * movementSensetivity;

        // clamps player movement to magnitude of 1
        directionToMove = Vector3.ClampMagnitude(directionToMove, 1);
        finalDirectionToMove = Vector3.ClampMagnitude(finalDirectionToMove, 1);

        // Only for other classes
        playerMoveDirection = directionToMove;

        Debug.DrawLine(transform.position, transform.position + directionToMove * 10, Color.green);

        isMoving = IsPlayerMoving();

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
