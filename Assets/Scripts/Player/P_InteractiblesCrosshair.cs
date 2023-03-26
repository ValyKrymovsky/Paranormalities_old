using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface IPopUp
{
    public void InstantiatePopup(GameObject target, string name);
    public void DestroyPopup();
}

public class P_InteractiblesCrosshair : MonoBehaviour
{
    [Header("Sphere Check")]
    [SerializeField] private float checkRange;

    [Header("Popups")]
    public GameObject pickItemPopup;
    public GameObject destroyPopup;
    public GameObject interactPopup;
    [SerializeField] private bool popupActive;
    private Dictionary<string, GameObject> popups = new Dictionary<string, GameObject>();

    [Header("Components")]
    public CharacterController ch_controller;

    void Awake()
    {
        ch_controller = GetComponent<CharacterController>();
        popupActive = false;

        popups.Add("pickup", pickItemPopup);
        popups.Add("destroy", destroyPopup);
        popups.Add("interact", interactPopup);
    }

    void Update()
    {
        if (Physics.SphereCast(transform.position, checkRange, transform.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IPopUp popupObj))
            {
                print(Vector3.Distance(hitInfo.collider.transform.position, this.transform.position));
                if (!hitInfo.transform.GetComponent<Item>().popupActive)
                {
                    GameObject popupObject;
                    switch (hitInfo.transform.tag)
                    {
                        case "pickup":
                            popupObject = popups["pickup"];
                            popupObj.InstantiatePopup(popupObject, "pick up");
                            break;
                        
                        case "destroy":
                            popupObject = popups["destroy"];
                            popupObj.InstantiatePopup(popupObject, "destroy");
                            break;
                        
                        case "interact":
                            popupObject = popups["interact"];
                            popupObj.InstantiatePopup(popupObject, "interact");
                            break;
                    }
                }
                
            }
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(this.transform.position, checkRange);
        Gizmos.DrawWireSphere(this.transform.position, checkRange);
    }
}
