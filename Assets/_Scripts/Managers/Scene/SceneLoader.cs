using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    public static async UniTask<Scene> LoadScene(Scene _scene)
    {
        await SceneManager.LoadSceneAsync(_scene.ToString(), LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scene.ToString()));

        return _scene;
    }
}
