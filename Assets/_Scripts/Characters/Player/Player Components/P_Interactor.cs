using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using MyBox;

namespace MyCode.Player
{
    public class P_Interactor : MonoBehaviour
    {
        private PlayerManager _pm;
        private GameObject _player;
        private Collider _playerCollider;

        public GameObject pickupPoint;

        private Transform _tempLocation;
        void Awake()
        {
            _pm = PlayerManager.Instance;

            _pm.Interaction.PickupPoint = pickupPoint;

            _player = GameObject.FindGameObjectWithTag("Player");
            _playerCollider = _player.GetComponent<Collider>();

            _tempLocation = GameObject.Find("TempLocation").transform;


        }

        private void Start()
        {
            _pm.Interaction.PickupPointDistance = Vector3.Distance(transform.position, _pm.Interaction.PickupPoint.transform.position);
            PickupPointDistanceCorrection();

            _pm.Interaction.ZoomInterval = (_pm.Interaction.MaxPickupPointDistance - _pm.Interaction.MinPickupPointDistance) / 50;

            _pm.Interaction.ColliderArray = new Collider[5];

            _pm.Interaction.InteractionPopup.transform.SetParent(_tempLocation.transform);
            _pm.Interaction.InteractionPopup.transform.position = _tempLocation.position;
        }

        private void OnEnable()
        {
            _pm.Interaction.Input_InteractValue.action.performed += Interact;
            _pm.Interaction.Input_PickupValue.action.performed += PickUp;
            _pm.Interaction.Input_PickupValue.action.canceled +=  Drop;
        }

        private void OnDisable()
        {
            _pm.Interaction.Input_InteractValue.action.performed -= Interact;
            _pm.Interaction.Input_PickupValue.action.performed -= PickUp;
            _pm.Interaction.Input_PickupValue.action.canceled -= Drop;
        }

        private void Update() 
        {
            if (!_pm.Interaction.ObjectRigidbody)
            {
                CheckInteractibles();
            } 
        }

        private void FixedUpdate()
        {
            _pm.Interaction.PickupPointDistance = Vector3.Distance(transform.position, _pm.Interaction.PickupPoint.transform.position);

            PickupPointDistanceCorrection();

            if (_pm.Interaction.ObjectRigidbody)
            {
                if (Vector3.Distance(_pm.Interaction.ObjectRigidbody.transform.position, _pm.Interaction.PickupPoint.transform.position) > _pm.Interaction.MaxDistanceFromPoint)
                {
                    _pm.Interaction.ObjectCollider.excludeLayers = _pm.Interaction.ExcludeCollisionMask;
                    ResetRigidbodyParameters();
                    _pm.Interaction.ObjectCollider = null;
                }

                // Object movement
                _pm.Interaction.ObjectRigidbody.angularVelocity = Vector3.zero;
                Vector3 DirectionToPoint = _pm.Interaction.PickupPoint.transform.position - _pm.Interaction.ObjectRigidbody.transform.position;
                _pm.Interaction.ObjectRigidbody.AddForce(DirectionToPoint * _pm.Interaction.PickupPointDistance * 500f, ForceMode.Acceleration);

                _pm.Interaction.ObjectRigidbody.velocity = Vector3.zero;
            }
        }

