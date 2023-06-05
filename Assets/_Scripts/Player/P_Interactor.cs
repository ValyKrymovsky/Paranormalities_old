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
    private Transform interactorSource;
    [SerializeField]
    private float interactRange;
    [SerializeField]
    private LayerMask interactiblesMask;

    [Separator("Highlights", true)]
    [SerializeField] private float visibleHighlightRange;
    [SerializeField] private float highlightRangeRadius;
    public GameObject pickItemHighlight;
    public GameObject destroyHighlight;
    public GameObject interactHighlight;
    private Dictionary<string, GameObject> highlights = new Dictionary<string, GameObject>();
    [SerializeField]
    private Collider activeHighlight;
    private Collider nearestItem;
    private Collider[] hitColliders;

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
        interactorSource = transform;

        hitColliders = new Collider[0];

        pickItemHighlight.tag = "Highlight";
        destroyHighlight.tag = "Highlight";
        interactHighlight.tag = "Highlight";

        highlights.Add("pickup", pickItemHighlight);
        highlights.Add("destroy", destroyHighlight);
        highlights.Add("interact", interactHighlight);
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
    }

    /// <summary>
    /// Checks if activeHighlight game object has interface IInteractible, then calls Interact() method if the interface was found.
    /// </summary>
    /// <param name="context"></param>
    public void Interact(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 3)
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
                Debug.Log("Not interactible");
            }
        }
    }

    /// <summary>
    /// Checks if there are any interactibles in front of you, then calculates the shortest distance between hitInfo.point and interactible and highlights the item with HighLight Object.
    /// </summary>
    public void CheckInteractibles()
    {
        Ray r = new Ray(interactorSource.position, interactorSource.forward);

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
        
    }
}