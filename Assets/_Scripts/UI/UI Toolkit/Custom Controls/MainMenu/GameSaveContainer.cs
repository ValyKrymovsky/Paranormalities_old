using MyCode.GameData.GameSave;
using UnityEngine.UIElements;
using MyCode.Managers;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine;

public class GameSaveContainer : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<GameSaveContainer, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription gameSavePath =
            new UxmlStringAttributeDescription { name = "Game Save Path" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as GameSaveContainer;

            ate.GameSavePath = gameSavePath.GetValueFromBag(bag, cc);
        }
    }
    public string GameSavePath { get; set; }

    private GameSave _gameSave;

    private VisualElement _container;
    private VisualElement _saveImage;

    private VisualElement _propertiesContainer;

    private Label _saveName;
    private Label _saveDate;

    private VisualElement _buttonContainer;

    private Button _loadButton;
    private Button _deleteButton;

    public event Action<GameSave> OnLoadSave;
    public event Action<GameSaveContainer> OnDeleteSave;

    public GameSave GameSave { get => _gameSave; set => _gameSave = value; }
    public VisualElement Container { get => _container; private set => _container = value; }
    public VisualElement SaveImage { get => _saveImage; set => _saveImage = value; }
    public VisualElement PropertiesContainer { get => _propertiesContainer; private set => _propertiesContainer = value; }
    public Label SaveName { get => _saveName; set => _saveName = value; }
    public Label SaveDate { get => _saveDate; set => _saveDate = value; }
    public VisualElement ButtonContainer { get => _buttonContainer; private set => _buttonContainer = value; }
    public Button LoadButton { get => _loadButton; private set => _loadButton = value; }
    public Button DeleteButton { get => _deleteButton; private set => _deleteButton = value; }

    public GameSaveContainer()
    {
        Container = new VisualElement();
        Container.name = "GameSaveContainer";
        Container.AddToClassList("saveContainer");

        SaveImage = new VisualElement();
        SaveImage.name = "GameSaveImage";
        SaveImage.AddToClassList("saveImage");

        PropertiesContainer = new VisualElement();
        PropertiesContainer.name = "PropertiesContainer";
        PropertiesContainer.AddToClassList("saveProperties");

        SaveName = new Label();
        SaveName.AddToClassList("propertyLabel");
        SaveDate = new Label();
        SaveDate.AddToClassList("propertyLabel");

        ButtonContainer = new VisualElement();
        ButtonContainer.name = "ButtonContainer";
        ButtonContainer.AddToClassList("buttonContainer");

        LoadButton = new Button();
        LoadButton.text = "Load";
        LoadButton.AddToClassList("button");
        DeleteButton = new Button();
        DeleteButton.text = "Delete";
        DeleteButton.AddToClassList("button");

        hierarchy.Add(Container);
        Container.Add(SaveImage);

        Container.Add(PropertiesContainer);

        PropertiesContainer.Add(SaveName);
        PropertiesContainer.Add(SaveDate);

        PropertiesContainer.Add(ButtonContainer);

        ButtonContainer.Add(LoadButton);
        ButtonContainer.Add(DeleteButton);

        _loadButton.clicked += () =>
        {
            OnLoadSave?.Invoke(_gameSave);
        };

        _deleteButton.clicked += () =>
        {
            File.Delete(GameSavePath);

            OnDeleteSave?.Invoke(this);
        };
    }




}
