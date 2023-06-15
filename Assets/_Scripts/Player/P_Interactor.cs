using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;

interface IInteractable
{
    public void Interact();
}

interface IHighlight
{
    public void SpawnHighlight(GameObject _target, string _name);
    public void DestroyHighlight();
}

public class P_Interactor : MonoBehaviour
{
    [Separator("Interaction", true)]
    [SerializeField]
    private float interactRange;
    [SerializeField]
    private LayerMask interactiblesMask;

    [Separator("Highlights", true)]
    [SerializeField] private float visibleHighlightRange;
    [SerializeField] private float highlightRangeRadius;
    [Space]
    [Header("Highlights")]
    [Space]
    public GameObject pickItemHighlight;
    public GameObject destroyHighlight;
    public GameObject interactHighlight;
    private Dictionary<string, GameObject> highlights = new Dictionary<string, GameObject>();
    [Space]
    [SerializeField]
    private Collider activeHighlight;
    private Collider nearestItem;
    private Collider[] hitColliders;

    [Separator("Pickup", true)]
    [SerializeField] private GameObject pickupPoint;
    [Space]
    [Header("Distances")]
    [Space]
    [SerializeField] private float maxPickupDistance;
    [SerializeField] private float maxPickupPointDistance;
    [SerializeField] private float minPickupPointDistance;
    [Tooltip("Defines the maximum distance between object and pickup point before dropping the object")]
    [SerializeField] private float maxDistanceFromPoint;
    [SerializeField, ReadOnly] private float pickupPointDistance;
    [Space]
    [SerializeField] private LayerMask pickupLayer;
    [Space]
    [Tooltip("Reduces the velocity be a multiplier. (Useful for adding more weight to objects)")]
    [SerializeField, MinValue(1)] private float dropVelocityReduction;
    [Space]
    [Header("Throw")]
    [Space]
    [SerializeField] private float throwStrength;
    
    private Quaternion objectRotation;
    private Rigidbody pickupObjectRigidbody;
    private float objectMass = 0;
    private float objectDrag = 0;
    private float objectAngularDrag = 0;

    private float zoomInterval;

    // Components //
    private P_Controls p_input;
    private Camera p_camera;

    private bool hittingAir;


    // Input Actions //
    private InputAction ac_interact;  // input action for interacting


    private Vector3 hitPosition;
    private Vector3 pointInAir;


    void Awake()
    {
        p_input = new P_Controls();
        ac_interact = p_input.Player.Interact;
        p_camera = GetComponent<Camera>();

        hitColliders = new Collider[0];

        pickItemHighlight.tag = "Highlight";
        destroyHighlight.tag = "Highlight";
        interactHighlight.tag = "Highlight";

        highlights.Add("pickup", pickItemHighlight);
        highlights.Add("destroy", destroyHighlight);
        highlights.Add("interact", interactHighlight);

        pickupPointDistance = Vector3.Distance(transform.position, pickupPoint.transform.position);

        if (pickupPointDistance > maxPickupPointDistance)
        {
            pickupPoint.transform.position = transform.position + transform.forward * maxPickupPointDistance;
        }
        else if (pickupPointDistance < minPickupPointDistance)
        {
            pickupPoint.transform.position = transform.position + transform.forward * minPickupPointDistance;
        }

        zoomInterval = (maxPickupPointDistance - minPickupPointDistance) / 50;
    }

    void OnEnable()
    {
        p_input.Enable();
    }

    void OnDisable()
    {
        p_input.Disable();
    }

    private void Update() 
    {
        CheckInteractibles();
        pickupPointDistance = Vector3.Distance(transform.position, pickupPoint.transform.position);
    }

    private void FixedUpdate()
    {

        if (pickupPointDistance > maxPickupPointDistance)
        {
            pickupPoint.transform.position = transform.position + transform.forward * maxPickupPointDistance;
        }
        else if (pickupPointDistance < minPickupPointDistance)
        {
            pickupPoint.transform.position = transform.position + transform.forward * minPickupPointDistance;
        }


        if (pickupObjectRigidbody)
        {
            if (Vector3.Distance(pickupObjectRigidbody.transform.position, pickupPoint.transform.position) > maxDistanceFromPoint)
            {
                pickupObjectRigidbody.useGravity = true;
                pickupObjectRigidbody.angularDrag = objectAngularDrag;
                pickupObjectRigidbody.drag = objectDrag;
                pickupObjectRigidbody.mass = objectMass;
                pickupObjectRigidbody = null;

                objectAngularDrag = 0;
                objectDrag = 0;
                objectMass = 0;
                return;
            }

            // Object movement
            pickupObjectRigidbody.angularVelocity = Vector3.zero;
            Vector3 DirectionToPoint = pickupPoint.transform.position - pickupObjectRigidbody.transform.position;
            pickupObjectRigidbody.AddForce(DirectionToPoint * pickupPointDistance * 500f, ForceMode.Acceleration);

            pickupObjectRigidbody.velocity = Vector3.zero;

            // Object rotation
            objectRotation = Quaternion.LookRotation(transform.position - pickupObjectRigidbody.transform.position);
            objectRotation = Quaternion.Slerp(transform.rotation, objectRotation, 1 * Time.fixedDeltaTime);
            pickupObjectRigidbody.MoveRotation(objectRotation);

            
        }
    }

