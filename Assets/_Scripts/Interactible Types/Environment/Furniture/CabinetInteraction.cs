using MyBox;
using MyCode.GameData;
using UnityEngine;

namespace MyCode.Interactibles
{

    public enum DrawerType
    {
        Top, Bottom
    }

    [RequireComponent(typeof(InteractionController))]
    public class CabinetInteraction : MonoBehaviour
    {
        public Animator _animator;

        private InteractionController _interactionController;

        private bool _isOpen = false;

        public DrawerType drawerType;

        private void Awake()
        {
            _animator = transform.parent.GetComponent<Animator>();
            _interactionController = GetComponent<InteractionController>();
        }

        private void OnEnable()
        {
            _interactionController.OnInteracted += PlayAnimation;
        }

        private void OnDisable()
        {
            _interactionController.OnInteracted -= PlayAnimation;
        }

        private void PlayAnimation()
        {
            Debug.Log("Played animation");
            if (_isOpen)
            {
                _isOpen = !_isOpen;
                _animator.SetTrigger(string.Format("{0}Close", drawerType));
                return;
            }

            _isOpen = !_isOpen;
            _animator.SetTrigger(string.Format("{0}Open", drawerType));
        }
    }
}
