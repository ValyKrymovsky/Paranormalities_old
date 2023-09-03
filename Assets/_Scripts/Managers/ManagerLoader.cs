using System.Collections.Generic;
using UnityEngine;
using MyCode.GameData.GameSettings;
using Cysharp.Threading.Tasks;
using MyCode.GameData.Scene;
using MyCode.GameData.GameSave;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Runtime.Serialization.Json;

namespace MyCode.Managers
{
    public class ManagerLoader : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference _managerGroupLabel;
        private AsyncOperationHandle _handle;
        private Dictionary<Type, GameObject> _managerList;
        [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();

        public event Action OnNewGame;
        public event Action OnLoadGame;

        public List<DifficultyProperties> DifficultyProperties { get => difficultyProperties; private set => difficultyProperties = value; }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public async void CreateManagers(DifficultyProperties _difficultyProp)
        {
            if (_handle.IsValid())
                DeleteAllManagers(_handle);

            _managerList = await LoadAllManagers(_managerGroupLabel, _managerList);
            
            SettingsManager.CreateManager(_managerList[typeof(SettingsManager)]);
            GameSaveManager.CreateManager(_managerList[typeof(GameSaveManager)]);
            PopupManager.CreateManager(_managerList[typeof(PopupManager)]);
            PlayerManager.CreateManager(_managerList[typeof(PlayerManager)]);
            PlayerSoundManager.CreateManager(_managerList[typeof(PlayerSoundManager)]);

            UniTask[] taskPool = new UniTask[]
            {
                PlayerManager.Instance.SetUpNewManager(_difficultyProp),
                PopupManager.Instance.SetUpNewManager(_difficultyProp),
            };

            await UniTask.WhenAll(taskPool);

            GameSaveManager.Instance.SetUpNewManager(_difficultyProp);
            
            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);
            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnNewGame?.Invoke();
        }

        public async void LoadManagers(GameSave _gameSave)
        {
            if (_handle.IsValid())
                DeleteAllManagers(_handle);

            _managerList = await LoadAllManagers(_managerGroupLabel, _managerList);

            SettingsManager.CreateManager(_managerList[typeof(SettingsManager)]);
            GameSaveManager.CreateManager(_managerList[typeof(GameSaveManager)]);
            PlayerManager.CreateManager(_managerList[typeof(PlayerManager)]);
            PopupManager.CreateManager(_managerList[typeof(PopupManager)]);
            PlayerSoundManager.CreateManager(_managerList[typeof(PlayerSoundManager)]);

            UniTask[] taskPool = new UniTask[]
            {
                GameSaveManager.Instance.SetUpExistingManager(_gameSave),
                PlayerManager.Instance.SetUpExistingManager(_gameSave),
                PlayerSoundManager.Instance.SetUpExistingManager(_gameSave),
            };

            await UniTask.WhenAll(taskPool);

            await PopupManager.Instance.SetUpExistingManager(_gameSave);

            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);

            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnLoadGame?.Invoke();

            PlayerManager.InvokeOnPlayerTeleport(_gameSave);
        }

        private void DeleteAllManagers(AsyncOperationHandle _handle)
        {
            Addressables.ReleaseInstance(_handle);
            Debug.Log("Released handle");
            _managerList.Clear();
        }

        private async UniTask<Dictionary<Type, GameObject>> LoadAllManagers(AssetLabelReference _label, Dictionary<Type, GameObject> _list)
        {
            if (_list == null) _list = new Dictionary<Type, GameObject>();
            _handle = Addressables.LoadAssetsAsync<GameObject>(_label, (a) =>
            {
                Type type = Type.GetType($"MyCode.Managers.{a.GetComponent<ManagerType>().managerType}");
                _list.Add(type, a);
            });

            await _handle.Task;

            return _list;
        }
    }

}
