using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightParent : MonoBehaviour
{
    public GameObject parent;
    void Start()
    {
        parent = transform.parent.gameObject;
    }
}
