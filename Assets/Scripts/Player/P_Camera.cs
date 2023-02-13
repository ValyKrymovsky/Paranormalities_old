using UnityEngine;
using UnityEngine.InputSystem;

public class P_Camera : MonoBehaviour
{
    // Parameters //
    [SerializeField] private float sensetivity; // mouse sensetivity
    private float cur_rotX = 0f;                // current mouse rotation value on X axis

    // Floats //
    private float mouseX;                       // mouse rotation on X axis
    private float mouseY;                       // mouse rotation on Y axis

    // Components //
    private P_Controls p_input;
    private Transform p_body;

    // Input Actions //
    private InputAction ac_look;

    private void Awake()
    {
        p_input = new P_Controls();
        Cursor.lockState = CursorLockMode.Locked;
        p_body = transform.parent;

        ac_look = p_input.Player.Look;
    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }


    void Update()
    {
        Look();
    }


    private void Look()
    {
        Vector2 input_value = ac_look.ReadValue<Vector2>();

        mouseX = input_value.x * sensetivity * Time.deltaTime;
        mouseY = input_value.y * sensetivity * Time.deltaTime;

        cur_rotX -= mouseY;
        cur_rotX = Mathf.Clamp(cur_rotX, -75f, 70f);
        transform.localRotation = Quaternion.Euler(cur_rotX, 0, 0);
        p_body.Rotate(Vector3.up * mouseX);
    }

}
