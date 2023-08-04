using MyCode.GameData.GameSave;
using MyCode.GameData.GameSettings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    public GameSave gameSave;

    public VisualElement container;
    public VisualElement saveImage;

    public VisualElement propertiesContainer;

    public Label saveName;
    public Label saveDate;

    public VisualElement buttonContainer;

    public Button loadButton;
    public Button deleteButton;

    public GameSaveContainer()
    {
        container = new VisualElement();
        container.name = "GameSaveContainer";
        container.AddToClassList("gameSaveContainer");

        saveImage = new VisualElement();
        saveImage.name = "GameSaveImage";
        saveImage.AddToClassList("gameSaveImage");

        propertiesContainer = new VisualElement();
        propertiesContainer.name = "PropertiesContainer";
        propertiesContainer.AddToClassList("gameSavePropertiesContainer");

        saveName = new Label();
        saveDate = new Label();

        buttonContainer = new VisualElement();
        buttonContainer.name = "ButtonContainer";
        buttonContainer.AddToClassList("gameSaveButtonContainer");

        loadButton = new Button();
        loadButton.text = "Load";
        deleteButton = new Button();
        deleteButton.text = "Delete";

        hierarchy.Add(container);
        container.Add(saveImage);

        container.Add(propertiesContainer);

        propertiesContainer.Add(saveName);
        propertiesContainer.Add(saveDate);

        propertiesContainer.Add(buttonContainer);

        buttonContainer.Add(loadButton);
        buttonContainer.Add(deleteButton);

        Debug.Log("Called constructor");
    }

}
