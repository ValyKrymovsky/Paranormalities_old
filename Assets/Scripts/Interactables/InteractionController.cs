using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public enum HighlightType
{
    PickUp,
    Interact,
    Destroy
}

[RequireComponent(typeof(HighlightController))]
public class InteractionController : MonoBehaviour
{

    [Separator("Interaction", true)]
    public bool interactible = true;

    [Separator("Highlight", true)]
    [SerializeField]
    private HighlightController highlightController;

    
    
    private void Awake()
    {
        // highlightActive = false;
        highlightController = GetComponent<HighlightController>();
    }
}
