using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace MyCode.Managers
{
    public enum MyScene
    {
        MainMenu,
        DebugScene,
        Game
    }

    public class SceneLoader
    {
        public static async UniTask<MyScene> LoadScene(MyScene _scene)
        {
            await SceneManager.LoadSceneAsync(_scene.ToString(), LoadSceneMode.Single);

            return _scene;
        }

        public static void SetActiveScene(MyScene _scene)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scene.ToString()));

        }
    }
}
