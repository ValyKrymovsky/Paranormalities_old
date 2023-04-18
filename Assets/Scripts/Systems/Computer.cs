using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    private bool interacted;

    public bool GetInteracted()
    {
        return interacted;
    }

    public void SetInteracted(bool _value)
    {
        interacted = _value;
    }

    public void Interact()
    {
        if (!interacted)
        {
            interacted = true;
        }
    }
}