    /// <summary>
    /// Checks if activeHighlight game object has interface IInteractible, then calls Interact() method if the interface was found.
    /// </summary>
    /// <param name="context"></param>
    public void Interact(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
        {
            try
            {
                if (activeHighlight.gameObject.TryGetComponent(out IInteractable interactObj))
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
    }

    /// <summary>
    /// Checks if there are any interactibles in front of you, then calculates the shortest distance between hitInfo.point and interactible and highlights the item with HighLight Object.
    /// </summary>
    public void CheckInteractibles()
    {
        Ray r = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, visibleHighlightRange))
        {
            hittingAir = false;

            hitPosition = hitInfo.point;

            hitColliders = Physics.OverlapSphere(hitInfo.point, highlightRangeRadius, interactiblesMask);
            List<float> hitItemsDistance = new List<float>();
            Collider nearestItem = null;
            if (hitColliders.Length != 0)
            {
                foreach (Collider hitItem in hitColliders)
                {
                    if (hitItem.TryGetComponent(out InteractionController interactorObject))
                    {
                        if (!interactorObject.interactible)
                        {
                            continue;
                        }
                        HighlightController highlightController = hitItem.GetComponent<HighlightController>();
                        hitItemsDistance.Add(Vector3.Distance(hitInfo.transform.position, highlightController.highlightLocation.transform.position));
                    }
                }
                if (hitItemsDistance.Count != 0)
                {
                    nearestItem = hitColliders[hitItemsDistance.IndexOf(hitItemsDistance.Min())];
                    InteractionController interactor = nearestItem.GetComponent<InteractionController>();
                    Debug.DrawLine(hitInfo.point, nearestItem.transform.position);
                    if (nearestItem.TryGetComponent(out HighlightController highlightController))
                    {
                        if (!highlightController.highlightActive)
                        {
                            
                            switch (highlightController.highlightType)
                            {
                                case HighlightType.PickUp:
                                    highlightController.SpawnHighlight(highlights["pickup"], "pick up");
                                    activeHighlight = nearestItem;
                                    break;
                                
                                case HighlightType.Destroy:
                                    highlightController.SpawnHighlight(highlights["destroy"], "destroy");
                                    activeHighlight = nearestItem;
                                    break;
                                
                                case HighlightType.Interact:
                                    highlightController.SpawnHighlight(highlights["interact"], "interact");
                                    activeHighlight = nearestItem;
                                    break;
                            }
                        }
                    }
                    if (highlightController.highlight != null)
                    {
                        highlightController.highlight.transform.LookAt(transform);
                        highlightController.highlightRenderer.color = new Color(255, 255, 255, Mathf.InverseLerp(highlightRangeRadius, 0, Vector3.Distance(hitInfo.point, highlightController.highlightLocation.transform.position)));
                    }
                }
                
                
            } 
        }
        else
        {
            hittingAir = true;
            pointInAir = transform.position + (transform.forward * (visibleHighlightRange));

            hitColliders = Physics.OverlapSphere(pointInAir, highlightRangeRadius, interactiblesMask);
            List<float> hitItemsDistance = new List<float>();
            Collider nearestItem = null;
            if (hitColliders.Length != 0)
            {
                foreach (Collider hitItem in hitColliders)
                {
                    if (hitItem.TryGetComponent(out InteractionController interactorObject))
                    {
                        if (!interactorObject.interactible)
                        {
                            continue;
                        }
                        HighlightController highlightController = hitItem.GetComponent<HighlightController>();
                        hitItemsDistance.Add(Vector3.Distance(pointInAir, highlightController.highlightLocation.transform.position));
                    }
                }
                if (hitItemsDistance.Count != 0)
                {
                    nearestItem = hitColliders[hitItemsDistance.IndexOf(hitItemsDistance.Min())];
                    InteractionController interactor = nearestItem.GetComponent<InteractionController>();
                    Debug.DrawLine(pointInAir, nearestItem.transform.position);
                    if (nearestItem.TryGetComponent(out HighlightController highlightController))
                    {
                        if (!highlightController.highlightActive)
                        {
                            switch (highlightController.highlightType)
                            {
                                case HighlightType.PickUp:
                                    highlightController.SpawnHighlight(highlights["pickup"], "pick up");
                                    activeHighlight = nearestItem;
                                    break;
                                
                                case HighlightType.Destroy:
                                    highlightController.SpawnHighlight(highlights["destroy"], "destroy");
                                    activeHighlight = nearestItem;
                                    break;
                                
                                case HighlightType.Interact:
                                    highlightController.SpawnHighlight(highlights["interact"], "interact");
                                    activeHighlight = nearestItem;
                                    break;
                            }
                        }
                    }
                    if (highlightController.highlight != null)
                    {
                        highlightController.highlight.transform.LookAt(transform);
                        highlightController.highlightRenderer.color = new Color(255, 255, 255, Mathf.InverseLerp(highlightRangeRadius, 0, Vector3.Distance(pointInAir, highlightController.highlightLocation.transform.position)));
                    }
                }
            } 
        }

        if (hitColliders.Length == 0)
        {
            activeHighlight = null;
            GameObject[] allHighlights = GameObject.FindGameObjectsWithTag("Highlight");
            foreach(GameObject activeHighlight in allHighlights)
            {
                try
                {
                    HighlightController highlightController = activeHighlight.GetComponent<HighlightParent>().parent.GetComponent<HighlightController>();
                    highlightController.TurnOffHighlight();
                }
                catch(NullReferenceException)
                {
                }
                
                UnityEngine.Object.Destroy(activeHighlight);
            }
            
        }
        
        if (activeHighlight != null)
        {
            try
            {
                foreach (Collider hitCollider in hitColliders)
                {
                    if (activeHighlight != hitCollider)
                    {
                        if (hitCollider.TryGetComponent(out HighlightController highlightObject))
                        {
                            highlightObject.DestroyHighlight();
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {

            }
            
        }
    }

    public void PickUp(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
        {
            Ray r = new Ray(transform.position, transform.forward);

            if (Physics.SphereCast(transform.position, .25f, transform.forward, out RaycastHit hitInfo, maxPickupDistance, pickupLayer))
            {
                float objectWeight = Physics.gravity.y * hitInfo.rigidbody.mass;
                if (objectWeight <= 10)
                {
                    pickupObjectRigidbody = hitInfo.rigidbody;

                    objectMass = pickupObjectRigidbody.mass;
                    objectDrag = pickupObjectRigidbody.drag;
                    objectAngularDrag = pickupObjectRigidbody.angularDrag;

                    pickupObjectRigidbody.useGravity = false;
                    pickupObjectRigidbody.angularDrag = 200f;
                    float distanceToObject = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(pickupObjectRigidbody.transform.position.x, pickupObjectRigidbody.transform.position.z));
                    pickupPoint.transform.position = transform.position + transform.forward * distanceToObject;
                }
                else
                {
                    Debug.Log("Object too heavy!");
                }
                
                
            }
        }
        else if(_context.phase == InputActionPhase.Canceled)
        {
            if (pickupObjectRigidbody)
            {
                pickupObjectRigidbody.useGravity = true;
                pickupObjectRigidbody.angularDrag = objectAngularDrag;
                pickupObjectRigidbody.drag = objectDrag;
                pickupObjectRigidbody.mass = objectMass;

                pickupObjectRigidbody.velocity /= dropVelocityReduction;
                pickupObjectRigidbody = null;

                objectAngularDrag = 0;
                objectDrag = 0;
                objectMass = 0;
                return;
            }
        }
    }

    public void Zoom(InputAction.CallbackContext _context)
    {
        if(_context.ReadValue<float>() > 0)
        {
            if (pickupPointDistance < maxPickupPointDistance)
            {
                pickupPoint.transform.position = transform.position + transform.forward * (pickupPointDistance + zoomInterval);
            }
        }
        else if (_context.ReadValue<float>() < 0)
        {
           if (pickupPointDistance > minPickupPointDistance)
            {
                pickupPoint.transform.position = transform.position + transform.forward * (pickupPointDistance - zoomInterval);
            }
        }
    }

    public void Throw(InputAction.CallbackContext _context)
    {
        if (pickupObjectRigidbody)
        {
            if (_context.phase == InputActionPhase.Performed)
            {
                pickupObjectRigidbody.AddForce(transform.position + transform.forward * (throwStrength * 100), ForceMode.Acceleration);

                pickupObjectRigidbody.useGravity = true;
                pickupObjectRigidbody.angularDrag = objectAngularDrag;
                pickupObjectRigidbody.drag = objectDrag;
                pickupObjectRigidbody.mass = objectMass;

                pickupObjectRigidbody.velocity /= dropVelocityReduction;

                pickupObjectRigidbody = null;

                objectAngularDrag = 0;
                objectDrag = 0;
                objectMass = 0;
                return;
            }
        }
    }

    private void OnDrawGizmos() 
    {
        if (hittingAir)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointInAir, highlightRangeRadius / 2);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hitPosition, highlightRangeRadius);
        }

        Gizmos.DrawWireSphere(transform.position + transform.forward * maxPickupDistance, .25f);
        
    }
}
