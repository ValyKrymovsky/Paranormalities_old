using UnityEngine;
using MyBox;
using System;

namespace MyCode.GameData.Interaction
{
    public class InteractionController : MonoBehaviour
    {

        [Separator("Interaction", true)]
        [SerializeField] private bool _interactible = true;
        [SerializeField] private InteractionType _interactionType;

        [Separator("Highlight", true)]
        [SerializeField] private string _popupText;
        [SerializeField] private bool _customPopupLocation;
        [SerializeField, ConditionalField("_customPopupLocation")] public GameObject customPopupLocationObject;
        [ReadOnly] private Vector3 popupLocation;

        public event Action OnInteracted;


        private void Awake()
        {
            PopupLocation = CustomPopupLocation ? customPopupLocationObject.transform.position : transform.position;
        }

        public bool Interactible { get => _interactible; set => _interactible = value; }
        public InteractionType InteractionType { get => _interactionType; private set => _interactionType = value; }
        public string PopupText { get => _popupText; set => _popupText = value; }
        public Vector3 PopupLocation { get => popupLocation; private set => popupLocation = value; }
        public bool CustomPopupLocation { get => _customPopupLocation; set => _customPopupLocation = value; }

        public void Interact()
        {
            OnInteracted?.Invoke();
        }
    }

}
