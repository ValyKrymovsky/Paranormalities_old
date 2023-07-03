using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace MyCode.Player
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

        [Header("Camera Stabilization Components")]
        [Space]
        [SerializeField] private GameObject camStabilizationObject;
        [SerializeField] private GameObject headJoint;
        [SerializeField] private GameObject eyeJoint;

        private float mouseRotation = 0f;
        private float valueX;
        private float valueY;

        private Vector3 smootheningVelocity;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            player = GameObject.FindGameObjectWithTag("Player");
            cam = GetComponent<Camera>();
        }

        private void Start()
        {

            PlayerManager.Instance.Camera.CamStabilizationObject = camStabilizationObject;
            PlayerManager.Instance.Camera.HeadJoint = headJoint;
            PlayerManager.Instance.Camera.EyeJoint = eyeJoint;

            PlayerManager.Instance.Camera.CameraValueInput.action.performed += ctx => Look(ctx.ReadValue<Vector2>());
            Debug.Log("Done");
        }

        private void Look(Vector2 _valueXY)
        {
            valueX = _valueXY.x * PlayerManager.Instance.Camera.Sensetivity * Time.deltaTime;
            valueY = _valueXY.y * PlayerManager.Instance.Camera.Sensetivity * Time.deltaTime;

            mouseRotation -= valueY;
            mouseRotation = Mathf.Clamp(mouseRotation, PlayerManager.Instance.Camera.BottomRotationLimit, PlayerManager.Instance.Camera.TopRotationLimit);
            transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            PlayerManager.Instance.Camera.CamStabilizationObject.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            player.transform.Rotate(Vector3.up * valueX);
            if (PlayerManager.Instance.Camera.UseStabilization)
            {
                cam.transform.LookAt(FocusTarget());
                transform.position = FollowHeadJoint(PlayerManager.Instance.Camera.HeadJoint, PlayerManager.Instance.Camera.EyeJoint, .2f, PlayerManager.Instance.Camera.StabilizationAmount);
            }
            else
            {
                transform.position = FollowHeadJoint(PlayerManager.Instance.Camera.HeadJoint, PlayerManager.Instance.Camera.EyeJoint, .2f);
            }
        }

        private Vector3 FocusTarget()
        {
            PlayerManager.Instance.Camera.FocusPointStabilizationDistance = PlayerManager.Instance.Camera.FocusPointStabilizationDistance <= 0 ? 1f : PlayerManager.Instance.Camera.FocusPointStabilizationDistance;
            Vector3 pos = PlayerManager.Instance.Camera.CamStabilizationObject.transform.position + PlayerManager.Instance.Camera.CamStabilizationObject.transform.forward * PlayerManager.Instance.Camera.FocusPointStabilizationDistance;
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