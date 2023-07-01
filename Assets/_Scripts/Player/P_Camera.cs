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

    private GameObject playerBody;
    private Transform playerBodyPosition;

    private Camera cam;
    private Transform camPosition;

    private bool canLook;

    private void Awake()
    {
        p_input = new P_Controls();
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = GameObject.FindGameObjectWithTag("Player");
        playerBodyPosition = playerBody.transform;
        cam = GetComponent<Camera>();
        camPosition = cam.transform;

        canLook = true;

    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }

    public bool GetCanLook()
    {
        return canLook;
    }

    public void SetCanLook(bool _canLook)
    {
        canLook = _canLook;
    }

    void Update()
    {
        if (canLook)
        {
            Look();
        }
        
    }

    public void GetLookValue(InputAction.CallbackContext _ctx)
    {
        input_value = _ctx.ReadValue<Vector2>();
    }

    public Vector2 GetInput()
    {
        return input_value;
    }

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
