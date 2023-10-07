using MyCode.GameData;
using MyCode.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyCode.Managers
{
    public class ObjectiveManager
    {
        private static readonly object _lock = new object();
        private static ObjectiveManager _instance;
        public static ObjectiveManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ObjectiveManager();
                }
                return _instance;
            }
        }

        public List<SuperObjective> Objectives { get; set; }
        public SuperObjective CurrentSuperObjective { get; set; }
        public SubObjective CurrentSubObjective { get; set; }

        public event Action<SuperObjective> OnSuperObjectiveChange;
        public event Action<SubObjective> OnSubObjectiveChange;

        public void InvokeOnSuperObjectiveChange(SuperObjective objective)
        {
            OnSuperObjectiveChange?.Invoke(objective);
        }

        public void InvokeOnSubObjectiveChange(SubObjective objective)
        {
            OnSubObjectiveChange?.Invoke(objective);
        }

        public void NextSubObjective()
        {
            if (CurrentSubObjective.superObjective.completionType == ObjectiveCompletionType.Random)
            {
                int currentIndex = CurrentSuperObjective.subObjectives.ToList().IndexOf(CurrentSubObjective);
                if (currentIndex >= CurrentSuperObjective.subObjectives.Count() - 1)
                {
                    Debug.Log("Called for next super objective");
                    NextSuperObjective();
                    return;
                }

                CurrentSubObjective.InvokeOnCompleted();

                CurrentSubObjective = CurrentSuperObjective.subObjectives[currentIndex + 1];
                OnSubObjectiveChange?.Invoke(CurrentSubObjective);
                return;
            }

            int nextSubId = CurrentSubObjective.id + 1;
            if (nextSubId > CurrentSuperObjective.subObjectives.Count() - 1)
            {
                NextSuperObjective();
                return;
            }

            CurrentSubObjective = CurrentSuperObjective.subObjectives[nextSubId];
            OnSubObjectiveChange?.Invoke(CurrentSubObjective);
        }

        public void NextSuperObjective()
        {
            int nextSuperId = CurrentSuperObjective.id + 1;
            if (nextSuperId > Objectives.Count() - 1)
            {
                Debug.Log("There are no more super objectives!");
                return;
            }

            CurrentSuperObjective?.InvokeOnCompleted();

            CurrentSuperObjective = Objectives[nextSuperId];
            if (CurrentSuperObjective.completionType == ObjectiveCompletionType.Random) CurrentSuperObjective.RandomizeObjectives();
            CurrentSubObjective = CurrentSuperObjective.subObjectives[0];
            OnSuperObjectiveChange?.Invoke(CurrentSuperObjective);
            OnSubObjectiveChange?.Invoke(CurrentSubObjective);
        }

        public void PerformObjectiveCheck()
        {
            bool allCompleted = true;
            for (int i = 0; i < CurrentSuperObjective.subObjectives.Count() - 1; i++)
            {
                if (!CurrentSuperObjective.subObjectives[i].isCompleted)
                {
                    allCompleted = false;
                    break;
                }
            }

            if (allCompleted) NextSuperObjective();
        }


    }
}

