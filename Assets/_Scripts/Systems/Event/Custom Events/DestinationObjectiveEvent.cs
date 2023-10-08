using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.GameData;
using MyCode.Managers;
using System.Linq;

namespace MyCode.Systems
{
    public class DestinationObjectiveEvent : EventSystem
    {
        [SerializeField] private SubObjective _objective;

        private void OnEnable()
        {
            OnEventStart += CompleteObjective;
        }

        private void OnDisable()
        {
            OnEventStart -= CompleteObjective;
        }

        private void CompleteObjective()
        {
            ObjectiveCompletionType completionType = ObjectiveManager.Instance.CurrentSuperObjective.completionType;

            if (completionType == ObjectiveCompletionType.Indexed || completionType == ObjectiveCompletionType.Random)
            {
                if (ObjectiveManager.Instance.CurrentSubObjective != _objective || _objective.isCompleted) return;

                _objective.InvokeOnCompleted();
                ObjectiveManager.Instance.NextSubObjective();
                return;
            }
            else if(completionType == ObjectiveCompletionType.Concurrent)
            {
                if (!ObjectiveManager.Instance.CurrentSuperObjective.subObjectives.Contains(_objective)) return;

                _objective.InvokeOnCompleted();

                ObjectiveManager.Instance.PerformObjectiveCheck();
            }
        }
    }
}
