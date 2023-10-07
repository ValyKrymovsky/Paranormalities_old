using System.Collections;
using System.Collections.Generic;
using MyCode.Managers;
using MyCode.GameData;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyCode.UI
{
    public class GameplayUI : MonoBehaviour
    {
        public UIDocument gameplayUI;
        public VisualElement _root;
        public VisualElement _objectivePanel;
        public Label _superObjective;
        public Label _subObjective;

        private void Awake()
        {
            gameplayUI = GetComponent<UIDocument>();
            _root = gameplayUI.rootVisualElement;

            _objectivePanel = _root.Q<VisualElement>("ObjectiveScreen");
            _superObjective = _objectivePanel.Q<Label>("SuperObj");
            _subObjective = _objectivePanel.Q<Label>("SubObj");
        }

        private void OnEnable()
        {
            ObjectiveManager.Instance.OnSuperObjectiveChange += SetNewSuper;
            ObjectiveManager.Instance.OnSubObjectiveChange += SetNewSub;
        }

        private void OnDisable()
        {
            ObjectiveManager.Instance.OnSuperObjectiveChange -= SetNewSuper;
            ObjectiveManager.Instance.OnSubObjectiveChange -= SetNewSub;
        }

        private void SetNewSuper(SuperObjective objective)
        {
            _superObjective.text = objective.name;
            Debug.Log("GameplayUI : set new super");
        }

        private void SetNewSub(SubObjective objective)
        {
            _subObjective.text = objective.name;
            Debug.Log("GameplayUI : set new sub");
        }
    }
}
