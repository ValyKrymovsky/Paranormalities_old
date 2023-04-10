using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class P_Camera : MonoBehaviour
{
    [SerializeField, Header("Sensetivity")] private float sensetivity;
    private float mouseRotation = 0f;
    private float mouseX;
    private float mouseY;

    // [SerializeField, Header("Head Bob")]
    // private float walkBobSpeed = 14f;
    // [SerializeField]
    // private float walkBobAmount = .5f;
    // [SerializeField]
    // private float sprintBobSpeed = 18f;
    // [SerializeField]
    // private float sprintBobAmount = .75f;
    // [SerializeField]
    // private float sneakBobSpeed = 18f;
    // [SerializeField]
    // private float sneakBobAmount = .75f;

    [SerializeField, Header("Camera Stabilization")]
    private bool useStabilization;
    [SerializeField, Range(0, 100)]
    private float focusPointStabilizationDistance;
    [SerializeField, Range(0, 1)]
    private float stabilizationAmount;
    [SerializeField]
    private GameObject camStabilizationObject;
    [SerializeField]
    private GameObject headJoint;
    [SerializeField]
    private GameObject eyeJoint;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 smootheningVelocity;

    private Vector2 input_value;

    private P_Controls p_input;
    private InputAction ac_look;

    private GameObject playerBody;
    private Transform playerBodyPosition;

    private Camera cam;
    private Transform camPosition;

    private void Awake()
    {
        p_input = new P_Controls();
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = GameObject.FindGameObjectWithTag("Player");
        playerBodyPosition = playerBody.transform;
        cam = GetComponent<Camera>();
        camPosition = cam.transform;

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

    public void GetLookValue(InputAction.CallbackContext _ctx)
    {
        input_value = _ctx.ReadValue<Vector2>();
    }

    private void Look()
    {
        mouseX = input_value.x * sensetivity * Time.deltaTime;
        mouseY = input_value.y * sensetivity * Time.deltaTime;

        mouseRotation -= mouseY;
        mouseRotation = Mathf.Clamp(mouseRotation, -75f, 70f);
        transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
        camStabilizationObject.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
        playerBodyPosition.Rotate(Vector3.up * mouseX);
        if (useStabilization)
        {
            camPosition.LookAt(FocusTarget());
            transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f, stabilizationAmount);
        }
        else
        {
            transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f);
        }
        
    }

    private Vector3 FocusTarget()
    {
        focusPointStabilizationDistance = focusPointStabilizationDistance <= 0 ? 1f : focusPointStabilizationDistance;
        Vector3 pos = camStabilizationObject.transform.position + camStabilizationObject.transform.forward * focusPointStabilizationDistance;
        return pos;
    }

    private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset)
    {
        return new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset;
    }

    private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset, float _stabilizationAmount)
    {
        return Vector3.SmoothDamp(transform.position, new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset, ref smootheningVelocity, _stabilizationAmount);
    }
}
