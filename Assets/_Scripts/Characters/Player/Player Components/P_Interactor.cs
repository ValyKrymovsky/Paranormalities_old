using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using MyCode.Managers;
using MyCode.GameData.Interaction;
using MyCode.GameData.Inventory;

namespace MyCode.PlayerComponents
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

        [SerializeField] private Collider[] _colliderArray;

        private Vector3 _colliderHitPosition;

        private float _mass;
        private float _drag;
        private float _angularDrag;

        [SerializeField] private InteractionController _interactionController;

        [SerializeField] private InputActionReference _input_InteractValue;
        [SerializeField] private InputActionReference _input_ThrowValue;
        [SerializeField] private InputActionReference _input_ZoomValue;



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

            _colliderArray = new Collider[_playerManager.InteractionData.ColliderAraySize];
        }

        private void OnEnable()
        {
            _input_InteractValue.action.Enable();
            _input_ThrowValue.action.Enable();
            _input_ZoomValue.action.Enable();

            _input_InteractValue.action.performed += Interact;
            _input_InteractValue.action.canceled += Drop;
            _input_ThrowValue.action.performed += Throw;
            _input_ZoomValue.action.performed += _ctx => Zoom(_ctx);

            _playerManager.InventoryData.OnInventoryStatusChange += value => canInteract = !value;
            _playerManager.InteractionData.OnPickUpObject += () => canCheckInteractibles = false;
            _playerManager.InteractionData.OnDropObject += () => canCheckInteractibles = true;
        }

        private void OnDisable()
        {
            _input_InteractValue.action.Disable();
            _input_ThrowValue.action.Disable();
            _input_ZoomValue.action.Disable();

            _input_InteractValue.action.performed -= Interact;
            _input_InteractValue.action.canceled -= Drop;
            _input_ThrowValue.action.performed -= Throw;
            _input_ZoomValue.action.performed -= _ctx => Zoom(_ctx);

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

            if (_selectedCollider == null)
                return;

            float distanceToCollider = Vector3.Distance(_playerManager.InteractionData.HitPosition, _colliderHitPosition);

            
            if (distanceToCollider > _playerManager.InteractionData.MaxInteractDistance)
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

        private float CalculateTextSize(float maxDistance)
        {
            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 popupPosition = new Vector2(_popupManager.PopupObject.transform.position.x, _popupManager.PopupObject.transform.position.z);
            float proximityTextSize = Mathf.InverseLerp(0, maxDistance, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.PopupData.MaxTextSize;

            if (proximityTextSize < _popupManager.PopupData.MinTextSize) proximityTextSize = _popupManager.PopupData.MinTextSize;
            return proximityTextSize;
        }

        public void CheckInteractibles()
        {
            if (!canInteract) return;
            if (!canCheckInteractibles) return;

            Ray r = new Ray(transform.position, transform.forward);

            // Cast ray in front of the camera with InteractRange as max distance
            Physics.Raycast(r, out RaycastHit hitInfo, _playerManager.InteractionData.InteractRange);
            _playerManager.InteractionData.HitPosition = hitInfo.collider != null ? hitInfo.point : transform.position + transform.forward * _playerManager.InteractionData.InteractRange;

            // Checks if raycast has hit any collider
            // Then it checks whether the collider is interactible or not
            // If the collider is interactible, then the _selectedCollider is updated along with interaction popup location, opacity and size
            if (hitInfo.collider != null)
            {
                if (!hitInfo.collider.TryGetComponent(out InteractionController controller)) goto Continue;

                if (!controller.Interactible) goto Continue;

                if (_selectedCollider != hitInfo.collider)
                {
                    UpdateSelectedCollider(hitInfo.collider, controller);
                }

                _colliderHitPosition = _playerManager.InteractionData.HitPosition;

                if (!_popupManager.PopupData.IsVisible)
                    _popupManager.PopupData.InvokeOnVisibilityChange(true);

                /*
                Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 popupPosition = new Vector2(_popupManager.PopupObject.transform.position.x, _popupManager.PopupObject.transform.position.z);
                float proximityTextSize = Mathf.InverseLerp(_playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.PopupData.MaxTextSize;
                if (proximityTextSize < _popupManager.PopupData.MinTextSize) proximityTextSize = _popupManager.PopupData.MinTextSize;
                */

                _popupManager.PopupObject.transform.LookAt(transform, transform.up);

                float distanceFromPopup = _playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange;

                UpdateText(CalculateTextSize(distanceFromPopup), 1);

                interactibleColliders.Clear();
                return;
            }

            Continue:
            
            int results = Physics.OverlapSphereNonAlloc(_playerManager.InteractionData.HitPosition, _playerManager.InteractionData.SphereCheckRange, _colliderArray, _playerManager.InteractionData.InteractiblesMask);

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
                if (_colliderArray[i].TryGetComponent(out InteractionController tempIntController))
                {
                    if (tempIntController.Interactible) interactibleColliders.Add(_colliderArray[i]);
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
                    _colliderHitPosition = perColliderHitInfo.point;
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
                        _colliderHitPosition = perColliderHitInfo.point;
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

            if (_newController.CustomPopupLocation)
                _popupManager.PopupData.InvokeOnParentChange(_newController.customPopupLocationObject.transform);
            else
                _popupManager.PopupData.InvokeOnParentChange(_newCollider.transform);

            _popupManager.PopupObject.Text.text = _interactionController.PopupText;
        }

        private void UpdateText()
        {
            float proximityTextOpacity = Mathf.InverseLerp(_playerManager.InteractionData.SphereCheckRange, 0, Vector3.Distance(_playerManager.InteractionData.HitPosition, _playerManager.InteractionData.PerColliderHitPosition));
            _popupManager.PopupData.InvokeOnOpacityChange(proximityTextOpacity);

            /*
            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 popupPosition = new Vector2(_playerManager.InteractionData.PerColliderHitPosition.x, _playerManager.InteractionData.PerColliderHitPosition.z);
            float proximityTextSize = Mathf.InverseLerp(_playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.PopupData.MaxTextSize;
            if (proximityTextSize < _popupManager.PopupData.MinTextSize) proximityTextSize = _popupManager.PopupData.MinTextSize;
            */

            float distanceFromPopup = _playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange;

            _popupManager.PopupData.InvokeOnSizeChange(CalculateTextSize(distanceFromPopup));
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

        private void OnDrawGizmos()
        {
            if (_selectedCollider != null)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(transform.position + (transform.forward * _playerManager.InteractionData.InteractRange), _playerManager.InteractionData.SphereCheckRange);

            if (_selectedCollider != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position + (transform.forward * _playerManager.InteractionData.InteractRange), _selectedCollider.transform.position);
            }  
        }
    }
}