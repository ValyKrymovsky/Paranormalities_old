using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MyCode.GameData.GameSave;

namespace MyCode.UI.MainMenu
{
    public class GameSaveHandler : MonoBehaviour
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
        [Space]

        [Header("Elements - MainMenu")]
        [Space]
        private GameSave _selectedSave;

        private ManagerLoader _loader;
    }
}
