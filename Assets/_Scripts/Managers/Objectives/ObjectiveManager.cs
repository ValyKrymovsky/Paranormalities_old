using MyCode.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private SuperObjective _currentSuperObjective;
    private SubObjective _currentSubObjective;

    public event Action<SuperObjective> OnSuperObjectiveChange;
    public event Action<SubObjective> OnSubObjectiveChange;

    public SuperObjective CurrentSuperObjective
    {
        get
        {
            return _currentSuperObjective;
        }
        set
        {
            _currentSuperObjective = value;
            Debug.Log("New super objective set");
            OnSuperObjectiveChange?.Invoke(_currentSuperObjective);
        }
    }

    public SubObjective CurrentSubObjective
    {
        get
        {
            return _currentSubObjective;
        }
        set
        {
            _currentSubObjective = value;
            Debug.Log("New sub objective set");
            OnSubObjectiveChange?.Invoke(_currentSubObjective);
            _currentSubObjective.OnCompleted += () =>
            {
                int nextSubId = _currentSubObjective.id + 1;
                if (nextSubId > _currentSuperObjective.subObjectives.Count() - 1)
                {
                    Debug.Log("There is no next sub objective");
                    int nextSuperId = _currentSuperObjective.id + 1;
                    if (nextSuperId > Objectives.Count() - 1)
                        Debug.Log("There is no next super objective");
                    else
                    {
                        CurrentSuperObjective = Objectives[_currentSuperObjective.id + 1];
                        CurrentSubObjective = _currentSuperObjective.subObjectives[0];
                    }
                    
                }
                else
                    CurrentSubObjective = _currentSuperObjective.subObjectives[nextSubId];
            };


            
            
        }
    }
}
