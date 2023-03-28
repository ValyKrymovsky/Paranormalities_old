using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;

interface IInteractable
{
    public void Interact();
}

interface IHighlight
{
    public void InstantiatePopup(GameObject target, string name);
    public void DestroyPopup();
}

public class P_Interactor : MonoBehaviour
{
    [Header("Interaction")]
    private Transform interactorSource;
    [SerializeField] private float interactRange;

    [Header("Highlights")]
    [SerializeField] private float visibleHighlightRange;
    [SerializeField] private float highlightRangeRadius;
    public GameObject pickItemHighlight;
    public GameObject destroyHighlight;
    public GameObject interactHighlight;
    private Dictionary<string, GameObject> highlights = new Dictionary<string, GameObject>();
    private List<Collider> activeHighlights;
    private Collider[] hitColliders;
    private Collider[] hitItems;

    private static int layerId = 6;
    private int layerMask = 1 << layerId;

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

        activeHighlights = new List<Collider>();
        hitColliders = new Collider[0];
        hitItems = new Collider[0];

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

    public void Interact(InputAction.CallbackContext context)
    {
        if ((int)context.phase == 2)
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);
            
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange)) 
            {
                Debug.DrawLine(interactorSource.position, hitInfo.transform.position, Color.yellow);
                List<float> hitItemsDistance = new List<float>();
                hitItems = Physics.OverlapSphere(hitInfo.point, 5, layerMask);
                foreach (Collider hitItem in hitItems)
                {
                    hitItemsDistance.Add(Vector3.Distance(hitInfo.transform.position, hitItem.transform.position));
                }
                Collider nearestItem = hitItems[hitItemsDistance.IndexOf(hitItemsDistance.Min())];
                if (nearestItem.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    activeHighlights.Remove(nearestItem);
                    interactObj.Interact();
                }
            }
        }
    }

    public void CheckInteractibles()
    {
        Ray r = new Ray(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, visibleHighlightRange)) 
        {
            hittingAir = false;
            hitPosition = hitInfo.point;
            hitColliders = Physics.OverlapCapsule(transform.position, hitInfo.point, highlightRangeRadius, layerMask);
            foreach(Collider hitCollider in hitColliders)
            {
                Debug.DrawLine(hitInfo.point, hitCollider.transform.position);
                if (hitCollider.TryGetComponent(out IHighlight highlightObj))
                {
                    if (!hitCollider.GetComponent<Item>().highlightActive)
                    {
                        switch (hitCollider.tag)
                        {
                            case "pickup":
                                highlightObj.InstantiatePopup(highlights["pickup"], "pick up");
                                activeHighlights.Add(hitCollider);
                                break;
                            
                            case "destroy":
                                highlightObj.InstantiatePopup(highlights["destroy"], "destroy");
                                activeHighlights.Add(hitCollider);
                                break;
                            
                            case "interact":
                                highlightObj.InstantiatePopup(highlights["interact"], "interact");
                                activeHighlights.Add(hitCollider);
                                break;
                        }
                    }
                }

                if (hitCollider.GetComponent<Item>().highlight != null)
                {
                    hitCollider.GetComponent<Item>().highlight.transform.LookAt(transform);
                    hitCollider.GetComponent<Item>().highlightRenderer.color = new Color(255, 255, 255, Mathf.InverseLerp(highlightRangeRadius, 0, Vector3.Distance(hitInfo.point, hitCollider.transform.position)));
                }
            }

        }
        else
        {
            hittingAir = true;
            pointInAir = transform.position + (transform.forward * (visibleHighlightRange));

            hitColliders = Physics.OverlapCapsule(transform.position, pointInAir, highlightRangeRadius, layerMask);
            foreach(Collider hitCollider in hitColliders)
            {
                Debug.DrawLine(pointInAir, hitCollider.transform.position);
                if (hitCollider.TryGetComponent(out IHighlight highlightObj))
                {
                    if (!hitCollider.GetComponent<Item>().highlightActive)
                    {
                        switch (hitCollider.tag)
                        {
                            case "pickup":
                                highlightObj.InstantiatePopup(highlights["pickup"], "pick up");
                                activeHighlights.Add(hitCollider);
                                break;
                            
                            case "destroy":
                                highlightObj.InstantiatePopup(highlights["destroy"], "destroy");
                                activeHighlights.Add(hitCollider);
                                break;
                            
                            case "interact":
                                highlightObj.InstantiatePopup(highlights["interact"], "interact");
                                activeHighlights.Add(hitCollider);
                                break;
                        }
                    }
                }

                if (hitCollider.GetComponent<Item>().highlight != null)
                {
                    hitCollider.GetComponent<Item>().highlight.transform.LookAt(transform);
                    hitCollider.GetComponent<Item>().highlightRenderer.color = new Color(255, 255, 255, Mathf.InverseLerp(highlightRangeRadius, 0, Vector3.Distance(pointInAir, hitCollider.transform.position)));
                }
            }
        }

        if (hitColliders.Length == 0)
        {
            // print("hitColliders array is empty");
            activeHighlights.Clear();
            GameObject[] allHighlights = GameObject.FindGameObjectsWithTag("Highlight");
            foreach(GameObject activeHighlight in allHighlights)
            {
                activeHighlight.GetComponent<HighlightParent>().parent.GetComponent<Item>().highlight = null;
                activeHighlight.GetComponent<HighlightParent>().parent.GetComponent<Item>().highlightRenderer = null;
                activeHighlight.GetComponent<HighlightParent>().parent.GetComponent<Item>().highlightActive = false;
                Object.Destroy(activeHighlight);
            }
            
        }
        else
        {
            foreach (Collider activeCollider in activeHighlights)
            {
                if (!hitColliders.Contains(activeCollider))
                {
                    if (activeCollider.TryGetComponent(out IHighlight highlightObject))
                    {
                        highlightObject.DestroyPopup();
                        activeHighlights.Remove(activeCollider);
                    }
                }
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
