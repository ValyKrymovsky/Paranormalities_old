using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;


namespace MyCode.Player
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager _instance;
        public static PopupManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PopupManager>();
                }


                if (_instance == null)
                {
                    Time.timeScale = 0;
                    Addressables.LoadAssetAsync<GameObject>("PopupManager").Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            _instance = Instantiate(handle.Result).GetComponent<PopupManager>();
                            Time.timeScale = 1;
                        }
                        else
                        {
                        }
                    };
                }


                return _instance;
            }
        }

        public static async UniTask<PopupManager> LoadManager()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PopupManager>();
            }


            if (_instance == null)
            {
                AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("PopupManager");

                await managerHandle.Task;

                if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
                {
                    _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<PopupManager>();

                    if (_popupObject == null)
                    {
                        _popupObject = GameObject.FindObjectOfType<InteractionPopup>();
                    }

                    if (_popupObject == null)
                    {
                        LoadInteractionPopup();
                    }

                    return _instance;
                }
                return null;
            }

            return _instance;
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

        private static async void LoadInteractionPopup()
        {
            AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("InteractionPopup");

            await managerHandle.Task;

            if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
            {
                _popupObject = Instantiate((GameObject)managerHandle.Result).GetComponent<InteractionPopup>();
                DontDestroyOnLoad(_popupObject.gameObject);
            }
        }



        [SerializeField] private static InteractionPopup _popupObject;
        [field: SerializeField] private InteractionPopupData _popupData;
        public InteractionPopup PopupObject { get => _popupObject; set => _popupObject = value; }
        public InteractionPopupData Popup { get => _popupData; set => _popupData = value; }
    }
}

