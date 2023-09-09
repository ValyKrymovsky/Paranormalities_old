using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using MyCode.Managers;

namespace MyCode.PlayerComponents
{
    [System.Serializable]
    public class P_Camera : MonoBehaviour
    {
        [Space]
        [Separator("Components", true)]
        [Space]

        [Header("Player Components")]
        [Space]
        [SerializeField] private GameObject player;
        [SerializeField] private Camera cam;
        [Space]

        [Header("CameraData Stabilization Components")]
        [Space]
        [SerializeField] private GameObject camStabilizationObject;
        [SerializeField] private GameObject headJoint;
        [SerializeField] private GameObject eyeJoint;

        private bool canLook = true;

        private float mouseRotation = 0f;
        private float valueX;
        private float valueY;

        public InputActionReference inputAction;

        private Vector3 smootheningVelocity;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            player = GameObject.FindGameObjectWithTag("Player");
            cam = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            inputAction.action.Enable();
            inputAction.action.performed += ctx => Look(ctx.ReadValue<Vector2>());
            PlayerManager.InventoryData.OnInventoryStatusChange += value => canLook = !value;
        }

        private void OnDisable()
        {
            inputAction.action.Disable();
            inputAction.action.performed -= ctx => Look(ctx.ReadValue<Vector2>());
            PlayerManager.InventoryData.OnInventoryStatusChange -= value => canLook = !value;
        }

        private void Look(Vector2 _valueXY)
        {
            if (!canLook)
                return;

            valueX = _valueXY.x * PlayerManager.CameraData.Sensetivity * Time.deltaTime;
            valueY = _valueXY.y * PlayerManager.CameraData.Sensetivity * Time.deltaTime;

            mouseRotation -= valueY;
            mouseRotation = Mathf.Clamp(mouseRotation, PlayerManager.CameraData.BottomRotationLimit, PlayerManager.CameraData.TopRotationLimit);
            transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            player.transform.Rotate(Vector3.up * valueX);
            if (PlayerManager.CameraData.UseStabilization)
            {
                camStabilizationObject.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
                cam.transform.LookAt(FocusTarget());
                transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f, PlayerManager.CameraData.StabilizationAmount);
            }
        }

        private Vector3 FocusTarget()
        {
            PlayerManager.CameraData.FocusPointStabilizationDistance = PlayerManager.CameraData.FocusPointStabilizationDistance <= 0 ? 1f : PlayerManager.CameraData.FocusPointStabilizationDistance;
            Vector3 pos = camStabilizationObject.transform.position + camStabilizationObject.transform.forward * PlayerManager.CameraData.FocusPointStabilizationDistance;
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
}