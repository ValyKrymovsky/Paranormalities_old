using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class InteractionController : MonoBehaviour
{

    [Separator("Interaction", true)]
    [SerializeField] private bool interactible = true;

    [Separator("Highlight", true)]
    [SerializeField] private string popupText;
    [SerializeField] private bool customPopupLocation;
    [SerializeField, ConditionalField("customPopupLocation")] public GameObject customPopupLocationObject;
    [ReadOnly] public Vector3 popupLocation;
    private TextMeshPro textComponent;

    private GameObject activePopup;

    private void Awake()
    {
        popupLocation = customPopupLocation ? customPopupLocationObject.transform.position : transform.position;
        
    }

    public bool IsInteractible()
    {
        return interactible;
    }

    public void SetInteractible(bool _interactible)
    {
        interactible = _interactible;
    }

    public bool HasInteractionPopup()
    {
        if (activePopup)
        {
            return true;
        }

        return false;
    }

    public GameObject GetInteractionPopup()
    {
        return activePopup;
    }

    public void SetTextOpacity(float _opacity)
    {
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, _opacity);
    }

    public void SetTextSize(float _size)
    {
        textComponent.fontSize = _size;
    }
}
