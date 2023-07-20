using MyCode.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private MeshRenderer _renderer;

    public TextMeshPro Text { get => _text; private set => _text = value; }
    public MeshRenderer Renderer { get => _renderer; private set => _renderer = value; }

    private void Awake()
    {
        Text = GetComponent<TextMeshPro>();
        Renderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.InteractionData.OnPickUpObject += () => SetVisibility(false);
        PlayerManager.Instance.InteractionData.OnDropObject += () => SetVisibility(true);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InteractionData.OnPickUpObject -= () => SetVisibility(false);
        PlayerManager.Instance.InteractionData.OnDropObject -= () => SetVisibility(true);
    }

    public void ChangeTransform(Transform _transform)
    {
        gameObject.transform.SetParent(_transform);
        gameObject.transform.position = _transform.position;
    }

    public void SetTextOpacity(float _opacity)
    {
        _text.alpha = _opacity;
    }

    public void SetTextSize(float _size)
    {
        _text.fontSize = _size;
    }

    public void SetVisibility(bool _state)
    {
        _renderer.enabled = _state;
    }

    public bool IsVisible()
    {
        return _renderer.enabled;
    }

}
