using UnityEngine;
using UnityEngine.InputSystem;
using MyCode.Managers;
using MyCode.GameData.Interaction;
using TMPro;
using System.Linq;
using UnityEditor.PackageManager;

namespace MyCode.PlayerComponents
{
    public class P_Interactor : MonoBehaviour
    {
        private Camera cam;

        private bool canInteract = true;
        private bool canCheckInteractibles = true;

        [SerializeField] private GameObject _pickupPoint;

        [SerializeField] private RectTransform _popupTransform;
        [SerializeField] private TextMeshProUGUI _popupText;

        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _closestCollider;
        [SerializeField] private InteractionController _activeController;

        [SerializeField] private Collider[] _colliderArray;

        private Vector3 _hitPosition;
        private Vector3 _colliderHitPosition;

        public float _colliderHitDistance;

        private float _mass;
        private float _drag;
        private float _angularDrag;

        [SerializeField] private InputActionReference _input_InteractValue;
        [SerializeField] private InputActionReference _input_ThrowValue;
        [SerializeField] private InputActionReference _input_ZoomValue;

        void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            PlayerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _pickupPoint.transform.position);
            PickupPointDistanceCorrection();

            PlayerManager.InteractionData.ZoomInterval = (PlayerManager.InteractionData.MaxPickupPointDistance - PlayerManager.InteractionData.MinPickupPointDistance) / PlayerManager.InteractionData.IntervalCount;

            _colliderArray = new Collider[PlayerManager.InteractionData.ColliderAraySize];
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

            PlayerManager.InventoryData.OnInventoryStatusChange += value => canInteract = !value;
            PlayerManager.InteractionData.OnPickUpObject += () => canCheckInteractibles = false;
            PlayerManager.InteractionData.OnDropObject += () => canCheckInteractibles = true;
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

