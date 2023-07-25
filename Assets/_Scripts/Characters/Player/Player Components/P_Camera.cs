using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using MyCode.Managers;

namespace MyCode.Player.Components
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
            PlayerManager.Instance.InventoryData.OnInventoryStatusChange += value => canLook = !value;
        }

        private void OnDisable()
        {
            inputAction.action.Disable();
            inputAction.action.performed -= ctx => Look(ctx.ReadValue<Vector2>());
            PlayerManager.Instance.InventoryData.OnInventoryStatusChange -= value => canLook = !value;
        }

        private void Look(Vector2 _valueXY)
        {
            if (!canLook)
                return;

            valueX = _valueXY.x * PlayerManager.Instance.CameraData.Sensetivity * Time.deltaTime;
            valueY = _valueXY.y * PlayerManager.Instance.CameraData.Sensetivity * Time.deltaTime;

            mouseRotation -= valueY;
            mouseRotation = Mathf.Clamp(mouseRotation, PlayerManager.Instance.CameraData.BottomRotationLimit, PlayerManager.Instance.CameraData.TopRotationLimit);
            transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            camStabilizationObject.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            player.transform.Rotate(Vector3.up * valueX);
            if (PlayerManager.Instance.CameraData.UseStabilization)
            {
                cam.transform.LookAt(FocusTarget());
                transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f, PlayerManager.Instance.CameraData.StabilizationAmount);
            }
            else
            {
                transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f);
            }
        }

        private Vector3 FocusTarget()
        {
            PlayerManager.Instance.CameraData.FocusPointStabilizationDistance = PlayerManager.Instance.CameraData.FocusPointStabilizationDistance <= 0 ? 1f : PlayerManager.Instance.CameraData.FocusPointStabilizationDistance;
            Vector3 pos = camStabilizationObject.transform.position + camStabilizationObject.transform.forward * PlayerManager.Instance.CameraData.FocusPointStabilizationDistance;
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