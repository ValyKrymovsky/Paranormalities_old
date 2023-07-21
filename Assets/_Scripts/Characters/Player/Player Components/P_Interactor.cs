using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using MyCode.Managers;
using MyCode.Player.Interaction;

namespace MyCode.Player.Components
{
    public class P_Interactor : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private PopupManager _popupManager;

        private bool canInteract = true;
        private bool canCheckInteractibles = true;

        [SerializeField] private GameObject _pickupPoint;

        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _selectedCollider;

        private float _mass;
        private float _drag;
        private float _angularDrag;

        private InteractionController _interactionController;

        

        List<Collider> interactibleColliders = new List<Collider>();
        void Awake()
        {
            _playerManager = PlayerManager.Instance;
            _popupManager = PopupManager.Instance;
        }

        private void Start()
        {
            _playerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _pickupPoint.transform.position);
            PickupPointDistanceCorrection();

            _playerManager.InteractionData.ZoomInterval = (_playerManager.InteractionData.MaxPickupPointDistance - _playerManager.InteractionData.MinPickupPointDistance) / _playerManager.InteractionData.IntervalCount;

            _playerManager.InteractionData.ColliderArray = new Collider[5];
        }

        private void OnEnable()
        {
            _playerManager.InteractionData.Input_InteractValue.action.started += Interact;
            _playerManager.InteractionData.Input_InteractValue.action.canceled += Drop;
            _playerManager.InteractionData.Input_ThrowValue.action.performed += Throw;
            _playerManager.InteractionData.Input_ZoomValue.action.performed += _ctx => Zoom(_ctx);

            _playerManager.InventoryData.OnInventoryStatusChange += value => canInteract = !value;
            _playerManager.InteractionData.OnPickUpObject += () => canCheckInteractibles = false;
            _playerManager.InteractionData.OnDropObject += () => canCheckInteractibles = true;
        }

        private void OnDisable()
        {
            _playerManager.InteractionData.Input_InteractValue.action.started -= Interact;
            _playerManager.InteractionData.Input_InteractValue.action.canceled -= Drop;
            _playerManager.InteractionData.Input_ThrowValue.action.performed -= Throw;
            _playerManager.InteractionData.Input_ZoomValue.action.performed -= _ctx => Zoom(_ctx);

            _playerManager.InventoryData.OnInventoryStatusChange -= value => canInteract = !value;
            _playerManager.InteractionData.OnPickUpObject -= () => canCheckInteractibles = false;
            _playerManager.InteractionData.OnDropObject -= () => canCheckInteractibles = true;
        }

        private void Update() 
        {
            CheckInteractibles();
        }

        private void FixedUpdate()
        {
            _playerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _pickupPoint.transform.position);

            PickupPointDistanceCorrection();

            if (_rb)
            {
                if (Vector3.Distance(_rb.transform.position, _pickupPoint.transform.position) > _playerManager.InteractionData.MaxDistanceFromPoint)
                {
                    _selectedCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;
                    ResetRigidbodyParameters();
                    _selectedCollider = null;
                    _playerManager.InteractionData.InvokeDropObject();
                    return;
                }

                // Object movement
                _rb.angularVelocity = Vector3.zero;
                Vector3 DirectionToPoint = _pickupPoint.transform.position - _rb.transform.position;
                _rb.AddForce(DirectionToPoint * _playerManager.InteractionData.PickupPointDistance * 500f, ForceMode.Acceleration);

                _rb.velocity = Vector3.zero;
            }
        }

        public void Interact(InputAction.CallbackContext _value)
        {
            if (!canInteract) return;

            try
            {
                if (_interactionController == null)
                    return;

                if (_interactionController.InteractionType == InteractionType.Interact)
                {
                    _interactionController.Interact();
                    return;
                }
                else if (_interactionController.InteractionType == InteractionType.PickUp)
                {
                    PickUp();
                    return;
                }
                
                
                
            }
            catch(NullReferenceException)
            {
                Debug.Log("InteractionData script not found");
            }
            catch(UnassignedReferenceException)
            {
                Debug.Log("Object cannot be interacted with");
            }
        }

        public void CheckInteractibles()
        {
            if (!canInteract) return;
            if (!canCheckInteractibles) return;

            Ray r = new Ray(transform.position, transform.forward);

            Physics.Raycast(r, out RaycastHit hitInfo, _playerManager.InteractionData.InteractRange);

            _playerManager.InteractionData.HitPosition = hitInfo.collider != null ? hitInfo.point : transform.position + transform.forward * _playerManager.InteractionData.InteractRange;

            if (hitInfo.collider != null)
            {
                if (!hitInfo.collider.TryGetComponent(out InteractionController controller)) return;

                if (!controller.Interactible) return;

                if (_selectedCollider != hitInfo.collider)
                {
                    UpdateSelectedCollider(hitInfo.collider, controller);
                }

                if (!_popupManager.PopupData.IsVisible)
                    _popupManager.PopupData.InvokeOnVisibilityChange(true);

                Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 popupPosition = new Vector2(_popupManager.PopupObject.transform.position.x, _popupManager.PopupObject.transform.position.z);
                float proximityTextSize = Mathf.InverseLerp(_playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.PopupData.MaxTextSize;
                if (proximityTextSize < _popupManager.PopupData.MinTextSize) proximityTextSize = _popupManager.PopupData.MinTextSize;

                _popupManager.PopupObject.transform.LookAt(transform, transform.up);

                UpdateText(proximityTextSize, 1);

                interactibleColliders.Clear();
                return;
            }

            int results = Physics.OverlapSphereNonAlloc(_playerManager.InteractionData.HitPosition, _playerManager.InteractionData.SphereCheckRange, _playerManager.InteractionData.ColliderArray, _playerManager.InteractionData.InteractiblesMask);

            Collider nearestInteractibleCollider = null;

            if (results == 0)
            {
                if (_popupManager.PopupData.IsVisible)
                    _popupManager.PopupData.InvokeOnVisibilityChange(false);

                interactibleColliders.Clear();
                _interactionController = null;
                _selectedCollider = null;
                _playerManager.InteractionData.PerColliderHitPosition = Vector3.zero;
                return;
            }

            if (!_popupManager.PopupData.IsVisible)
                _popupManager.PopupData.InvokeOnVisibilityChange(true);

            // Checks if all colliders are interactible and adding them to a separate list if they are
            for (int i = 0; i < results; i++)
            {
                if (_playerManager.InteractionData.ColliderArray[i].TryGetComponent(out InteractionController tempIntController))
                {
                    if (tempIntController.Interactible) interactibleColliders.Add(_playerManager.InteractionData.ColliderArray[i]);
                }
            }

            if (interactibleColliders.Count <= 0)
            {
                interactibleColliders.Clear();
                _selectedCollider = null;
                _interactionController = null;
                _playerManager.InteractionData.PerColliderHitPosition = Vector3.zero;
                return;
            }


            // finds the nearest collider
            float minDistance = -1;
            foreach (Collider collider in interactibleColliders)
            {
                Ray ray = new(_playerManager.InteractionData.HitPosition, collider.transform.position - _playerManager.InteractionData.HitPosition);
                collider.Raycast(ray, out RaycastHit perColliderHitInfo, _playerManager.InteractionData.SphereCheckRange);
                if (minDistance == -1)
                {
                    minDistance = Vector3.Distance(_playerManager.InteractionData.HitPosition, perColliderHitInfo.point);
                    _playerManager.InteractionData.PerColliderHitPosition = perColliderHitInfo.point;
                    nearestInteractibleCollider = collider;
                    continue;
                }
                else
                {
                    float distance = Vector3.Distance(_playerManager.InteractionData.HitPosition, perColliderHitInfo.point);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        _playerManager.InteractionData.PerColliderHitPosition = perColliderHitInfo.point;
                        nearestInteractibleCollider = collider;
                        continue;
                    }
                }
            }


            if (_selectedCollider != nearestInteractibleCollider)
            {
                UpdateSelectedCollider(nearestInteractibleCollider);
            }

            if (!_popupManager.PopupData.IsVisible)
                _popupManager.PopupData.IsVisible = true;

            _popupManager.PopupObject.transform.LookAt(transform, transform.up);
            UpdateText();

            interactibleColliders.Clear();
        }

        private void UpdateSelectedCollider(Collider _newCollider)
        {
            _selectedCollider = _newCollider;
            _interactionController = _newCollider.GetComponent<InteractionController>();
            _popupManager.PopupData.InvokeOnParentChange(_newCollider.transform);
            _popupManager.PopupObject.Text.text = _interactionController.PopupText;
        }

        private void UpdateSelectedCollider(Collider _newCollider, InteractionController _newController)
        {
            _selectedCollider = _newCollider;
            _interactionController = _newController;
            _popupManager.PopupData.InvokeOnParentChange(_newCollider.transform);
            _popupManager.PopupObject.Text.text = _interactionController.PopupText;
        }

        private void UpdateText()
        {
            float proximityTextOpacity = Mathf.InverseLerp(_playerManager.InteractionData.SphereCheckRange, 0, Vector3.Distance(_playerManager.InteractionData.HitPosition, _playerManager.InteractionData.PerColliderHitPosition));
            _popupManager.PopupData.InvokeOnOpacityChange(proximityTextOpacity);

            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 popupPosition = new Vector2(_playerManager.InteractionData.PerColliderHitPosition.x, _playerManager.InteractionData.PerColliderHitPosition.z);
            float proximityTextSize = Mathf.InverseLerp(_playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.PopupData.MaxTextSize;
            if (proximityTextSize < _popupManager.PopupData.MinTextSize) proximityTextSize = _popupManager.PopupData.MinTextSize;
            _popupManager.PopupData.InvokeOnSizeChange(proximityTextSize);
        }

        private void UpdateText(float _size, float _opacity)
        {
            _popupManager.PopupData.InvokeOnOpacityChange(_opacity);

            _popupManager.PopupData.InvokeOnSizeChange(_size);
        }

        public void PickUp()
        {
            if (!canInteract) return;

            if (!_selectedCollider) return;

            if (_interactionController.InteractionType != InteractionType.PickUp) return;

            _rb = _selectedCollider.GetComponent<Rigidbody>();
            float objectWeight =_rb.mass;

            if (objectWeight >= _playerManager.InteractionData.MaxObjectWeight)
            {
                Debug.Log("Object too heavy!");
                _rb = null;
                return;
            }

            _playerManager.InteractionData.ExcludeCollisionMask = _selectedCollider.excludeLayers;

            _selectedCollider.excludeLayers = _playerManager.InteractionData.PlayerLayerMask;

            _mass = _rb.mass;
            _drag = _rb.drag;
            _angularDrag = _rb.angularDrag;

            _rb.useGravity = false;
            _rb.angularDrag = 200f;
            float distanceToObject = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_rb.transform.position.x, _rb.transform.position.z));
            _pickupPoint.transform.position = transform.position + transform.forward * distanceToObject;

            _playerManager.InteractionData.InvokePickUpObject();
        }

        public void Drop(InputAction.CallbackContext _context)
        {
            if (!_rb)
            {
                return;
            }

            _selectedCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

            ResetRigidbodyParameters();
            _selectedCollider = null;
            _playerManager.InteractionData.InvokeDropObject();
        }

        public void Throw(InputAction.CallbackContext _context)
        {
            if (!_rb)
            {
                return;
            }

            if (_context.phase == InputActionPhase.Performed)
            {
                _rb.AddForce(transform.position + transform.forward * (_playerManager.InteractionData.ThrowStrength * 100), ForceMode.Acceleration);


                _selectedCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

                ResetRigidbodyParameters();

                _playerManager.InteractionData.InvokeDropObject();
            }
        }

        public void Zoom(InputAction.CallbackContext _context)
        {
            float zoomValue = _context.ReadValue<float>() / 120;

            if (zoomValue == 0) return;

            _pickupPoint.transform.position = transform.position + transform.forward * (_playerManager.InteractionData.PickupPointDistance + _playerManager.InteractionData.ZoomInterval * zoomValue);
            PickupPointDistanceCorrection();
        }

        private void PickupPointDistanceCorrection()
        {
            if (_playerManager.InteractionData.PickupPointDistance > _playerManager.InteractionData.MaxPickupPointDistance)
            {
                _pickupPoint.transform.position = transform.position + transform.forward * _playerManager.InteractionData.MaxPickupPointDistance;
                _playerManager.InteractionData.PickupPointDistance = _playerManager.InteractionData.MaxPickupPointDistance;
            }
            else if (_playerManager.InteractionData.PickupPointDistance < _playerManager.InteractionData.MinPickupPointDistance)
            {
                _pickupPoint.transform.position = transform.position + transform.forward * _playerManager.InteractionData.MinPickupPointDistance;
                _playerManager.InteractionData.PickupPointDistance = _playerManager.InteractionData.MinPickupPointDistance;
            }
        }

        public void ResetRigidbodyParameters()
        {
            _rb.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

            _rb.useGravity = true;
            _rb.angularDrag = _angularDrag;
            _rb.drag = _drag;
            _rb.mass = _mass;

            _rb.velocity /= _playerManager.InteractionData.DropVelocityReduction;
            _rb = null;

            _angularDrag = 0;
            _drag = 0;
            _mass = 0;
            return;
        }
    }
}