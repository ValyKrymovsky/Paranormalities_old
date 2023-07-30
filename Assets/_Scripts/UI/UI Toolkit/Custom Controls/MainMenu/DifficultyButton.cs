using UnityEngine.UIElements;
using MyCode.GameData.GameSettings;

namespace MyCode.UI.MainMenu
{
    public class DifficultyButton : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<DifficultyButton, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<Difficulty> difficulty =
                new UxmlEnumAttributeDescription<Difficulty> { name = "difficulty" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as DifficultyButton;

                ate.Difficulty = difficulty.GetValueFromBag(bag, cc);
            }
        }
        public Difficulty Difficulty { get; set; }


        private Button _button;
        public Button Button { get => _button; private set => _button = value; }

        public DifficultyButton()
        {
            Button = new Button();
            Button.AddToClassList("mainButton");
            hierarchy.Add(Button);
        }


    }

}
