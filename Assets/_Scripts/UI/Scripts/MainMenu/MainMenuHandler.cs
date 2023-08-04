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

        [Header("Elements - MainMenu")]
        [Space]
        private VisualElement _difficultySelection;

        private VisualElement _gameSaveContainer;
        private ListView _gameSaveSelection;
        private GameSave[] gameSaves;

        public VisualTreeAsset saveAsset;

        private ManagerLoader _loader;

        private void Awake()
        {
            _loader = GetComponent<ManagerLoader>();
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
            SpawnDifficultyButtons();

            // Game save selection
            _gameSaveContainer = _root.Q<VisualElement>("GameSaveSelection");
            _gameSaveSelection = _gameSaveContainer.Q<ListView>("GameSaveView");

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

            _gameSaveSelection.Rebuild();

            _gameSaveContainer.style.display = DisplayStyle.Flex;
        }

        private void SpawnDifficultyButtons()
        {
            foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
            {
                DifficultyButton buttonElement = new DifficultyButton();
                buttonElement.Difficulty = diff;
                buttonElement.name = diff.ToString();
                buttonElement.AddToClassList("mainButton");
                buttonElement.Button.text = diff.ToString();
                _difficultySelection.Add(buttonElement);

                buttonElement.Button.clicked += () => activateManagerLoader(_loader.DifficultyProperties.Where(d => d.difficulty.Equals(buttonElement.Difficulty)).First());
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
            _gameSaveSelection.makeItem = () =>
            {
                TemplateContainer test = saveAsset.CloneTree();

                test.userData = new GameSave();

                return test;
            };

            _gameSaveSelection.bindItem = (item, index) =>
            {
                item.userData = gameSaves[index];
                item.Q<Label>("SaveName").text = gameSaves[index].SaveName;
                item.Q<Label>("SaveTime").text = gameSaves[index].SaveTime.ToString();
            };

            _gameSaveSelection.itemsSource = _saveList;
        }

        private void activateManagerLoader(DifficultyProperties _properties)
        {
            ManagerLoader.LoadNewManagers(_properties);
        }
    }

}
