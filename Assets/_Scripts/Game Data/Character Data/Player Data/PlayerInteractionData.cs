using UnityEngine;
using System;
using MyBox;


namespace MyCode.GameData.PlayerData
{
    [CreateAssetMenu(fileName = "NewInteractionData", menuName = "DataObjects/Player/Interaction")]
    public class PlayerInteractionData : ScriptableObject
    {
        [Space]
        [Separator("Interaction", true)]
        [Space]

        [Header("Player")]
        [Space]
        [SerializeField] private LayerMask _playerLayerMask;

        [Header("Interaction parameters")]
        [Space]
        [SerializeField] private float _interactRange;
        [SerializeField] private float _sphereCheckRange;
        [SerializeField, ReadOnly] private Vector3 _hitPosition;
        [SerializeField, ReadOnly] private Vector3 _perColliderHitPosition;
        [SerializeField] private int _colliderAraySize;
        [SerializeField] private LayerMask _interactiblesMask;
        private LayerMask _excludeCollisionMask;
        [Space]

        [Space]
        [Separator("Pickup", true)]
        [Space]

        [Header("Distances")]
        [Space]
        [SerializeField] private float _maxPickupPointDistance;
        [SerializeField] private float _minPickupPointDistance;
        [Tooltip("Defines the maximum distance between object and pickup point before dropping the object")]
        [SerializeField] private float _maxDistanceFromPoint;
        [SerializeField, ReadOnly] private float _pickupPointDistance;
        [Space]
        [SerializeField] private float _maxObjectWeight;
        [Space]
        [Tooltip("Reduces the velocity by a multiplier. (Useful for adding more weight to objects)")]
        [SerializeField, MinValue(1)] private float _dropVelocityReduction;
        [Space]

        [Header("Throw")]
        [Space]
        [SerializeField] private float _throwStrength;
        [Space]

        [Header("Zoom")]
        [Space]
        [SerializeField] private int _intervalCount;
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
