using UnityEngine;
using MyBox;
using System;

namespace MyCode.GameData.PlayerData
{
    public class PlayerCamera
    {
        [Space]
        [Separator("Camera properties", true)]
        [Space]

        [Header("Sensetivity")]
        [Space]
        private float _sensetivity;
        [Space]

        [Header("Rotation Limit")]
        [Space]
        private float _topRotationLimit;
        private float _bottomRotationLimit;

        [Space]
        [Separator("Camera Stabilization", true)]
        [Space]

        [Header("Stabilization")]
        [Space]
        private bool _useStabilization;
        private float _focusPointStabilizationDistance;
        private float _stabilizationAmount;


        public float Sensetivity { get => _sensetivity; set => _sensetivity = value; }
        public float TopRotationLimit { get => _topRotationLimit; set => _topRotationLimit = value; }
        public float BottomRotationLimit { get =>_bottomRotationLimit; set => _bottomRotationLimit = value; }
        public bool UseStabilization { get => _useStabilization; set => _useStabilization = value; }
        public float FocusPointStabilizationDistance { get => _focusPointStabilizationDistance; set => _focusPointStabilizationDistance = value; }
        public float StabilizationAmount { get => _stabilizationAmount; set => _stabilizationAmount = value; }


        public PlayerCamera(PlayerCameraData _data)
        {
            _sensetivity = _data.Sensetivity;
            _topRotationLimit = _data.TopRotationLimit;
            _bottomRotationLimit = _data.BottomRotationLimit;
            _useStabilization = _data.UseStabilization;
            _focusPointStabilizationDistance = _data.FocusPointStabilizationDistance;
            _stabilizationAmount = _data.StabilizationAmount;
        }
    }

}
