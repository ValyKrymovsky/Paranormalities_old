using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Linq;
using MyCode.Data.Player;
using MyCode.Player.Sound;


namespace MyCode.Managers
{
    public class PlayerSoundManager : MonoBehaviour
    {
        private static PlayerSoundManager _instance;
        public static PlayerSoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerSoundManager>();
                }


                if (_instance == null)
                {
                    Addressables.LoadAssetAsync<GameObject>("PlayerSoundManager").Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            _instance = Instantiate(handle.Result).GetComponent<PlayerSoundManager>();
                        }
                    };
                }


                return _instance;
            }
        }

        public static async UniTask<PlayerSoundManager> LoadManager()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerSoundManager>();
            }


            if (_instance == null)
            {
                AsyncOperationHandle managerHandle = Addressables.LoadAssetAsync<GameObject>("PlayerSoundManager");

                await managerHandle.Task;

                if (managerHandle.Status.Equals(AsyncOperationStatus.Succeeded))
                {
                    _instance = Instantiate((GameObject)managerHandle.Result).GetComponent<PlayerSoundManager>();
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

        public void LoadSoundObjects()
        {
            for (int i = 0; i < _soundData.ListCapacity; i++)
            {
                _soundData.SoundObjects.Add(new SoundObject(0, 10, false, null, Vector3.zero));
            }
        }

        public SoundObject UseSoundObject()
        {
            if (_soundData.SoundObjects.Count == 0) return null;

            SoundObject sound = _soundData.SoundObjects.First();
            _soundData.SoundObjects.Remove(sound);
            sound.Active = true;

            return sound;
        }

        public void ReturnSoundObject(SoundObject _soundObject)
        {
            if (_soundObject == null) return;

            _soundObject.ResetProperties();
            _soundData.SoundObjects.Add(_soundObject);
        }

        [field: SerializeField] private PlayerSoundData _soundData;
        public PlayerSoundData SoundData { get => _soundData; }
    }
}

