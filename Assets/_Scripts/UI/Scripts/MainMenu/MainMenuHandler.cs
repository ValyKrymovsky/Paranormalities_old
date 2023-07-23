using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UIElements;
using System;
using System.Linq;
using MyCode.Data.Settings;

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
    private Button _play;
    private Button _loadSave;
    private Button _options;
    private Button _exit;
    [Space]

    [Header("Elements - MainMenu")]
    [Space]
    private VisualElement _difficultySelection;

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
        _play = _root.Q<Button>("NewGame");
        _loadSave = _root.Q<Button>("LoadSave");
        _options = _root.Q<Button>("Options");
        _exit = _root.Q<Button>("Exit");

        _play.clicked += OpenDifficultySelection;

        // Difficulty selection buttons
        _difficultySelection = _root.Q<VisualElement>("DifficultySelection");
        SpawnDifficultyButtons();

        _difficultySelection.style.display = DisplayStyle.None;
    }

    private void OpenDifficultySelection()
    {
        _title.style.display = DisplayStyle.None;
        _menu.style.display = DisplayStyle.None;

        _difficultySelection.style.display = DisplayStyle.Flex;
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

    private void activateManagerLoader(DifficultyProperties _properties)
    {
        ManagerLoader.LoadNewManagers(_properties);
    }
}
