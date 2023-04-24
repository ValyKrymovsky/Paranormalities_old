using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using MyBox;

public class EndGame
{
    private string mainMenu = "";
    private UIDocument deathScreen;
    private VisualElement root;

    public EndGame(string _mainMenuSceneName)
    {
        mainMenu = _mainMenuSceneName;
        deathScreen = GameObject.Find("DeathScreen").GetComponent<UIDocument>();
        root = deathScreen.rootVisualElement;
        root.style.display = DisplayStyle.None;
    }

    public void LoadMainMenu()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
    }

    public IEnumerator StartDeathScreen()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene(mainMenu, LoadSceneMode.Single);
    }
    
}
