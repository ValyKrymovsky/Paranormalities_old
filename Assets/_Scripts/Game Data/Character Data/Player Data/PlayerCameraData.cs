using UnityEngine.InputSystem;
using UnityEngine;
using MyBox;

namespace MyCode.Data.Player
{
    [CreateAssetMenu(fileName = "NewCameraData", menuName = "DataObjects/Player/Camera")]
    public class PlayerCameraData : ScriptableObject
    {
        [Space]
        [Separator("Camera properties", true)]
        [Space]

        [Header("Sensetivity")]
        [Space]
        [SerializeField] private float sensetivity;
        [Space]

        [Header("Rotation Limit")]
        [Space]
        [SerializeField] private float topRotationLimit;
        [SerializeField] private float bottomRotationLimit;

        [Space]
        [Separator("Camera Stabilization", true)]
        [Space]

        [Header("Stabilization")]
        [Space]
        [SerializeField] private bool useStabilization;
        [SerializeField, Range(0, 100)] private float focusPointStabilizationDistance;
        [SerializeField, Range(0, 1)] private float stabilizationAmount;
        private GameObject camStabilizationObject;
        private GameObject headJoint;
        private GameObject eyeJoint;

        [Space]
        [Separator("Inputs")]
        [Space]

        [Header("Input Action")]
        [Space]
        [SerializeField] private InputActionReference _input_CameraValue;


        public float Sensetivity { get => sensetivity; set => sensetivity = value; }
        public float TopRotationLimit { get => topRotationLimit; set => topRotationLimit = value; }
        public float BottomRotationLimit { get => bottomRotationLimit; set => bottomRotationLimit = value; }
        public bool UseStabilization { get => useStabilization; set => useStabilization = value; }
        public float FocusPointStabilizationDistance { get => focusPointStabilizationDistance; set => focusPointStabilizationDistance = value; }
        public float StabilizationAmount { get => stabilizationAmount; set => stabilizationAmount = value; }
        public GameObject CamStabilizationObject { get => camStabilizationObject; set => camStabilizationObject = value; }
        public GameObject HeadJoint { get => headJoint; set => headJoint = value; }
        public GameObject EyeJoint { get => eyeJoint; set => eyeJoint = value; }
        public InputActionReference CameraValueInput { get => _input_CameraValue; }

        private void OnEnable()
        {
            _input_CameraValue.action.Enable();
        }

        private void OnDisable()
        {
            _input_CameraValue.action.Disable();
        }
    }

}
