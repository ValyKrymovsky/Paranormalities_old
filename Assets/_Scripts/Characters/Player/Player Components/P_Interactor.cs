using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;

namespace MyCode.Player
{
    public class P_Interactor : MonoBehaviour
    {
        private PlayerManager _playerManager;
        private PopupManager _popupManager;
        private GameObject _player;
        private Collider _playerCollider;

        public GameObject pickupPoint;

        private InteractionController _interactionController;

        

        List<Collider> interactibleColliders = new List<Collider>();
        void Awake()
        {
            _playerManager = PlayerManager.Instance;
            _popupManager = PopupManager.Instance;

            _playerManager.InteractionData.PickupPoint = pickupPoint;

            _player = GameObject.FindGameObjectWithTag("Player");
            _playerCollider = _player.GetComponent<Collider>();
        }

        private void Start()
        {
            _playerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _playerManager.InteractionData.PickupPoint.transform.position);
            PickupPointDistanceCorrection();

            _playerManager.InteractionData.ZoomInterval = (_playerManager.InteractionData.MaxPickupPointDistance - _playerManager.InteractionData.MinPickupPointDistance) / 50;

            _playerManager.InteractionData.ColliderArray = new Collider[5];
        }

        private void OnEnable()
        {
            _playerManager.InteractionData.Input_InteractValue.action.started += Interact;
            _playerManager.InteractionData.Input_InteractValue.action.canceled += Drop;
            _playerManager.InteractionData.Input_ThrowValue.action.performed += Throw;
            _playerManager.InteractionData.Input_ZoomValue.action.performed += _ctx => Zoom(_ctx);
        }

        private void OnDisable()
        {
            _playerManager.InteractionData.Input_InteractValue.action.started -= Interact;
            _playerManager.InteractionData.Input_InteractValue.action.canceled -= Drop;
            _playerManager.InteractionData.Input_ThrowValue.action.performed -= Throw;
            _playerManager.InteractionData.Input_ZoomValue.action.performed -= _ctx => Zoom(_ctx);
        }

        private void Update() 
        {
            if (!_playerManager.InteractionData.ObjectRigidbody)
            {
                CheckInteractibles();
            } 
        }

        private void FixedUpdate()
        {
            _playerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _playerManager.InteractionData.PickupPoint.transform.position);

            PickupPointDistanceCorrection();

