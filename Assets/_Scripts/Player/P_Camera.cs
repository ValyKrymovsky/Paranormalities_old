using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

[System.Serializable]
public class P_Camera : MonoBehaviour
{
    [SerializeField, Separator("Sensetivity", true)] private float sensetivity;
    private float mouseRotation = 0f;
    [SerializeField]
    private float topRotationLimit;
    [SerializeField]
    private float bottomRotationLimit;
    private float mouseX;
    private float mouseY;

    [SerializeField, Separator("Camera Stabilization", true)]
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

    /// <summary>
    /// Gets mouse input from InputAction and puts it in input_value.
    /// </summary>
    /// <param name="_ctx"></param>
    public void GetLookValue(InputAction.CallbackContext _ctx)
    {
        input_value = _ctx.ReadValue<Vector2>();
    }

    public Vector2 GetInput()
    {
        return input_value;
    }

    /// <summary>
    /// Rotates camera based on mouse input value. If useStabilization is true, calls camera stabilization methods.
    /// </summary>
    private void Look()
    {
        mouseX = input_value.x * sensetivity * Time.deltaTime;
        mouseY = input_value.y * sensetivity * Time.deltaTime;

        mouseRotation -= mouseY;
        mouseRotation = Mathf.Clamp(mouseRotation, bottomRotationLimit, topRotationLimit);
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

        Debug.DrawLine(transform.position, transform.position + transform.forward * 50);
        
    }

    /// <summary>
    /// Finds point in space in front of camera based on given focusPointStabilizationDistance.
    /// </summary>
    /// <returns>Point in space position</returns>
    private Vector3 FocusTarget()
    {
        focusPointStabilizationDistance = focusPointStabilizationDistance <= 0 ? 1f : focusPointStabilizationDistance;
        Vector3 pos = camStabilizationObject.transform.position + camStabilizationObject.transform.forward * focusPointStabilizationDistance;
        return pos;
    }

    /// <summary>
    /// Returns position of head joint position with eye joint y position with given offset.
    /// </summary>
    /// <param name="_headJoint"></param>
    /// <param name="_eyeJoint"></param>
    /// <param name="_offset"></param>
    private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset)
    {
        return new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset;
    }

    /// <summary>
    /// Returns position of head joint position with eye joint y position with given offset and stabilizes position with smoothDamp.
    /// </summary>
    /// <param name="_headJoint"></param>
    /// <param name="_eyeJoint"></param>
    /// <param name="_offset"></param>
    /// <param name="_stabilizationAmount"></param>
    private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset, float _stabilizationAmount)
    {
        return Vector3.SmoothDamp(transform.position, new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset, ref smootheningVelocity, _stabilizationAmount);
    }
}