        public void Interact(InputAction.CallbackContext _value)
        {
            try
            {
                if (_pm.Interaction.SelectedCollider.gameObject.TryGetComponent(out IInteraction interactObj))
                {
                    interactObj.Interact();
                }
            }
            catch(NullReferenceException)
            {
                Debug.Log("Interaction script not found");
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

            Physics.Raycast(r, out hit, _pm.Interaction.InteractRange);

            _pm.Interaction.HitPosition = hit.collider != null ? hit.point : transform.position + transform.forward * _pm.Interaction.InteractRange;

            int results = Physics.OverlapSphereNonAlloc(_pm.Interaction.HitPosition, _pm.Interaction.SphereCheckRange, _pm.Interaction.ColliderArray, _pm.Interaction.InteractiblesMask);

            List<Collider> interactibleColliders = new List<Collider>();

            Collider nearestInteractibleCollider = null;

            // Checks if all colliders are interactible and adding them to a separate list if they are
            if (results > 0)
            {
                for (int i = 0; i < results; i++)
                {
                    if (_pm.Interaction.ColliderArray[i].TryGetComponent(out InteractionController tempIntController))
                    {
                        if (tempIntController.IsInteractible()) interactibleColliders.Add(_pm.Interaction.ColliderArray[i]);
                    }
                }

                // finds the nearest collider
                if (interactibleColliders.Count > 0)
                {
                    float minDistance = -1;
                    foreach (Collider collider in interactibleColliders)
                    {
                        if (minDistance == -1)
                        {
                            minDistance = Vector3.Distance(_pm.Interaction.HitPosition, collider.transform.position);
                            nearestInteractibleCollider = collider;
                            continue;
                        }
                        else
                        {
                            float distance = Vector3.Distance(_pm.Interaction.HitPosition, collider.transform.position);

                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestInteractibleCollider = collider;
                                continue;
                            }
                        }
                    }


                    if (_pm.Interaction.SelectedCollider != nearestInteractibleCollider)
                    {
                        _pm.Interaction.InteractionPopup.transform.SetParent(_tempLocation.transform);
                        _pm.Interaction.InteractionPopup.transform.position = _tempLocation.position;
                    }

                    _pm.Interaction.InteractionPopup.transform.SetParent(nearestInteractibleCollider.transform);

                    _pm.Interaction.InteractionPopup.transform.position = Vector3.zero;


                    if (_pm.Interaction.InteractionController)
                    {
                        _pm.Interaction.InteractionPopup.transform.LookAt(transform, transform.up);

                        float proximityTextOpacity = Mathf.InverseLerp(_pm.Interaction.SphereCheckRange, 0, Vector3.Distance(_pm.Interaction.HitPosition, _pm.Interaction.InteractionPopup.transform.position));
                        _pm.Interaction.InteractionController.SetTextOpacity(proximityTextOpacity);

                        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
                        Vector2 popupPosition = new Vector2(_pm.Interaction.InteractionPopup.transform.position.x, _pm.Interaction.InteractionPopup.transform.position.z);
                        float proximityTextSize = Mathf.InverseLerp(_pm.Interaction.InteractRange + _pm.Interaction.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _pm.Interaction.MaxTextSize;
                        if (proximityTextSize < _pm.Interaction.MinTextSize) proximityTextSize = _pm.Interaction.MinTextSize;
                        _pm.Interaction.InteractionController.SetTextSize(proximityTextSize);
                    }
                }
                /*
                if (results != 0)
                {
                    for(int i = 0; i < results; i++)
                    {
                        if (_pm.Interaction.ColliderArray[i].TryGetComponent(out InteractionController tempIntController))
                        {
                            if (tempIntController.IsInteractible()) interactibleColliders.Add(_pm.Interaction.ColliderArray[i]);
                        }
                    }

                    if (interactibleColliders.Count != 0)
                    {
                        float minDistance = -1;
                        foreach(Collider collider in interactibleColliders)
                        {
                            if (minDistance == -1)
                            {
                                minDistance = Vector3.Distance(_pm.Interaction.HitPosition, collider.transform.position);
                                nearestInteractibleCollider = collider;
                                continue;
                            }
                            else
                            {
                                float distance = Vector3.Distance(_pm.Interaction.HitPosition, collider.transform.position);

                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    nearestInteractibleCollider = collider;
                                    continue;
                                }
                            }

                        }

                        if (_pm.Interaction.SelectedCollider != nearestInteractibleCollider)
                        {
                            InteractionController tempIntController;

                            if (_pm.Interaction.SelectedCollider)
                            {
                                _pm.Interaction.SelectedCollider.gameObject.TryGetComponent(out tempIntController);

                                if (tempIntController.HasInteractionPopup())
                                {
                                    IInteractionPopup interactionPopupInterface = tempIntController.GetComponent<IInteractionPopup>();
                                    _pm.Interaction.InteractionController = null;
                                    interactionPopupInterface.DestroyPopup();
                                }
                            }

                            nearestInteractibleCollider.gameObject.TryGetComponent(out tempIntController);

                            if (!tempIntController.HasInteractionPopup())
                            {
                                IInteractionPopup interactionPopupInterface = tempIntController.GetComponent<IInteractionPopup>();
                                _pm.Interaction.InteractionController = tempIntController;
                                interactionPopupInterface.SpawnPopup(_pm.Interaction.InteractionPopup);
                            }

                            _pm.Interaction.SelectedCollider = nearestInteractibleCollider;
                        }

                        if (_pm.Interaction.InteractionController)
                        {
                            _pm.Interaction.ActivePopup.transform.LookAt(transform, transform.up);

                            float proximityTextOpacity = Mathf.InverseLerp(_pm.Interaction.SphereCheckRange, 0, Vector3.Distance(_pm.Interaction.HitPosition, _pm.Interaction.ActivePopup.transform.position));
                            _pm.Interaction.InteractionController.SetTextOpacity(proximityTextOpacity);

                            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
                            Vector2 popupPosition = new Vector2(_pm.Interaction.ActivePopup.transform.position.x, _pm.Interaction.ActivePopup.transform.position.z);
                            float proximityTextSize = Mathf.InverseLerp(_pm.Interaction.InteractRange + _pm.Interaction.SphereCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * _pm.Interaction.MaxTextSize;
                            if (proximityTextSize < _pm.Interaction.MinTextSize) proximityTextSize = _pm.Interaction.MinTextSize;
                            _pm.Interaction.InteractionController.SetTextSize(proximityTextSize);
                        }

                    }

                }
                else
                {
                    if (_pm.Interaction.SelectedCollider) _pm.Interaction.SelectedCollider = null;
                }
                */
            }
        }

        public void PickUp(InputAction.CallbackContext _value)
        {
            Ray r = new Ray(transform.position, transform.forward);

            if (Physics.SphereCast(transform.position, .25f, transform.forward, out RaycastHit hitInfo, _pm.Interaction.MaxPickupDistance, _pm.Interaction.PickupLayer))
            {
                float objectWeight = -Physics.gravity.y * hitInfo.rigidbody.mass;

                if (objectWeight <= _pm.Interaction.MaxObjectWeight)
                {
                    _pm.Interaction.ObjectRigidbody = hitInfo.rigidbody;

                    _pm.Interaction.ObjectCollider = _pm.Interaction.ObjectRigidbody.GetComponent<Collider>();

                    _pm.Interaction.ExcludeCollisionMask = _pm.Interaction.ObjectCollider.excludeLayers;

                    _pm.Interaction.ObjectCollider.excludeLayers = _pm.Interaction.PlayerLayerMask;

                    _pm.Interaction.ObjectMass = _pm.Interaction.ObjectRigidbody.mass;
                    _pm.Interaction.ObjectDrag = _pm.Interaction.ObjectRigidbody.drag;
                    _pm.Interaction.ObjectAngularDrag = _pm.Interaction.ObjectRigidbody.angularDrag;

                    _pm.Interaction.ObjectRigidbody.useGravity = false;
                    _pm.Interaction.ObjectRigidbody.angularDrag = 200f;
                    float distanceToObject = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_pm.Interaction.ObjectRigidbody.transform.position.x, _pm.Interaction.ObjectRigidbody.transform.position.z));
                    _pm.Interaction.PickupPoint.transform.position = transform.position + transform.forward * distanceToObject;
                }
                else
                {
                    Debug.Log("Object too heavy!");
                }
            }
        }

