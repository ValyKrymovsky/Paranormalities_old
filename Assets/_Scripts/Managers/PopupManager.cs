using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;


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
                    Addressables.LoadAssetAsync<GameObject>("InteractionPopup").Completed += (handle) =>
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

        public static async Task<PopupManager> LoadManager()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PopupManager>();
            }


            if (_instance == null)
            {
                AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("InteractionPopup");

                await managerHandle.Task;

                if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
                {
                    _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<PopupManager>();
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
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

