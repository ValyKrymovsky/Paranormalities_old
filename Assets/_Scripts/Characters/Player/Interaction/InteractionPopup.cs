using MyCode.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
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

        PopupManager.Instance.PopupData.OnParentChange += value => ChangeParent(value);
        PopupManager.Instance.PopupData.OnOpacityChange += value => SetTextOpacity(value);
        PopupManager.Instance.PopupData.OnSizeChange += value => SetTextSize(value);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.InteractionData.OnPickUpObject -= () => SetVisibility(false);
        PlayerManager.Instance.InteractionData.OnDropObject -= () => SetVisibility(true);

        PopupManager.Instance.PopupData.OnParentChange -= value => ChangeParent(value);
        PopupManager.Instance.PopupData.OnOpacityChange -= value => SetTextOpacity(value);
        PopupManager.Instance.PopupData.OnSizeChange -= value => SetTextSize(value);
    }

    private void ChangeParent(Transform _transform)
    {
        gameObject.transform.SetParent(_transform);
        gameObject.transform.position = _transform.position;
    }

    private void SetTextOpacity(float _opacity)
    {
        _text.alpha = _opacity;
    }

    private void SetTextSize(float _size)
    {
        _text.fontSize = _size;
    }

    private void SetVisibility(bool _state)
    {
        _renderer.enabled = _state;
        PopupManager.Instance.PopupData.IsVisible = _state;
    }

}
