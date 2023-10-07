using MyBox;
using MyCode.GameData;
using MyCode.Managers;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCode.Characters
{
    public enum MovementDirection
    {
        none,
        forward,
        forward_right,
        forward_left,
        right,
        left,
        backward,
        backward_right,
        backward_left
    }

    public enum MovementState
    {
        none,
        walk,
        sprint,
        sneak
    }

    public class PlayerLogic : MonoBehaviour
    {


        /**********************/
        /* Movement variables */
        /**********************/


        [Space]
        [Separator("Movement", true)]
        [Space]

        private CharacterController _characterController;

        private Vector2 _moveValue;
        private float _sprintValue;
        private float _sneakValue;

        private float _internalSpeedMultiplier;
        private float _joystickWeight;

        [SerializeField] private Vector3 _directionToMove;
        private Vector2 _smoothMoveValue;

        private Vector2 _currentVelocity;

        private MovementDirection _movementDirection;
        private MovementState _movementState;

        // Gravity
        private float _currentGravityVelocity;
        private float _gravityForce;

        [SerializeField] private LayerMask _groundLayer;

        [Header("Input Action")]
        [Space]
        [SerializeField] private InputActionReference _inputWalk;
        [SerializeField] private InputActionReference _inputSprint;
        [SerializeField] private InputActionReference _inputSneak;

        private bool canMove = true;


        /********************/
        /* Camera variables */
        /********************/


        [Space]
        [Separator("Camera")]
        [Space]

        private Camera _camera;

        [Header("Stabilization Components")]
        [Space]
        [SerializeField] private GameObject camStabilizationObject;
        [SerializeField] private GameObject headJoint;
        [SerializeField] private GameObject eyeJoint;

        private bool canLook = true;

        private float mouseRotation = 0f;
        private float valueX;
        private float valueY;

        private Vector3 stabilizationVelocity;

        [SerializeField] private InputActionReference _inputCamera;


        /*************************/
        /* Interaction variables */
        /*************************/


        [Space]
        [Separator("Interaction")]
        [Space]

        private bool canInteract = true;
        private bool canCheckInteractibles = true;

        [SerializeField] private GameObject _pickupPoint;

        [SerializeField] private InteractibleIndicator _interactibleIndicator;

        private Rigidbody _rb;
        private Collider _closestCollider;
        private InteractionController _activeController;

        [SerializeField] private Collider[] _colliderArray;

        private bool _lookingAtCollider;

        private Vector3 _hitPosition;
        private Vector3 _colliderHitPosition;

        public float _colliderHitDistance;

        private float _mass;
        private float _drag;
        private float _angularDrag;

        [SerializeField] private InputActionReference _inputInteract;
        [SerializeField] private InputActionReference _inputThrow;
        [SerializeField] private InputActionReference _inputZoom;




        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;

            Cursor.lockState = CursorLockMode.Locked;
            
        }

        private void OnEnable()
        {
            _inputWalk.action.Enable();
            _inputSprint.action.Enable();
            _inputSneak.action.Enable();
            _inputCamera.action.Enable();
            _inputInteract.action.Enable();
            _inputThrow.action.Enable();
            _inputZoom.action.Enable();

            _inputWalk.action.performed += OnWalkAction;
            _inputSprint.action.performed += OnSprintAction;
            _inputSneak.action.performed += OnSneakAction;
            _inputWalk.action.canceled += StopWalkAction;
            _inputSprint.action.canceled += StopSprintAction;
            _inputSneak.action.canceled += StopSneakAction;
            _inputInteract.action.performed += Interact;
            _inputInteract.action.canceled += DropObject;
            _inputThrow.action.performed += Throw;
            _inputZoom.action.performed += _ctx => Zoom(_ctx);

            _inputCamera.action.performed += ctx => Look(ctx.ReadValue<Vector2>());
            PlayerManager.Instance.InventoryData.OnInventoryStateChange += value => canLook = !value;

            PlayerManager.Instance.OnPlayerTeleport += (position) =>
            {
                transform.position = position;
            };

            PlayerManager.Instance.InventoryData.OnInventoryStateChange += value => canMove = !value;
        }

        private void OnDisable()
        {
            _inputWalk.action.Disable();
            _inputSprint.action.Disable();
            _inputSneak.action.Disable();
            _inputCamera.action.Disable();
            _inputInteract.action.Disable();
            _inputThrow.action.Disable();
            _inputZoom.action.Disable();

            _inputWalk.action.performed -= OnWalkAction;
            _inputSprint.action.performed -= OnSprintAction;
            _inputSneak.action.performed -= OnSneakAction;
            _inputWalk.action.canceled -= StopWalkAction;
            _inputSprint.action.canceled -= StopSprintAction;
            _inputSneak.action.canceled -= StopSneakAction;
            _inputInteract.action.performed -= Interact;
            _inputInteract.action.canceled -= DropObject;
            _inputThrow.action.performed -= Throw;
            _inputZoom.action.performed -= _ctx => Zoom(_ctx);

            _inputCamera.action.performed -= ctx => Look(ctx.ReadValue<Vector2>());
            PlayerManager.Instance.InventoryData.OnInventoryStateChange -= value => canLook = !value;

            PlayerManager.Instance.InventoryData.OnInventoryStateChange -= value => canMove = !value;
        }

        void Start()
        {
            // Movement logic
            _internalSpeedMultiplier = 1;
            _currentVelocity = Vector2.zero;

            _movementState = MovementState.none;
            _smoothMoveValue = Vector3.zero;

            PlayerManager.Instance.MovementData.IsMoving = false;
            PlayerManager.Instance.MovementData.IsMovingForward = false;
            _smoothMoveValue = Vector2.zero;

            PlayerManager.Instance.MovementData.IsGrounded = false;

            // Interaction logic
            PlayerManager.Instance.InteractionData.PickupPointDistance = Vector3.Distance(_camera.transform.position, _pickupPoint.transform.position);
            PickupPointDistanceCorrection();

            PlayerManager.Instance.InteractionData.ZoomInterval = (PlayerManager.Instance.InteractionData.MaxPickupPointDistance - PlayerManager.Instance.InteractionData.MinPickupPointDistance) / PlayerManager.Instance.InteractionData.IntervalCount;

            _colliderArray = new Collider[PlayerManager.Instance.InteractionData.ColliderAraySize];
        }

        private void Update()
        {
            MovePlayer();
            CheckInteractibleColliders();
        }

        private void FixedUpdate()
        {
            PlayerManager.Instance.InteractionData.PickupPointDistance = Vector3.Distance(_camera.transform.position, _pickupPoint.transform.position);

            PickupPointDistanceCorrection();

            if (!_rb) return;

            if (Vector3.Distance(_rb.transform.position, _pickupPoint.transform.position) > PlayerManager.Instance.InteractionData.MaxDistanceFromPoint)
            {
                DropObject();
                return;
            }

            // Object movement
            _rb.angularVelocity = Vector3.zero;
            Vector3 DirectionToPoint = _pickupPoint.transform.position - _rb.transform.position;
            _rb.AddForce(DirectionToPoint * PlayerManager.Instance.InteractionData.PickupPointDistance * 500f, ForceMode.Acceleration);

            _rb.velocity = Vector3.zero;
        }



        /**************************************************************/
        /*                      Movement Logic                        */
        /**************************************************************/

        private void MovePlayer()
        {
            if (!canMove)
                return;

            PlayerManager.Instance.MovementData.IsMoving = IsPlayerMoving();
            PlayerManager.Instance.MovementData.IsMovingForward = IsPlayerMovingForward();
            PlayerManager.Instance.MovementData.IsGrounded = IsGrounded();

            UpdateMovementDirection();

            _joystickWeight = GetJoystickWeight();
            _directionToMove = CalculateMovementDirection();

            ApplyGravity();
            CheckForMovementUpdate();

            _characterController.Move(_directionToMove * PlayerManager.Instance.MovementData.WalkSpeed * _internalSpeedMultiplier * Time.deltaTime);
        }

        private float GetJoystickWeight()
        {
            float weight = Mathf.Max(Mathf.Abs(_moveValue.x), Mathf.Abs(_moveValue.y));
            weight = weight < .4f ? .4f : weight;
            return weight;
        }

        private Vector3 CalculateMovementDirection()
        {
            // increases/decreases initial speed over time for smooth movement
            _smoothMoveValue = Vector2.SmoothDamp(_smoothMoveValue, _moveValue, ref _currentVelocity, PlayerManager.Instance.MovementData.SmoothTime);
            if (!IsPlayerMoving())
            {
                _smoothMoveValue.x = _smoothMoveValue.x < .0001 ? 0 : _smoothMoveValue.x;
                _smoothMoveValue.y = _smoothMoveValue.y < .0001 ? 0 : _smoothMoveValue.y;
            }
            

            Vector3 directionToMove = (_smoothMoveValue.x * transform.right + _smoothMoveValue.y * transform.forward) * _joystickWeight;

            // clamps player movement to magnitude of 1
            directionToMove = Vector3.ClampMagnitude(directionToMove, 1);

            return directionToMove;
        }

        private void OnWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = value.ReadValue<Vector2>();
            if (_sneakValue != 0)
            {
                _movementState = MovementState.sneak;
            }
            else
            {
                _movementState = MovementState.walk;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext value)
        {
            if (!IsPlayerMovingForward()) return;

            _sprintValue = value.ReadValue<float>();
            _internalSpeedMultiplier = PlayerManager.Instance.MovementData.SprintMultiplier;
            _movementState = MovementState.sprint;
        }

        private void OnSneakAction(InputAction.CallbackContext value)
        {
            if (_movementState == MovementState.sprint) return;

            _sneakValue = value.ReadValue<float>();
            _internalSpeedMultiplier = PlayerManager.Instance.MovementData.SneakMultiplier;
            _movementState = MovementState.sneak;
        }

        private void StopWalkAction(InputAction.CallbackContext value)
        {
            _moveValue = Vector2.zero;
            _sprintValue = 0;
            _sneakValue = 0;
            _movementState = MovementState.none;
        }

        private void StopSprintAction(InputAction.CallbackContext value)
        {
            _sprintValue = 0;

            if (_sneakValue != 0)
            {
                _internalSpeedMultiplier = PlayerManager.Instance.MovementData.SneakMultiplier;
                _movementState = MovementState.sneak;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _movementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _movementState = MovementState.none;
            return;
        }

        private void StopSneakAction(InputAction.CallbackContext value)
        {
            _sneakValue = 0;
            if (_sprintValue != 0)
            {
                _internalSpeedMultiplier = PlayerManager.Instance.MovementData.SprintMultiplier;
                _movementState = MovementState.sprint;
                return;
            }
            else if (_moveValue != Vector2.zero)
            {
                _internalSpeedMultiplier = 1;
                _movementState = MovementState.walk;
                return;
            }

            _internalSpeedMultiplier = 1;
            _movementState = MovementState.none;
            return;
        }

        private void ApplyGravity()
        {
            if (IsGrounded())
            {
                _currentGravityVelocity = -1.0f;
            }
            else
            {
                if (PlayerManager.Instance.MovementData.UseCustomGravity)
                    _currentGravityVelocity += (PlayerManager.Instance.MovementData.CustomGravity * PlayerManager.Instance.MovementData.GravityMultiplier) * Time.deltaTime;
                else
                    _currentGravityVelocity += (Physics.gravity.y * PlayerManager.Instance.MovementData.GravityMultiplier) * Time.deltaTime;
            }


            _gravityForce = _currentGravityVelocity;
            _directionToMove.y = _gravityForce;
        }

        public bool IsGrounded()
        {
            Vector3 characterBottom = new Vector3(transform.position.x, transform.position.y - _characterController.height / 2, transform.position.z);
            if (Physics.Raycast(characterBottom, transform.up * -1, .1f, _groundLayer))
                return true;

            return false;
        }

        private bool IsPlayerMoving()
        {
            if (!_moveValue.Equals(Vector2.zero))
            {
                return true;
            }

            return false;
        }

        private bool IsPlayerMovingForward()
        {
            if (IsPlayerMoving())
            {
                if (_moveValue.y > 0 && _moveValue.y >= _moveValue.x)
                    return true;
                else
                    return false;
            }
            return false;
        }

        private void UpdateMovementDirection()
        {
            if (_moveValue == Vector2.zero)
            {
                _movementDirection = MovementDirection.none;
            }
            else if (_moveValue.y > 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _movementDirection = MovementDirection.forward;
            }
            else if (_moveValue.y < 0 && (_moveValue.x < .5 && _moveValue.x > -.5))
            {
                _movementDirection = MovementDirection.backward;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x > .5))
            {
                _movementDirection = MovementDirection.forward_right;
            }
            else if ((_moveValue.y > .5) && (_moveValue.x < -.5))
            {
                _movementDirection = MovementDirection.forward_left;
            }
            else if (_moveValue.x > 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _movementDirection = MovementDirection.right;
            }
            else if (_moveValue.x < 0 && (_moveValue.y < .5 && _moveValue.y > -.5))
            {
                _movementDirection = MovementDirection.left;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x > .5))
            {
                _movementDirection = MovementDirection.backward_right;
            }
            else if ((_moveValue.y < -.5) && (_moveValue.x < -.5))
            {
                _movementDirection = MovementDirection.backward_left;
            }
        }

        private void CheckForMovementUpdate()
        {
            if (_sprintValue == 0) return;

            if (_sneakValue != 0)
            {
                _internalSpeedMultiplier = PlayerManager.Instance.MovementData.SneakMultiplier;
                _movementState = MovementState.sneak;
                return;
            }

            if (!IsPlayerMovingForward())
            {
                _internalSpeedMultiplier = 1;
                _movementState = MovementState.walk;
            }
        }



        /**************************************************************/
        /*                        Camera Logic                        */
        /**************************************************************/



        private void Look(Vector2 _valueXY)
        {
            if (!canLook)
                return;

            valueX = _valueXY.x * PlayerManager.Instance.CameraData.Sensetivity * Time.deltaTime;
            valueY = _valueXY.y * PlayerManager.Instance.CameraData.Sensetivity * Time.deltaTime;

            mouseRotation -= valueY;
            mouseRotation = Mathf.Clamp(mouseRotation, PlayerManager.Instance.CameraData.BottomRotationLimit, PlayerManager.Instance.CameraData.TopRotationLimit);
            _camera.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
            transform.Rotate(Vector3.up * valueX);
            if (PlayerManager.Instance.CameraData.UseStabilization)
            {
                camStabilizationObject.transform.localRotation = Quaternion.Euler(mouseRotation, 0, 0);
                _camera.transform.LookAt(FocusTarget());
                transform.position = FollowHeadJoint(headJoint, eyeJoint, .2f, PlayerManager.Instance.CameraData.StabilizationAmount);
            }
        }

        private Vector3 FocusTarget()
        {
            PlayerManager.Instance.CameraData.FocusPointStabilizationDistance = PlayerManager.Instance.CameraData.FocusPointStabilizationDistance <= 0 ? 1f : PlayerManager.Instance.CameraData.FocusPointStabilizationDistance;
            Vector3 pos = camStabilizationObject.transform.position + camStabilizationObject.transform.forward * PlayerManager.Instance.CameraData.FocusPointStabilizationDistance;
            return pos;
        }

        private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset)
        {
            return new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset;
        }

        private Vector3 FollowHeadJoint(GameObject _headJoint, GameObject _eyeJoint, float _offset, float _stabilizationAmount)
        {
            return Vector3.SmoothDamp(transform.position, new Vector3(_headJoint.transform.position.x, _eyeJoint.transform.position.y, _headJoint.transform.position.z) + _headJoint.transform.forward * _offset, ref stabilizationVelocity, _stabilizationAmount);
        }



        /*******************************************************************/
        /*                        Interaction Logic                        */
        /*******************************************************************/



        public void Interact(InputAction.CallbackContext _value)
        {
            if (!canInteract)
                return;

            if (_closestCollider == null)
                return;

            if (_colliderHitDistance > PlayerManager.Instance.InteractionData.MaxInteractDistance)
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

            Collider[] interactibleColliders = GetInteractibleColliders();
            Collider closestCollider = _lookingAtCollider ? interactibleColliders[0] : GetClosestCollider(interactibleColliders);

            if (closestCollider == null)
            {
                _closestCollider = null;
                _activeController = null;
                _interactibleIndicator.indicatorText.enabled = false;
                return;
            }


            if (_closestCollider != closestCollider)
            {
                _closestCollider = closestCollider;
                _activeController = _closestCollider.GetComponent<InteractionController>();
                _interactibleIndicator.indicatorText.enabled = true;
                _interactibleIndicator.indicatorText.text = _activeController.PopupText;
            }

            Vector3 pointOnScreen = _activeController.CustomPopupLocation ? _camera.WorldToScreenPoint(_activeController.PopupLocation) : _camera.WorldToScreenPoint(_closestCollider.transform.position);
            _interactibleIndicator.indicatorTransform.position = pointOnScreen;
            float proximityTextOpacity = Mathf.InverseLerp(PlayerManager.Instance.InteractionData.SphereCheckRange, 0, _colliderHitDistance);

            _interactibleIndicator.indicatorText.alpha = proximityTextOpacity;

            _lookingAtCollider = false;
        }

        private Collider[] GetInteractibleColliders()
        {
            if (!canInteract) return null;
            if (!canCheckInteractibles) return null;

            Ray r = new Ray(_camera.transform.position, _camera.transform.forward);

            // Cast ray in front of the camera with InteractRange as its max distance
            Physics.Raycast(r, out RaycastHit hitInfo, PlayerManager.Instance.InteractionData.InteractRange);

            _hitPosition = hitInfo.collider != null ? hitInfo.point : _camera.transform.position + _camera.transform.forward * PlayerManager.Instance.InteractionData.InteractRange;

            while (hitInfo.collider != null)
            {
                if (!hitInfo.collider.TryGetComponent(out InteractionController controller)) break;
                if (!controller.Interactible) break;

                _colliderHitPosition = _hitPosition;
                _lookingAtCollider = true;
                _colliderHitDistance = 0;

                return new Collider[1] { hitInfo.collider };
            }

            

            // Number of new detected colliders from OverlapSphereNonAlloc
            int results = Physics.OverlapSphereNonAlloc(_hitPosition, PlayerManager.Instance.InteractionData.SphereCheckRange, _colliderArray, PlayerManager.Instance.InteractionData.InteractiblesMask);
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
                Physics.Raycast(ray, out RaycastHit hitInfo, PlayerManager.Instance.InteractionData.SphereCheckRange);
                if (hitInfo.point == Vector3.zero) return null;
                _colliderHitPosition = hitInfo.point;
                _colliderHitDistance = Vector3.Distance(_hitPosition, hitInfo.point);

                return _colliders[0];
            }

            float lowestDistance = -1;
            Collider nearestCollider = null;
            for (int i = 0; i < _colliders.Count(); i++)
            {
                Ray ray = new Ray(_hitPosition, _colliders[i].transform.position - _hitPosition);
                _colliders[i].Raycast(ray, out RaycastHit perColliderHitInfo, PlayerManager.Instance.InteractionData.SphereCheckRange);
                if (perColliderHitInfo.point == Vector3.zero) continue;
                if (lowestDistance == -1 || perColliderHitInfo.distance < lowestDistance)
                {
                    lowestDistance = perColliderHitInfo.distance;
                    nearestCollider = _colliders[i];
                    _colliderHitPosition = perColliderHitInfo.point;
                    _colliderHitDistance = Vector3.Distance(_hitPosition, perColliderHitInfo.point);
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
            float objectWeight = _rb.mass;

            if (objectWeight >= PlayerManager.Instance.InteractionData.MaxObjectWeight)
            {
                Debug.Log("Object too heavy!");
                _rb = null;
                return;
            }

            PlayerManager.Instance.InteractionData.ExcludeCollisionMask = _closestCollider.excludeLayers;

            _closestCollider.excludeLayers = PlayerManager.Instance.InteractionData.PlayerLayerMask;

            _mass = _rb.mass;
            _drag = _rb.drag;
            _angularDrag = _rb.angularDrag;

            _rb.useGravity = false;
            _rb.angularDrag = 200f;
            float distanceToObject = Vector2.Distance(new Vector2(_camera.transform.position.x, _camera.transform.position.z), new Vector2(_rb.transform.position.x, _rb.transform.position.z));
            _pickupPoint.transform.position = _camera.transform.position + _camera.transform.forward * distanceToObject;

            _interactibleIndicator.indicatorText.enabled = false;
            canCheckInteractibles = false;
        }

        private void DropObject(InputAction.CallbackContext _context)
        {
            if (!_rb)
            {
                return;
            }

            _closestCollider.excludeLayers = PlayerManager.Instance.InteractionData.ExcludeCollisionMask;

            ResetRigidbodyParameters();
            _closestCollider = null;

            _interactibleIndicator.indicatorText.enabled = true;
            canCheckInteractibles = true;
        }

        private void DropObject()
        {
            if (!_rb)
            {
                return;
            }

            _closestCollider.excludeLayers = PlayerManager.Instance.InteractionData.ExcludeCollisionMask;

            ResetRigidbodyParameters();
            _closestCollider = null;

            _interactibleIndicator.indicatorText.enabled = true;
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
                _rb.AddForce(_camera.transform.position + _camera.transform.forward * (PlayerManager.Instance.InteractionData.ThrowStrength * 100), ForceMode.Acceleration);


                _closestCollider.excludeLayers = PlayerManager.Instance.InteractionData.ExcludeCollisionMask;

                ResetRigidbodyParameters();

                _interactibleIndicator.indicatorText.enabled = true;
                canCheckInteractibles = true;
            }
        }

        private void Zoom(InputAction.CallbackContext _context)
        {
            float zoomValue = _context.ReadValue<float>() / 120;

            if (zoomValue == 0) return;

            _pickupPoint.transform.position = _camera.transform.position + _camera.transform.forward * (PlayerManager.Instance.InteractionData.PickupPointDistance + PlayerManager.Instance.InteractionData.ZoomInterval * zoomValue);
            PickupPointDistanceCorrection();
        }

        private void PickupPointDistanceCorrection()
        {
            if (PlayerManager.Instance.InteractionData.PickupPointDistance > PlayerManager.Instance.InteractionData.MaxPickupPointDistance)
            {
                _pickupPoint.transform.position = _camera.transform.position + _camera.transform.forward * PlayerManager.Instance.InteractionData.MaxPickupPointDistance;
                PlayerManager.Instance.InteractionData.PickupPointDistance = PlayerManager.Instance.InteractionData.MaxPickupPointDistance;
            }
            else if (PlayerManager.Instance.InteractionData.PickupPointDistance < PlayerManager.Instance.InteractionData.MinPickupPointDistance)
            {
                _pickupPoint.transform.position = _camera.transform.position + _camera.transform.forward * PlayerManager.Instance.InteractionData.MinPickupPointDistance;
                PlayerManager.Instance.InteractionData.PickupPointDistance = PlayerManager.Instance.InteractionData.MinPickupPointDistance;
            }
        }

        public void ResetRigidbodyParameters()
        {
            _rb.excludeLayers = PlayerManager.Instance.InteractionData.ExcludeCollisionMask;

            _rb.useGravity = true;
            _rb.angularDrag = _angularDrag;
            _rb.drag = _drag;
            _rb.mass = _mass;

            _rb.velocity /= PlayerManager.Instance.InteractionData.DropVelocityReduction;
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

            Gizmos.DrawWireSphere(_hitPosition, PlayerManager.Instance.InteractionData.SphereCheckRange);

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

