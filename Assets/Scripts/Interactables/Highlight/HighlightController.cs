using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class HighlightController : MonoBehaviour, IHighlight
{

    [Separator("Highlight", true)]
    public HighlightType highlightType;
    public GameObject highlight;
    [HideInInspector] public TextMeshPro highlightRenderer;
    public GameObject highlightLocation;
    public bool highlightActive;

    private void Start()
    {
        if (highlightLocation == null)
        {
            highlightLocation = gameObject;
        }    
    }

    /// <summary>
    /// Spawns highlight object if not already active.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_name"></param>
    public void SpawnHighlight(GameObject _target, string _name)
    {
        if (!highlightActive)
        {
            // _target.transform.localScale = new Vector3(_target.transform.localScale.x / gameObject.transform.localScale.x, _target.transform.localScale.y / gameObject.transform.localScale.y, _target.transform.localScale.z / gameObject.transform.localScale.z);
            _target.transform.localScale = new Vector3(.25f, .25f, .25f);
            highlight = Instantiate(_target, highlightLocation.transform.position, transform.rotation, gameObject.transform);
            highlightRenderer = highlight.GetComponentInChildren<TextMeshPro>();
            highlight.name = string.Format("{0} highlight", name);
            highlightActive = true;
        }
    }

    /// <summary>
    /// Destroys highlight object if it is active.
    /// </summary>
    public void DestroyHighlight()
    {
        if (highlightActive)
        {
            Object.Destroy(highlight);
            TurnOffHighlight();
        }
    }

    public void TurnOffHighlight()
    {
        GameObject.Destroy(highlight);
        highlight = null;
        highlightRenderer = null;
        highlightActive = false;
    }
}
