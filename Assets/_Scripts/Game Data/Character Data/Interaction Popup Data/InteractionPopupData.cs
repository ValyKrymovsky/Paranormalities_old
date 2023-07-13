using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewInteractionPopupData", menuName = "DataObjects/InteractionPopup/Popup")]
public class InteractionPopupData : ScriptableObject
{

    [Header("Text parameters")]
    [Space]
    [SerializeField] private float _maxTextSize;
    [SerializeField] private float _minTextSize;

    public float MaxTextSize { get => _maxTextSize; set => _maxTextSize = value; }
    public float MinTextSize { get => _minTextSize; set => _minTextSize = value; }
}
