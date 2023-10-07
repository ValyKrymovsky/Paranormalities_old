using System.Collections.Generic;
using UnityEngine;
using MyCode.GameData;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyCode.Managers
{
    public class ManagerLoader : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference _managerGroupLabel;
        [SerializeField] private List<DifficultyProperties> difficultyProperties = new List<DifficultyProperties>();
        [SerializeField] private List<SuperObjective> objectives = new List<SuperObjective>();


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
            PlayerManager.Instance.SetPlayerProperties(_difficultyProp);

            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);

            ObjectiveManager.Instance.Objectives = this.objectives;
            ObjectiveManager.Instance.CurrentSuperObjective = this.objectives[0];
            ObjectiveManager.Instance.CurrentSubObjective = this.objectives[0].subObjectives[0];

            GameSaveManager.Instance.CreateNewSave(_difficultyProp);

            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnNewGame?.Invoke();
        }

        public async void LoadManagers(GameSave _gameSave)
        {
            GameSaveManager.Instance.SetSave(_gameSave);
            PlayerManager.Instance.SetUpExistingManager(_gameSave);
            //playerSoundManager.SetUpExistingManager(_gameSave);

            // Load Main Scene
            await SceneLoader.LoadScene(MyScene.DebugScene);

            SceneLoader.SetActiveScene(MyScene.DebugScene);

            OnLoadGame?.Invoke();

            PlayerManager.Instance.InvokeOnPlayerTeleport(_gameSave);
        }
    }

}