        public void Drop(InputAction.CallbackContext _context)
        {
            if (_pm.Interaction.ObjectRigidbody)
            {

                _pm.Interaction.ObjectCollider.excludeLayers = _pm.Interaction.ExcludeCollisionMask;

                ResetRigidbodyParameters();
                _pm.Interaction.ObjectCollider = null;
            }
        }

        public void Throw(InputAction.CallbackContext _context)
        {
            if (_pm.Interaction.ObjectRigidbody)
            {
                if (_context.phase == InputActionPhase.Performed)
                {
                    _pm.Interaction.ObjectRigidbody.AddForce(transform.position + transform.forward * (_pm.Interaction.ThrowStrength * 100), ForceMode.Acceleration);


                    _pm.Interaction.ObjectCollider.excludeLayers = _pm.Interaction.ExcludeCollisionMask;

                    ResetRigidbodyParameters();
                }
            }
        }

        private void PickupPointDistanceCorrection()
        {
            if (_pm.Interaction.PickupPointDistance > _pm.Interaction.MaxPickupPointDistance)
            {
                _pm.Interaction.PickupPoint.transform.position = transform.position + transform.forward * _pm.Interaction.MaxPickupPointDistance;
            }
            else if (_pm.Interaction.PickupPointDistance < _pm.Interaction.MinPickupPointDistance)
            {
                _pm.Interaction.PickupPoint.transform.position = transform.position + transform.forward * _pm.Interaction.MinPickupPointDistance;
            }
        }

        public void ResetRigidbodyParameters()
        {
            _pm.Interaction.ObjectRigidbody.excludeLayers = _pm.Interaction.ExcludeCollisionMask;

            _pm.Interaction.ObjectRigidbody.useGravity = true;
            _pm.Interaction.ObjectRigidbody.angularDrag = _pm.Interaction.ObjectAngularDrag;
            _pm.Interaction.ObjectRigidbody.drag = _pm.Interaction.ObjectDrag;
            _pm.Interaction.ObjectRigidbody.mass = _pm.Interaction.ObjectMass;

            _pm.Interaction.ObjectRigidbody.velocity /= _pm.Interaction.DropVelocityReduction;
            _pm.Interaction.ObjectRigidbody = null;

            _pm.Interaction.ObjectAngularDrag = 0;
            _pm.Interaction.ObjectDrag = 0;
            _pm.Interaction.ObjectMass = 0;
            return;
        }

        private void OnDrawGizmos() 
        {
        }
    }
}