            if (_playerManager.InteractionData.ObjectRigidbody)
            {
                if (Vector3.Distance(_playerManager.InteractionData.ObjectRigidbody.transform.position, _playerManager.InteractionData.PickupPoint.transform.position) > _playerManager.InteractionData.MaxDistanceFromPoint)
                {
                    _playerManager.InteractionData.ObjectCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;
                    ResetRigidbodyParameters();
                    _playerManager.InteractionData.ObjectCollider = null;
                }

                // Object movement
                _playerManager.InteractionData.ObjectRigidbody.angularVelocity = Vector3.zero;
                Vector3 DirectionToPoint = _playerManager.InteractionData.PickupPoint.transform.position - _playerManager.InteractionData.ObjectRigidbody.transform.position;
                _playerManager.InteractionData.ObjectRigidbody.AddForce(DirectionToPoint * _playerManager.InteractionData.PickupPointDistance * 500f, ForceMode.Acceleration);

                _playerManager.InteractionData.ObjectRigidbody.velocity = Vector3.zero;
            }
        }

        public void Interact(InputAction.CallbackContext _value)
        {
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
            Ray r = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            Physics.Raycast(r, out hit, _playerManager.InteractionData.InteractRange);

            _playerManager.InteractionData.HitPosition = hit.collider != null ? hit.point : transform.position + transform.forward * _playerManager.InteractionData.InteractRange;

            int results = Physics.OverlapSphereNonAlloc(_playerManager.InteractionData.HitPosition, _playerManager.InteractionData.SphereCheckRange, _playerManager.InteractionData.ColliderArray, _playerManager.InteractionData.InteractiblesMask);

            Collider nearestInteractibleCollider = null;

            if (results <= 0)
            {
                if (_popupManager.PopupObject.IsVisible())
                    _popupManager.PopupObject.SetVisibility(false);

                interactibleColliders.Clear();
                _interactionController = null;
                _playerManager.InteractionData.SelectedCollider = null;
                return;
            }

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
                _playerManager.InteractionData.SelectedCollider = null;
                _interactionController = null;
                return;
            }
                

            // finds the nearest collider
            float minDistance = -1;
            foreach (Collider collider in interactibleColliders)
            {
                if (minDistance == -1)
                {
                    minDistance = Vector3.Distance(_playerManager.InteractionData.HitPosition, collider.transform.position);
                    nearestInteractibleCollider = collider;
                    continue;
                }
                else
                {
                    float distance = Vector3.Distance(_playerManager.InteractionData.HitPosition, collider.transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestInteractibleCollider = collider;
                        continue;
                    }
                }
            }


            if (_playerManager.InteractionData.SelectedCollider != nearestInteractibleCollider)
            {
                _playerManager.InteractionData.SelectedCollider = nearestInteractibleCollider;
                _interactionController = nearestInteractibleCollider.GetComponent<InteractionController>();
                _popupManager.PopupObject.ChangeTransform(nearestInteractibleCollider.transform);
                _popupManager.PopupObject.Text.text = _interactionController.PopupText;
            }

            if (!_popupManager.PopupObject.IsVisible())
                _popupManager.PopupObject.SetVisibility(true);

            _popupManager.PopupObject.transform.LookAt(transform, transform.up);

            float proximityTextOpacity = Mathf.InverseLerp(_playerManager.InteractionData.SphereCheckRange, 0, Vector3.Distance(_playerManager.InteractionData.HitPosition, _popupManager.PopupObject.transform.position));
            _popupManager.PopupObject.SetTextOpacity(proximityTextOpacity);

            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 popupPosition = new Vector2(_popupManager.PopupObject.transform.position.x, _popupManager.PopupObject.transform.position.z);
            float proximityTextSize = Mathf.InverseLerp(_playerManager.InteractionData.InteractRange + _playerManager.InteractionData.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _popupManager.Popup.MaxTextSize;
            if (proximityTextSize < _popupManager.Popup.MinTextSize) proximityTextSize = _popupManager.Popup.MinTextSize;
            _popupManager.PopupObject.SetTextSize(proximityTextSize);

            interactibleColliders.Clear();
        }

        public void PickUp()
        {
            Ray r = new Ray(transform.position, transform.forward);

            if (Physics.SphereCast(transform.position, .25f, transform.forward, out RaycastHit hitInfo, _playerManager.InteractionData.MaxPickupDistance, _playerManager.InteractionData.InteractiblesMask))
            {
                if (_interactionController.InteractionType != InteractionType.PickUp)
                    return;

                float objectWeight = -Physics.gravity.y * hitInfo.rigidbody.mass;

                if (objectWeight >= _playerManager.InteractionData.MaxObjectWeight)
                {
                    Debug.Log("Object too heavy!");
                    return;
                }

                _playerManager.InteractionData.ObjectRigidbody = hitInfo.rigidbody;

                _playerManager.InteractionData.ObjectCollider = _playerManager.InteractionData.ObjectRigidbody.GetComponent<Collider>();

                _playerManager.InteractionData.ExcludeCollisionMask = _playerManager.InteractionData.ObjectCollider.excludeLayers;

                _playerManager.InteractionData.ObjectCollider.excludeLayers = _playerManager.InteractionData.PlayerLayerMask;

                _playerManager.InteractionData.ObjectMass = _playerManager.InteractionData.ObjectRigidbody.mass;
                _playerManager.InteractionData.ObjectDrag = _playerManager.InteractionData.ObjectRigidbody.drag;
                _playerManager.InteractionData.ObjectAngularDrag = _playerManager.InteractionData.ObjectRigidbody.angularDrag;

                _playerManager.InteractionData.ObjectRigidbody.useGravity = false;
                _playerManager.InteractionData.ObjectRigidbody.angularDrag = 200f;
                float distanceToObject = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_playerManager.InteractionData.ObjectRigidbody.transform.position.x, _playerManager.InteractionData.ObjectRigidbody.transform.position.z));
                _playerManager.InteractionData.PickupPoint.transform.position = transform.position + transform.forward * distanceToObject;

                _playerManager.InteractionData.InvokePickUpObject();
            }
        }

        public void Drop(InputAction.CallbackContext _context)
        {
            if (!_playerManager.InteractionData.ObjectRigidbody)
            {
                return;
            }

            _playerManager.InteractionData.ObjectCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

            ResetRigidbodyParameters();
            _playerManager.InteractionData.ObjectCollider = null;
            _playerManager.InteractionData.InvokeDropObject();
        }

        public void Throw(InputAction.CallbackContext _context)
        {
            if (!_playerManager.InteractionData.ObjectRigidbody)
            {
                return;
            }

            if (_context.phase == InputActionPhase.Performed)
            {
                _playerManager.InteractionData.ObjectRigidbody.AddForce(transform.position + transform.forward * (_playerManager.InteractionData.ThrowStrength * 100), ForceMode.Acceleration);


                _playerManager.InteractionData.ObjectCollider.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

                ResetRigidbodyParameters();

                _playerManager.InteractionData.InvokeDropObject();
            }
        }

        public void Zoom(InputAction.CallbackContext _context)
        {

        }

        private void PickupPointDistanceCorrection()
        {
            if (_playerManager.InteractionData.PickupPointDistance > _playerManager.InteractionData.MaxPickupPointDistance)
            {
                _playerManager.InteractionData.PickupPoint.transform.position = transform.position + transform.forward * _playerManager.InteractionData.MaxPickupPointDistance;
            }
            else if (_playerManager.InteractionData.PickupPointDistance < _playerManager.InteractionData.MinPickupPointDistance)
            {
                _playerManager.InteractionData.PickupPoint.transform.position = transform.position + transform.forward * _playerManager.InteractionData.MinPickupPointDistance;
            }
        }

        public void ResetRigidbodyParameters()
        {
            _playerManager.InteractionData.ObjectRigidbody.excludeLayers = _playerManager.InteractionData.ExcludeCollisionMask;

            _playerManager.InteractionData.ObjectRigidbody.useGravity = true;
            _playerManager.InteractionData.ObjectRigidbody.angularDrag = _playerManager.InteractionData.ObjectAngularDrag;
            _playerManager.InteractionData.ObjectRigidbody.drag = _playerManager.InteractionData.ObjectDrag;
            _playerManager.InteractionData.ObjectRigidbody.mass = _playerManager.InteractionData.ObjectMass;

            _playerManager.InteractionData.ObjectRigidbody.velocity /= _playerManager.InteractionData.DropVelocityReduction;
            _playerManager.InteractionData.ObjectRigidbody = null;

            _playerManager.InteractionData.ObjectAngularDrag = 0;
            _playerManager.InteractionData.ObjectDrag = 0;
            _playerManager.InteractionData.ObjectMass = 0;
            return;
        }

    }
}