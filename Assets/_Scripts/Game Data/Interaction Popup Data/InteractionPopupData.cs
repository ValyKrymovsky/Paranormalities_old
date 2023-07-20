using UnityEngine;
using MyBox;
using System;

[CreateAssetMenu(fileName = "NewInteractionPopupData", menuName = "DataObjects/InteractionPopup/Popup")]
public class InteractionPopupData : ScriptableObject
{

    [Header("Text parameters")]
    [Space]
    [SerializeField] private float _maxTextSize;
    [SerializeField] private float _minTextSize;

    private bool isVisible;

    public event Action<Transform> OnParentChange;
    public event Action<float> OnOpacityChange;
    public event Action<float> OnSizeChange;

    public float MaxTextSize { get => _maxTextSize; set => _maxTextSize = value; }
    public float MinTextSize { get => _minTextSize; set => _minTextSize = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }

    public void InvokeOnParentChange(Transform _parent)
    {
        OnParentChange?.Invoke(_parent);
    }

    public void InvokeOnOpacityChange(float _opacity)
    {
        OnOpacityChange?.Invoke(_opacity);
    }

    public void InvokeOnSizeChange(float _size)
    {
        OnSizeChange?.Invoke(_size);
    }
}
