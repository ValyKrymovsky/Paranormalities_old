using Cysharp.Threading.Tasks;
using MyCode.Data.GameSave;
using MyCode.Data.Settings;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameSaveManager : Manager<GameSaveManager>
{

    public override async UniTask SetUpManager(DifficultyProperties _properties)
    {
        await CreateNewSave(_properties);
    }

    private async UniTask CreateNewSave(DifficultyProperties _properties)
    {
        CurrentGameSave = ScriptableObject.CreateInstance<GameSave>();

        string[] saveFiles = AssetDatabase.FindAssets(string.Format("GameSave_{0} l:GameSave t:GameSave", _properties.difficulty));
        SavePath = string.Format(SavePath + "GameSave_{0}_{1}.asset", _properties.difficulty, saveFiles.Length + 1);

        AssetDatabase.CreateAsset(CurrentGameSave, SavePath);

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = CurrentGameSave;

        AssetDatabase.SetLabels(CurrentGameSave, new[] {"GameSave"});

        CurrentGameSave.Difficulty = _properties;

        AssetDatabase.SaveAssets();
    }
    
    [field: SerializeField] internal string SavePath { get; set; }
    public GameSave CurrentGameSave { get; private set; }
}
