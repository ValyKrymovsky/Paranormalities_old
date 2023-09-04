using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using MyCode.GameData.GameSave;
using MyCode.Managers;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(ManagerType))]
public class Manager<T> : MonoBehaviour where T : class
{
    internal static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            Debug.LogWarning("Instance not found");

            _instance = FindObjectOfType(typeof(T)) as T;

            if (_instance != null) return _instance;

            Debug.LogWarning("Instance not found x2");

            ManagerLoader managerLoader = (ManagerLoader)FindObjectOfType(typeof(ManagerLoader));

            Addressables.LoadAssetsAsync<GameObject>(managerLoader.ManagerGroupLabel, null).Completed += handle =>
            {
                if (!handle.Status.Equals(AsyncOperationStatus.Succeeded)) return;

                GameObject managerObject = handle.Result.Where(m => m.name == typeof(T).Name).First();
                T manager = managerObject.GetComponent<T>();

                Debug.Log("Successfully loaded " +  managerObject.name);

                _instance = manager;
                DontDestroyOnLoad(managerObject);
            };
            

            return _instance;
        }
    }

    public virtual async UniTask SetUpNewManager(DifficultyProperties _properties) { await UniTask.WaitForSeconds(1); }

    public virtual async UniTask SetUpExistingManager(GameSave _save) { await UniTask.WaitForSeconds(1); }

    public static T CreateManager(GameObject _manager)
    {
        if (_instance != null) return _instance;

        _instance = GameObject.FindObjectOfType(typeof(T)) as T;

        if (_instance != null) return _instance;

        var manager = Instantiate(_manager);

        _instance = manager.GetComponent<T>();
        DontDestroyOnLoad(manager);

        return _instance;
    }
}
