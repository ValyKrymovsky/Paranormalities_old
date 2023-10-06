using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyCode.Systems
{
    public class DestinationObjectiveEvent : EventSystem
    {
        [SerializeField] private Objective _objective;

        private void OnEnable()
        {
            OnEventStart += () => _objective.InvokeOnCompleted();
        }

        private void OnDisable()
        {
            OnEventStart -= () => _objective.InvokeOnCompleted();
        }
    }
}
