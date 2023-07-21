using UnityEngine;
using MyBox;
using MyCode.Managers;

namespace MyCode.Player.Interaction
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



        private void Awake()
        {
            PopupLocation = _customPopupLocation ? customPopupLocationObject.transform.position : transform.position;
        }

        public bool Interactible { get => _interactible; set => _interactible = value; }
        public InteractionType InteractionType { get => _interactionType; private set => _interactionType = value; }
        public string PopupText { get => _popupText; set => _popupText = value; }
        public Vector3 PopupLocation { get => popupLocation; private set => popupLocation = value; }

        public void Interact()
        {
            PlayerManager.Instance.InteractionData.InvokeOnInteract();
        }
    }

}
