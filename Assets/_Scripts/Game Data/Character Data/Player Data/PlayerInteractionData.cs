using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using MyBox;


namespace MyCode.Data.Player
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
        [SerializeField] private LayerMask _interactiblesMask;
        [SerializeField, ReadOnly] private Collider _selectedCollider;
        private Collider[] colliderArray;
        private LayerMask _excludeCollisionMask;
        [Space]

        [Space]
        [Separator("Pickup", true)]
        [Space]

        [Header("Pickup Point")]
        [Space]
        [SerializeField] private GameObject _pickupPoint;
        [Space]

        [Header("Picked up object")]
        [Space]
        [SerializeField, ReadOnly] private Rigidbody _objectRigidbody;
        [SerializeField, ReadOnly] private Collider _objectCollider;
        private float _objectMass = 0;
        private float _objectDrag = 0;
        private float _objectAngularDrag = 0;
        [Space]

        [Header("Distances")]
        [Space]
        [SerializeField] private float _maxPickupDistance;
        [SerializeField] private float _maxPickupPointDistance;
        [SerializeField] private float _minPickupPointDistance;
        [Tooltip("Defines the maximum distance between object and pickup point before dropping the object")]
        [SerializeField] private float _maxDistanceFromPoint;
        [SerializeField, ReadOnly] private float _pickupPointDistance;
        [Space]
        [SerializeField] private LayerMask _pickupLayer;
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
        private float _zoomInterval;

        [Space]
        [Separator("Inputs", true)]
        [Space]

        [Header("Input Action")]
        [Space]
        [SerializeField] private InputActionReference _input_InteractValue;
        [SerializeField] private InputActionReference _input_ThrowValue;
        [SerializeField] private InputActionReference _input_ZoomValue;

        public event Action OnPickUpObject;
        public event Action OnDropObject;
        public event Action OnInteract;

        // Player
        public LayerMask PlayerLayerMask { get => _playerLayerMask; set => _playerLayerMask = value; }

        // Interaction
        public float InteractRange { get => _interactRange; set => _interactRange = value; }
        public float SphereCheckRange { get => _sphereCheckRange; set => _sphereCheckRange = value; }
        public Vector3 HitPosition { get => _hitPosition; set => _hitPosition = value; }
        public Vector3 PerColliderHitPosition { get => _perColliderHitPosition; set => _perColliderHitPosition = value; }
        public LayerMask InteractiblesMask { get => _interactiblesMask; set => _interactiblesMask = value; }
        public Collider SelectedCollider { get => _selectedCollider; set => _selectedCollider = value; }
        public Collider[] ColliderArray { get => colliderArray; set => colliderArray = value; }
        public LayerMask ExcludeCollisionMask { get => _excludeCollisionMask; set => _excludeCollisionMask = value; }


        // Pickup point
        public GameObject PickupPoint { get => _pickupPoint; set => _pickupPoint = value; }


        // Picked up object
        public Rigidbody ObjectRigidbody { get => _objectRigidbody; set => _objectRigidbody = value; }
        public Collider ObjectCollider { get => _objectCollider; set => _objectCollider = value; }
        public float ObjectMass { get => _objectMass; set => _objectMass = value; }
        public float ObjectDrag { get => _objectDrag; set => _objectDrag = value; }
        public float ObjectAngularDrag { get => _objectAngularDrag; set => _objectAngularDrag = value; }



        // Pickup distance parameters
        public float MaxPickupDistance { get => _maxPickupDistance; set => _maxPickupDistance = value; }
        public float MaxPickupPointDistance { get => _maxPickupPointDistance; set => _maxPickupPointDistance = value; }
        public float MinPickupPointDistance { get => _minPickupPointDistance; set => _minPickupPointDistance = value; }
        public float MaxDistanceFromPoint { get => _maxDistanceFromPoint; set => _maxDistanceFromPoint = value; }
        public float PickupPointDistance { get => _pickupPointDistance; set => _pickupPointDistance = value; }
        public LayerMask PickupLayer { get => _pickupLayer; }
        public float MaxObjectWeight { get => _maxObjectWeight; set => _maxObjectWeight = value; }
        public float DropVelocityReduction { get => _dropVelocityReduction; set => _dropVelocityReduction = value; }

        // Throw parameters
        public float ThrowStrength { get => _throwStrength; set => _throwStrength = value; }

        // Picked up object zoom parameters
        public float ZoomInterval { get => _zoomInterval; set => _zoomInterval = value; }

        // Inputs
        public InputActionReference Input_InteractValue { get => _input_InteractValue; set => _input_InteractValue = value; }
        public InputActionReference Input_ThrowValue { get => _input_ThrowValue; set => _input_ThrowValue = value; }
        public InputActionReference Input_ZoomValue { get => _input_ZoomValue; set => _input_ZoomValue = value; }

        public void InvokePickUpObject()
        {
            OnPickUpObject?.Invoke();
        }

        public void InvokeDropObject()
        {
            OnDropObject?.Invoke();
        }

        public void InvokeOnInteract()
        {
            OnInteract?.Invoke();
        }

        private void OnEnable()
        {
            _input_InteractValue.action.Enable();
            _input_ThrowValue.action.Enable();
            _input_ZoomValue.action.Enable();
        }

        private void OnDisable()
        {
            _input_InteractValue.action.Disable();
            _input_ThrowValue.action.Disable();
            _input_ZoomValue.action.Disable();
        }
    }

}
