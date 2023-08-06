using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using MyCode.GameData.GameSave;
using MyCode.Managers;

[RequireComponent(typeof(ManagerType))]
public class Manager<T> : MonoBehaviour where T : class
{
    internal static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindObjectOfType(typeof(T)) as T;

            if (_instance != null) return _instance;

            Addressables.InstantiateAsync(typeof(T).Name).Completed += handle =>
            {
                if (!handle.Status.Equals(AsyncOperationStatus.Succeeded)) return;

                _instance = ((GameObject)handle.Result).GetComponent<T>();
                DontDestroyOnLoad((GameObject)handle.Result);
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
