using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class InteractionController : MonoBehaviour, IInteractionPopup
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

    public void SpawnPopup(GameObject _popupObject)
    {
        if (!activePopup)
        {
            popupLocation = customPopupLocation ? customPopupLocationObject.transform.position : transform.position;
            activePopup = Instantiate(_popupObject, popupLocation, Quaternion.identity, transform);

            textComponent = activePopup.GetComponent<TextMeshPro>();
            textComponent.text = popupText;
        }
    }

    public void TurnOffPopup()
    {
        MeshRenderer renderer = activePopup.GetComponent<MeshRenderer>();

        renderer.enabled = false;
    }

    public void TurnOnPopup()
    {
        MeshRenderer renderer = activePopup.GetComponent<MeshRenderer>();

        renderer.enabled = true;
    }

    public void DestroyPopup()
    {
        Destroy(activePopup);
    }
}
