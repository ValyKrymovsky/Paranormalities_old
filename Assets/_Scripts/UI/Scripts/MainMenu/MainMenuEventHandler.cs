using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class MainMenuEventHandler : MonoBehaviour
{
    [Space]
    [Separator("MainMenu UI")]
    [Space]

    [Header("UI Document")]
    [Space]
    [SerializeField] private UIDocument mainMenu;
    [Space]

    [Header("Elements - MainMenu")]
    [Space]
    private VisualElement root;
    private VisualElement title;
    private VisualElement menu;
    private Button play;
    private Button options;
    private Button exit;
    [Space]

    [Header("Elements - MainMenu")]
    [Space]
    private VisualElement difficultySelection;

    private ManagerLoader loader;

    private void Awake()
    {
        loader = GetComponent<ManagerLoader>();
        mainMenu = GetComponent<UIDocument>();
        root = mainMenu.rootVisualElement;

        // Main menu containers
        title = root.Q<VisualElement>("Title");
        menu = root.Q<VisualElement>("Menu");

        // Main menu buttons
        play = root.Q<Button>("Play");
        options = root.Q<Button>("Options");
        exit = root.Q<Button>("Exit");

        play.clicked += OpenDifficultySelection;

        // Difficulty selection buttons
        difficultySelection = root.Q<VisualElement>("DifficultySelection");
        SpawnDifficultyButtons();

        difficultySelection.style.display = DisplayStyle.None;
    }

    private void OpenDifficultySelection()
    {
        title.style.display = DisplayStyle.None;
        menu.style.display = DisplayStyle.None;

        difficultySelection.style.display = DisplayStyle.Flex;
    }

    private void SpawnDifficultyButtons()
    {
        foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
        {
            Debug.Log("Spawning button for " + diff);
            DifficultyButton buttonElement = new DifficultyButton();
            buttonElement.Difficulty = diff;
            buttonElement.name = diff.ToString();
            buttonElement.AddToClassList("mainButton");
            buttonElement.Button.text = diff.ToString();
            difficultySelection.Add(buttonElement);

            buttonElement.Button.clicked += () => activateManagerLoader(loader.DifficultyProperties.Where(d => d.difficulty.Equals(buttonElement.Difficulty)).First());
        }
    }

    private void activateManagerLoader(DifficultyProperties _properties)
    {
        ManagerLoader.LoadManagers(_properties);
    }
}
