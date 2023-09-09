using UnityEngine;
using Cysharp.Threading.Tasks;
using MyCode.GameData.GameSettings;
using MyCode.GameData.GameSave;
using MyCode.Managers;

public class Manager<T> where T : class
{
    public virtual async UniTask SetUpNewManager(DifficultyProperties _properties) { await UniTask.WaitForSeconds(1); }

    public virtual async UniTask SetUpExistingManager(GameSave _save) { await UniTask.WaitForSeconds(1); }
}
