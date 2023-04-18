using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using MyBox;

public class PauseMenuUI : MonoBehaviour
{
    UIDocument document;

    VisualElement root;

    VisualElement pauseMenu;

    private Button resume;
    private Button exit;

    private bool paused;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        pauseMenu = root.Q<VisualElement>("Screen");

        resume = root.Q<Button>("Resume");
        exit = root.Q<Button>("Exit");

        resume.clicked += () => ResumeGame();
        exit.clicked += () => ExitGame();

        root.style.display = DisplayStyle.None;
        paused = false;
    }

    public void PauseGame(InputAction.CallbackContext _context)
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            pauseMenu.style.display = DisplayStyle.Flex;
            root.style.display = DisplayStyle.Flex;
            paused = true;
        }
        
    }

    public void ResumeGame(InputAction.CallbackContext _context)
    {
        if (paused)
        {
            Time.timeScale = 1;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.style.display = DisplayStyle.None;
            root.style.display = DisplayStyle.None;
            paused = false;
        }
        
    }

    private void ResumeGame()
    {
        if (paused)
        {
            Time.timeScale = 1;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            pauseMenu.style.display = DisplayStyle.None;
            root.style.display = DisplayStyle.None;
            paused = false;
        }
    }

    private void ExitGame()
    {
        Time.timeScale = 1.0f;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
