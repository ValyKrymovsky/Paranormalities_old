using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{

    public enum openSide
    {
        left,
        right,
        both,
    }

    public GameObject door;
    public GameObject doorHandle;
    public GameObject pivotObject;

    public openSide side;

    private Vector3 pivot;

    public bool lockable;
    public bool locked;
    public bool closed;

    public float openTime;
    public float closeTime;

    public void Awake()
    {
        door = this.gameObject;
        pivot = pivotObject.transform.position;
        closed = true;
    }

    public void Interact()
    {
        Debug.Log("Door");
        
    }
}
