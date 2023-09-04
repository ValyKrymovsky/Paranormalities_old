using UnityEngine;
using MyBox;
using UnityEngine.UIElements;
using System;
using System.Linq;
using MyCode.GameData.GameSettings;
using System.Collections;
using MyCode.Helper.Serializer;
using System.IO;
using System.Text.RegularExpressions;
using MyCode.GameData.GameSave;
using MyCode.Managers;

namespace MyCode.UI.MainMenu
{
    public class MainMenuHandler : MonoBehaviour
    {
        [Space]
        [Separator("MainMenu UI")]
        [Space]

        [Header("UI Document")]
        [Space]
        [SerializeField] private UIDocument _mainMenu;
        [Space]

        [Header("Elements - MainMenu")]
        [Space]
        private VisualElement _root;
        private VisualElement _title;
        private VisualElement _menu;
        private Button _newSave;
        private Button _loadSave;
        private Button _options;
        private Button _exit;
        [Space]

        [Header("Elements - DifficultySelection")]
        [Space]
        private VisualElement _difficultySelection;
        private VisualElement _difficultyButtonContainer;
        private Button _difficultyBackButton;
        [Space]

        [Header("Elements -GameSaveSelection")]
        [Space]
        private VisualElement _gameSaveSelection;
        private ListView _gameSaveList;
        private Button _gameSaveBackButton;
        private GameSave[] gameSaves;

        public ManagerLoader _loader;

        private void Awake()
        {
            _loader = FindObjectOfType<ManagerLoader>();

            _mainMenu = GetComponent<UIDocument>();
            _root = _mainMenu.rootVisualElement;

            // Main menu containers
            _title = _root.Q<VisualElement>("Title");
            _menu = _root.Q<VisualElement>("Menu");

            // Main menu buttons
            _newSave = _root.Q<Button>("NewGame");
            _loadSave = _root.Q<Button>("LoadSave");
            _options = _root.Q<Button>("Options");
            _exit = _root.Q<Button>("Exit");

            _newSave.clicked += OpenDifficultySelection;
            _loadSave.clicked += OpenGameSaveSelection;

            // Difficulty selection buttons
            _difficultySelection = _root.Q<VisualElement>("DifficultySelection");
            _difficultyButtonContainer = _difficultySelection.Q<VisualElement>("DifficultyButtonContainer");
            SpawnDifficultyButtons(_difficultyButtonContainer);
            _difficultyBackButton = _difficultySelection.Q<Button>("BackButton");
            _difficultyBackButton.clicked += BackToMainMenu;


            // Game save selection
            _gameSaveSelection = _root.Q<VisualElement>("GameSaveSelection");
            _gameSaveList = _gameSaveSelection.Q<ListView>("GameSaveView");
            _gameSaveBackButton = _gameSaveSelection.Q<Button>("BackButton");
            _gameSaveBackButton.clicked += BackToMainMenu;

            _difficultySelection.style.display = DisplayStyle.None;
        }

        private void OpenDifficultySelection()
        {
            _title.style.display = DisplayStyle.None;
            _menu.style.display = DisplayStyle.None;

            _difficultySelection.style.display = DisplayStyle.Flex;
        }

        private void OpenGameSaveSelection()
        {
            _title.style.display = DisplayStyle.None;
            _menu.style.display = DisplayStyle.None;

            gameSaves = DeserializeAllSaves(FindAllSaveFiles(), gameSaves);

            /*foreach (GameSave save in gameSaves)
            {
                Debug.Log(save.SaveName);
            }*/

            LoadGameSaves(gameSaves);

            _gameSaveList.Rebuild();

            _gameSaveSelection.style.display = DisplayStyle.Flex;
        }

        private void BackToMainMenu()
        {
            _gameSaveSelection.style.display = DisplayStyle.None;
            _difficultySelection.style.display = DisplayStyle.None;

            _title.style.display = DisplayStyle.Flex;
            _menu.style.display = DisplayStyle.Flex;
        }

        private void SpawnDifficultyButtons(VisualElement _parent)
        {
            foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
            {
                DifficultyButton buttonElement = new DifficultyButton();
                buttonElement.Difficulty = diff;
                buttonElement.name = diff.ToString();
                buttonElement.AddToClassList("mainButton");
                buttonElement.Button.text = diff.ToString();
                _parent.Add(buttonElement);

                buttonElement.Button.clicked += () => _loader.CreateManagers(_loader.DifficultyProperties.Where(d => d.difficulty.Equals(buttonElement.Difficulty)).First());
            }
        }

        private string[] FindAllSaveFiles()
        {
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace('\\', '/') + "/My Games/Paranormalities/Saves/";

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            return Directory.EnumerateFiles(savePath).Where(file => Regex.IsMatch(file, @"(Save\d+_\d+\.json)")).ToArray();
        }

        private string FileToJson(string _path)
        {
            return SaveSerializer.ReadSaveFile(_path);
        }

        private GameSave[] DeserializeAllSaves(string[] _paths, GameSave[] _arrayToFill)
        {
            if (_paths.Length == 0 || _paths == null) return null;
            if (_arrayToFill == null) _arrayToFill = new GameSave[_paths.Length];

            for (int i = 0; i < _paths.Length; i++)
            {
                _arrayToFill[i] = SaveSerializer.DeserializeGameSave(FileToJson(_paths[i]));
            }

            return _arrayToFill;
        }

        private void LoadGameSaves(GameSave[] _saveList)
        {
            _gameSaveList.makeItem = () => new GameSaveContainer();

            _gameSaveList.bindItem = (item, index) =>
            {
                (item as GameSaveContainer).GameSave = _saveList[index];
                (item as GameSaveContainer).GameSavePath = _saveList[index].SavePath;
                (item as GameSaveContainer).SaveName.text = _saveList[index].SaveName;
                (item as GameSaveContainer).SaveDate.text = _saveList[index].SaveTime.ToString();

                (item as GameSaveContainer).OnLoadSave += (s) =>
                {
                    _loader.LoadManagers(s);
                };

                (item as GameSaveContainer).OnDeleteSave += (e) =>
                {
                    _saveList = _saveList.RemoveAt(index);
                    _gameSaveList.itemsSource = _saveList;

                    _gameSaveList.Rebuild();
                };
            };

            _gameSaveList.itemsSource = _saveList;
        }
    }

}
