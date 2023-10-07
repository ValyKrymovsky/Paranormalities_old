using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    [Serializable]
    public class Objective : ScriptableObject
    {
        public int id;
        public new string name;

        public bool isCompleted;

        public event Action OnCompleted;
        public event Action OnFailed;

        public void InvokeOnCompleted() => OnCompleted?.Invoke();
        public void InvokeOnFailed() => OnFailed?.Invoke();

        private void OnEnable()
        {
            OnCompleted += () => isCompleted = true;
            OnFailed += () => isCompleted = false;
        }

        private void OnDisable()
        {
            OnCompleted -= () => isCompleted = true;
            OnFailed -= () => isCompleted = false;
        }
    }
}