            PlayerManager.InventoryData.OnInventoryStatusChange -= value => canInteract = !value;
            PlayerManager.InteractionData.OnPickUpObject -= () => canCheckInteractibles = false;
            PlayerManager.InteractionData.OnDropObject -= () => canCheckInteractibles = true;
        }

        private void Update() 
        {
            CheckInteractibleColliders();
        }

        private void FixedUpdate()
        {
            PlayerManager.InteractionData.PickupPointDistance = Vector3.Distance(transform.position, _pickupPoint.transform.position);

            PickupPointDistanceCorrection();

            if (_rb)
            {
                if (Vector3.Distance(_rb.transform.position, _pickupPoint.transform.position) > PlayerManager.InteractionData.MaxDistanceFromPoint)
                {
                    _closestCollider.excludeLayers = PlayerManager.InteractionData.ExcludeCollisionMask;
                    ResetRigidbodyParameters();
                    _closestCollider = null;
                    PlayerManager.InteractionData.InvokeDropObject();
                    return;
                }

                // Object movement
                _rb.angularVelocity = Vector3.zero;
                Vector3 DirectionToPoint = _pickupPoint.transform.position - _rb.transform.position;
                _rb.AddForce(DirectionToPoint * PlayerManager.InteractionData.PickupPointDistance * 500f, ForceMode.Acceleration);

                _rb.velocity = Vector3.zero;
            }
        }

        public void Interact(InputAction.CallbackContext _value)
        {
            if (!canInteract)
                return;

            if (_closestCollider == null)
                return;

            if (_colliderHitDistance > PlayerManager.InteractionData.MaxInteractDistance)
                return;

            if (_activeController.InteractionType == InteractionType.Interact)
            {
                _activeController.Interact();
                return;
            }
            else if (_activeController.InteractionType == InteractionType.PickUp)
            {
                PickUp();
                return;
            }
        }

        private void CheckInteractibleColliders()
        {
            if (!canCheckInteractibles) return;

            Collider closestCollider = GetClosestCollider(GetInteractibleColliders());
            if (closestCollider == null)
            {
                _closestCollider = null;
                _activeController = null;
                _popupText.enabled = false;
                _hitPosition = Vector3.zero;
                return;
            }


            if (_closestCollider != closestCollider)
            {
                _closestCollider = closestCollider;
                _activeController = _closestCollider.GetComponent<InteractionController>();
                _popupText.enabled = true;
                _popupText.text = _activeController.PopupText;
            }

            Vector3 pointOnScreen = _activeController.CustomPopupLocation ? cam.WorldToScreenPoint(_activeController.PopupLocation) : cam.WorldToScreenPoint(_closestCollider.transform.position);
            _popupTransform.position = pointOnScreen;
            float proximityTextOpacity = Mathf.InverseLerp(PlayerManager.InteractionData.SphereCheckRange, 0, _colliderHitDistance);
            _popupText.alpha = proximityTextOpacity;
        }

        private Collider[] GetInteractibleColliders()
        {
            if (!canInteract) return null;
            if (!canCheckInteractibles) return null;

            Ray r = new Ray(transform.position, transform.forward);

            // Cast ray in front of the camera with InteractRange as its max distance
            Physics.Raycast(r, out RaycastHit hitInfo, PlayerManager.InteractionData.InteractRange);

            if (hitInfo.collider != null)
            {
                if (!hitInfo.collider.TryGetComponent(out InteractionController controller)) goto Continue;
                if (!controller.Interactible) goto Continue;

                _colliderHitPosition = _hitPosition;
                _colliderHitDistance = 0;

                return new Collider[] { hitInfo.collider };
            }

            Continue:

            _hitPosition = hitInfo.collider != null ? hitInfo.point : transform.position + transform.forward * PlayerManager.InteractionData.InteractRange;

            // Number of new detected colliders from OverlapSphereNonAlloc
            int results = Physics.OverlapSphereNonAlloc(_hitPosition, PlayerManager.InteractionData.SphereCheckRange, _colliderArray, PlayerManager.InteractionData.InteractiblesMask);
            if (results == 0) return null;

            Collider[] colliders = new Collider[results];

            // Checks if all colliders are interactible and adding them to a separate list if they are
            int arrayIndex = 0;
            for (int i = 0; i < results; i++)
            {
                if (_colliderArray[i].TryGetComponent(out InteractionController tempIntController))
                {
                    if (!tempIntController.Interactible) continue;

                    colliders[arrayIndex] = _colliderArray[i];
                    arrayIndex++;
                }
            }

            return colliders;
        }

        private Collider GetClosestCollider(Collider[] _colliders)
        {
            if (_colliders == null || _colliders.Count() == 0) return null;
            if (_colliders.Count() == 1)
            {
                Ray ray = new Ray(_hitPosition, _colliders[0].transform.position - _hitPosition);
                _colliders[0].Raycast(ray, out RaycastHit hitInfo, PlayerManager.InteractionData.SphereCheckRange);
                _colliderHitPosition = hitInfo.distance < .025f ? _hitPosition : hitInfo.point;
                _colliderHitDistance = hitInfo.distance < .025f ? 0 : hitInfo.distance;
                return _colliders[0];
            }

            float lowestDistance = -1;
            Collider nearestCollider = null;
            for (int i = 0; i < _colliders.Count(); i++)
            {
                Ray ray = new Ray(_hitPosition, _colliders[i].transform.position - _hitPosition);
                _colliders[i].Raycast(ray, out RaycastHit perColliderHitInfo, PlayerManager.InteractionData.SphereCheckRange);

                if (lowestDistance == -1 || perColliderHitInfo.distance < lowestDistance)
                {
                    lowestDistance = perColliderHitInfo.distance;
                    nearestCollider = _colliders[i];
                    _colliderHitPosition = perColliderHitInfo.distance < .025f ? _hitPosition : perColliderHitInfo.point;
                    _colliderHitDistance = perColliderHitInfo.distance < .025f ? 0 : perColliderHitInfo.distance;
                    continue;
                }
            }

            return nearestCollider;
        }


        private void PickUp()
        {
            if (!canInteract) return;

            if (!_closestCollider) return;

            if (_activeController.InteractionType != InteractionType.PickUp) return;

            _rb = _closestCollider.GetComponent<Rigidbody>();
            float objectWeight =_rb.mass;

            if (objectWeight >= PlayerManager.InteractionData.MaxObjectWeight)
            {
                Debug.Log("Object too heavy!");
                _rb = null;
                return;
            }

            PlayerManager.InteractionData.ExcludeCollisionMask = _closestCollider.excludeLayers;

            _closestCollider.excludeLayers = PlayerManager.InteractionData.PlayerLayerMask;

            _mass = _rb.mass;
            _drag = _rb.drag;
            _angularDrag = _rb.angularDrag;

            _rb.useGravity = false;
            _rb.angularDrag = 200f;
            float distanceToObject = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_rb.transform.position.x, _rb.transform.position.z));
            _pickupPoint.transform.position = transform.position + transform.forward * distanceToObject;

            PlayerManager.InteractionData.InvokePickUpObject();

            _popupText.enabled = false;
            canCheckInteractibles = false;
        }

        private void Drop(InputAction.CallbackContext _context)
        {
            if (!_rb)
            {
                return;
            }

            _closestCollider.excludeLayers = PlayerManager.InteractionData.ExcludeCollisionMask;

            ResetRigidbodyParameters();
            _closestCollider = null;
            PlayerManager.InteractionData.InvokeDropObject();

            _popupText.enabled = true;
            canCheckInteractibles = true;
        }

        private void Throw(InputAction.CallbackContext _context)
        {
            if (!_rb)
            {
                return;
            }

            if (_context.phase == InputActionPhase.Performed)
            {
                _rb.AddForce(transform.position + transform.forward * (PlayerManager.InteractionData.ThrowStrength * 100), ForceMode.Acceleration);


                _closestCollider.excludeLayers = PlayerManager.InteractionData.ExcludeCollisionMask;

                ResetRigidbodyParameters();

                PlayerManager.InteractionData.InvokeDropObject();

                _popupText.enabled = true;
                canCheckInteractibles = true;
            }
        }

        private void Zoom(InputAction.CallbackContext _context)
        {
            float zoomValue = _context.ReadValue<float>() / 120;

            if (zoomValue == 0) return;

            _pickupPoint.transform.position = transform.position + transform.forward * (PlayerManager.InteractionData.PickupPointDistance + PlayerManager.InteractionData.ZoomInterval * zoomValue);
            PickupPointDistanceCorrection();
        }

        private void PickupPointDistanceCorrection()
        {
            if (PlayerManager.InteractionData.PickupPointDistance > PlayerManager.InteractionData.MaxPickupPointDistance)
            {
                _pickupPoint.transform.position = transform.position + transform.forward * PlayerManager.InteractionData.MaxPickupPointDistance;
                PlayerManager.InteractionData.PickupPointDistance = PlayerManager.InteractionData.MaxPickupPointDistance;
            }
            else if (PlayerManager.InteractionData.PickupPointDistance < PlayerManager.InteractionData.MinPickupPointDistance)
            {
                _pickupPoint.transform.position = transform.position + transform.forward * PlayerManager.InteractionData.MinPickupPointDistance;
                PlayerManager.InteractionData.PickupPointDistance = PlayerManager.InteractionData.MinPickupPointDistance;
            }
        }

        public void ResetRigidbodyParameters()
        {
            _rb.excludeLayers = PlayerManager.InteractionData.ExcludeCollisionMask;

            _rb.useGravity = true;
            _rb.angularDrag = _angularDrag;
            _rb.drag = _drag;
            _rb.mass = _mass;

            _rb.velocity /= PlayerManager.InteractionData.DropVelocityReduction;
            _rb = null;

            _angularDrag = 0;
            _drag = 0;
            _mass = 0;
            return;
        }

        private void OnDrawGizmos()
        {
            if (_closestCollider != null)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(transform.position + (transform.forward * PlayerManager.InteractionData.InteractRange), PlayerManager.InteractionData.SphereCheckRange);

            if (_closestCollider != null)
            {
                if (_colliderHitPosition != _hitPosition)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(_hitPosition, _colliderHitPosition);
                }
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_colliderHitPosition, _closestCollider.transform.position);
            }  
        }
    }
}