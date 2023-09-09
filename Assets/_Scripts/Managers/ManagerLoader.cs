using System.Collections.Generic;
using UnityEngine;
using MyCode.GameData.GameSettings;
using Cysharp.Threading.Tasks;
using MyCode.GameData.Scene;
using MyCode.GameData.GameSave;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using MyCode.GameData.Inventory;

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
        public AssetLabelReference ManagerGroupLabel { get => _managerGroupLabel; set => _managerGroupLabel = value; }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public async void CreateManagers(DifficultyProperties _difficultyProp)
        {
            PlayerManager playerManager = new PlayerManager();
            GameSaveManager gameSaveManager = new GameSaveManager();

            await playerManager.SetUpNewManager(_difficultyProp);

            gameSaveManager.SetUpNewManager(_difficultyProp);
            
            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);
            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnNewGame?.Invoke();
        }

        public async void LoadManagers(GameSave _gameSave)
        {
            GameSaveManager gameSaveManager = new GameSaveManager();
            PlayerManager playerManager = new PlayerManager();
            PlayerSoundManager playerSoundManager = new PlayerSoundManager();


            await gameSaveManager.SetUpExistingManager(_gameSave);
            playerManager.SetUpExistingManager(_gameSave);
            playerSoundManager.SetUpExistingManager(_gameSave);

            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);

            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnLoadGame?.Invoke();

            PlayerManager.InvokeOnPlayerTeleport(_gameSave);
        }
    }

}
