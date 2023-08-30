using UnityEngine;
using System;
using MyBox;


namespace MyCode.GameData.PlayerData
{
    public class PlayerInteraction
    {
        [Space]
        [Separator("Interaction", true)]
        [Space]

        [Header("Player")]
        [Space]
        private LayerMask _playerLayerMask;

        [Header("Interaction parameters")]
        [Space]
        private float _interactRange;
        private float _sphereCheckRange;
        private Vector3 _hitPosition;
        private Vector3 _perColliderHitPosition;
        private int _colliderAraySize;
        private LayerMask _interactiblesMask;
        private LayerMask _excludeCollisionMask;
        [Space]

        [Space]
        [Separator("Pickup", true)]
        [Space]

        [Header("Distances")]
        [Space]
        private float _maxPickupPointDistance;
        private float _minPickupPointDistance;
        private float _maxDistanceFromPoint;
        private float _pickupPointDistance;
        [Space]
        private float _maxObjectWeight;
        [Space]
        private float _dropVelocityReduction;
        [Space]

        [Header("Throw")]
        [Space]
        private float _throwStrength;
        [Space]

        [Header("Zoom")]
        [Space]
        private int _intervalCount;
        private float _zoomInterval;


        public event Action OnPickUpObject;
        public event Action OnDropObject;

        // Player
        public LayerMask PlayerLayerMask { get => _playerLayerMask; set => _playerLayerMask = value; }

        // Interaction
        public float InteractRange { get => _interactRange; set => _interactRange = value; }
        public float SphereCheckRange { get => _sphereCheckRange; set => _sphereCheckRange = value; }
        public Vector3 HitPosition { get => _hitPosition; set => _hitPosition = value; }
        public Vector3 PerColliderHitPosition { get => _perColliderHitPosition; set => _perColliderHitPosition = value; }
        public int ColliderAraySize { get => _colliderAraySize; }
        public LayerMask InteractiblesMask { get => _interactiblesMask; set => _interactiblesMask = value; }
        public LayerMask ExcludeCollisionMask { get => _excludeCollisionMask; set => _excludeCollisionMask = value; }


        // Pickup distance parameters
        public float MaxPickupPointDistance { get => _maxPickupPointDistance; set => _maxPickupPointDistance = value; }
        public float MinPickupPointDistance { get => _minPickupPointDistance; set => _minPickupPointDistance = value; }
        public float MaxDistanceFromPoint { get => _maxDistanceFromPoint; set => _maxDistanceFromPoint = value; }
        public float PickupPointDistance { get => _pickupPointDistance; set => _pickupPointDistance = value; }
        public float MaxObjectWeight { get => _maxObjectWeight; set => _maxObjectWeight = value; }
        public float DropVelocityReduction { get => _dropVelocityReduction; set => _dropVelocityReduction = value; }

        // Throw parameters
        public float ThrowStrength { get => _throwStrength; set => _throwStrength = value; }

        // Picked up object zoom parameters
        public float ZoomInterval { get => _zoomInterval; set => _zoomInterval = value; }
        public int IntervalCount { get => _intervalCount; set => _intervalCount = value; }


        public PlayerInteraction(PlayerInteractionData _data)
        {
            _playerLayerMask = _data.PlayerLayerMask;
            _interactRange = _data.InteractRange;
            _sphereCheckRange = _data.SphereCheckRange;
            _hitPosition = _data.HitPosition;
            _perColliderHitPosition = _data.PerColliderHitPosition;
            _colliderAraySize = _data.ColliderAraySize;
            _interactiblesMask = _data.InteractiblesMask;
            _excludeCollisionMask = _data.ExcludeCollisionMask;

            _maxPickupPointDistance = _data.MaxPickupPointDistance;
            _minPickupPointDistance = _data.MinPickupPointDistance;
            _maxDistanceFromPoint = _data.MaxDistanceFromPoint;
            _pickupPointDistance = _data.PickupPointDistance;
            _maxObjectWeight = _data.MaxObjectWeight;
            _dropVelocityReduction = _data.DropVelocityReduction;

            _throwStrength = _data.ThrowStrength;

            _intervalCount = _data.IntervalCount;
            _zoomInterval = _data.ZoomInterval;
        }

        public void InvokePickUpObject()
        {
            OnPickUpObject?.Invoke();
        }

        public void InvokeDropObject()
        {
            OnDropObject?.Invoke();
        }
    }

}
