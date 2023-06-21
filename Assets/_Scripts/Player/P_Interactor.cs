using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;

public interface IInteractionPopup
{
    public void SpawnPopup(GameObject _popupObject);
    public void TurnOffPopup();
    public void TurnOnPopup();
    public void DestroyPopup();
}

public interface IInteraction
{
    public void Interact();
}


public class P_Interactor : MonoBehaviour
{
    [Separator("Interaction", true)]

    [Header("Interaction")]
    [Space]
    [SerializeField] private float interactRange;
    [SerializeField] private float interactiblesCheckRange;
    [SerializeField] private Vector3 hitPosition;
    [SerializeField] private LayerMask interactiblesMask;
    [SerializeField] private Collider markedCollider;
    private InteractionController interactionController;
    [Space]

    [Header("Interaction Popup")]
    [Space]
    [SerializeField] private GameObject interactionPopup;
    [SerializeField] private GameObject activePopup;
    [Space]

    [Header("Text parameters")]
    [Space]
    [SerializeField] private float maxTextSize;
    [SerializeField] private float minTextSize;

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
    [Tooltip("Reduces the velocity by a multiplier. (Useful for adding more weight to objects)")]
    [SerializeField, MinValue(1)] private float dropVelocityReduction;
    [Space]

    [Header("Throw")]
    [Space]
    [SerializeField] private float throwStrength;
    [Space]

    [Header("Zoom")]
    [Space]
    private float zoomInterval;




    private Rigidbody pickupObjectRigidbody;
    private float objectMass = 0;
    private float objectDrag = 0;
    private float objectAngularDrag = 0;

    // Components //
    private P_Controls p_input;
    private Camera p_camera;

    private bool hittingAir;


    // Input Actions //
    private InputAction ac_interact;  // input action for interacting
    private Vector3 pointInAir;


    void Awake()
    {
        p_input = new P_Controls();
        ac_interact = p_input.Player.Interact;
        p_camera = GetComponent<Camera>();

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
        if (!pickupObjectRigidbody)
        {
            CheckInteractibles();
        }
    }

    private void FixedUpdate()
    {
        pickupPointDistance = Vector3.Distance(transform.position, pickupPoint.transform.position);
        
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
        }
    }

    /// <summary>
    /// Checks if activeHighlight game object has interface IInteractible, then calls Interact() method if the interface was found.
    /// </summary>
    /// <param name="context"></param>
    public void Interact(InputAction.CallbackContext _context)
    {
        // if (_context.phase == InputActionPhase.Performed)
        // {
        //     try
        //     {
        //         if (markedCollider.gameObject.TryGetComponent(out IInteraction interactObj))
        //         {
        //             interactObj.Interact();
        //         }
        //     }
        //     catch(NullReferenceException)
        //     {
        //         Debug.Log("Interaction script not found");
        //     }
        //     catch(UnassignedReferenceException)
        //     {
        //         Debug.Log("Object cannot be interacted with");
        //     }
        // }
    }

    /// <summary>
    /// Checks if there are any interactibles in front of you, then calculates the shortest distance between hitInfo.point and interactible and highlights the item with HighLight Object.
    /// </summary>
    public void CheckInteractibles()
    {
        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Physics.Raycast(r, out hit, interactRange);

        Debug.DrawLine(transform.position, transform.position + transform.forward * interactRange, Color.red);

        hitPosition = hit.collider != null ? hit.point : transform.position + transform.forward * interactRange;

        Collider[] hitColliders = Physics.OverlapSphere(hitPosition, interactiblesCheckRange, interactiblesMask);

        List<Collider> interactibleColliders = new List<Collider>();

        Collider nearestInteractibleCollider = null;

        if (hitColliders.Length != 0)
        {
            foreach(Collider collider in hitColliders)
            {
                if (collider.TryGetComponent(out InteractionController tempIntController))
                {
                    if (tempIntController.IsInteractible()) interactibleColliders.Add(collider);
                }
            }

            if (interactibleColliders.Count != 0)
            {
                float minDistance = -1;
                foreach(Collider collider in interactibleColliders)
                {
                    if (minDistance == -1)
                    {
                        minDistance = Vector3.Distance(hitPosition, collider.transform.position);
                        nearestInteractibleCollider = collider;
                        continue;
                    }
                    else
                    {
                        float distance = Vector3.Distance(hitPosition, collider.transform.position);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestInteractibleCollider = collider;
                            continue;
                        }
                    }

                }

                if (markedCollider != nearestInteractibleCollider)
                {
                    InteractionController tempIntController;

                    if (markedCollider)
                    {
                        markedCollider.gameObject.TryGetComponent(out tempIntController);

                        if (tempIntController.HasInteractionPopup())
                        {
                            IInteractionPopup interactionPopupInterface = tempIntController.GetComponent<IInteractionPopup>();
                            interactionController = null;
                            interactionPopupInterface.DestroyPopup();
                        }
                    }
                    
                    nearestInteractibleCollider.gameObject.TryGetComponent(out tempIntController);

                    if (!tempIntController.HasInteractionPopup())
                    {
                        IInteractionPopup interactionPopupInterface = tempIntController.GetComponent<IInteractionPopup>();
                        interactionController = tempIntController;
                        interactionPopupInterface.SpawnPopup(interactionPopup);
                        activePopup = interactionController.GetInteractionPopup();
                    }

                    markedCollider = nearestInteractibleCollider;
                }

                if (interactionController)
                {
                    activePopup.transform.LookAt(transform, transform.up);

                    float proximityTextOpacity = Mathf.InverseLerp(interactiblesCheckRange, 0, Vector3.Distance(hitPosition, activePopup.transform.position));
                    interactionController.SetTextOpacity(proximityTextOpacity);

                    Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
                    Vector2 popupPosition = new Vector2(activePopup.transform.position.x, activePopup.transform.position.z);
                    float proximityTextSize = Mathf.InverseLerp(interactRange + interactiblesCheckRange, 0, Vector2.Distance(playerPosition, popupPosition)) * maxTextSize;
                    if (proximityTextSize < minTextSize) proximityTextSize = minTextSize;
                    Debug.Log(proximityTextSize);
                    interactionController.SetTextSize(proximityTextSize);
                }

            }

        }
        else
        {
            if (markedCollider) markedCollider = null;
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

                    interactionController.TurnOffPopup();

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
                interactionController.TurnOnPopup();

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

                interactionController.TurnOnPopup();

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
        // if (hittingAir)
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(pointInAir, highlightRangeRadius / 2);
        // }
        // else
        // {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(hitPosition, highlightRangeRadius);
        // }

        // Gizmos.DrawWireSphere(transform.position + transform.forward * maxPickupDistance, .25f);     
    }
}
