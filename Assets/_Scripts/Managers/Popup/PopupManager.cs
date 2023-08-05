using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System;
using MyCode.GameData.Interaction;
using MyCode.GameData.GameSettings;
using MyCode.GameData.GameSave;

namespace MyCode.Managers
{
    public class PopupManager : Manager<PopupManager>
    {

        public override async UniTask SetUpNewManager(DifficultyProperties _properties)
        {
            if (_popupObject != null) return;

            _popupObject = FindObjectOfType<InteractionPopup>();

            if (_popupObject != null) return;

            await LoadPopup();
        } 

        public override async UniTask SetUpExistingManager(GameSave _save)
        {
            if (_popupObject != null) return;

            _popupObject = FindObjectOfType<InteractionPopup>();

            if (_popupObject != null) return;

            await LoadPopup();
        }

        private static async UniTask<InteractionPopup> LoadPopup()
        {
            AsyncOperationHandle instantiationHandler = Addressables.InstantiateAsync("InteractionPopup");
            await instantiationHandler.Task;

            if (!instantiationHandler.Status.Equals(AsyncOperationStatus.Succeeded)) return instantiationHandler.Result as InteractionPopup;

            _popupObject = ((GameObject)instantiationHandler.Result).GetComponent<InteractionPopup>();
            DontDestroyOnLoad(_popupObject);

            return instantiationHandler.Result as InteractionPopup;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                _popupObject = this.gameObject.GetComponent<InteractionPopup>();
                DontDestroyOnLoad(gameObject);
            }
        }


        [SerializeField] private static InteractionPopup _popupObject;
        [field: SerializeField] private InteractionPopupData _popupData;
        public InteractionPopup PopupObject { get => _popupObject; set => _popupObject = value; }
        public InteractionPopupData PopupData { get => _popupData; set => _popupData = value; }
    }
}

