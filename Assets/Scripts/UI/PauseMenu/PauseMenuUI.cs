using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using MyBox;

public class PauseMenuUI : MonoBehaviour
{
    UIDocument document;

    VisualElement root;

    private Button resume;
    private Button exit;

    private bool paused;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        resume = root.Q<Button>("Resume");
        exit = root.Q<Button>("Exit");

        resume.clicked += () => ResumeGame();
        exit.clicked += () => ExitGame();

        root.style.display = DisplayStyle.None;
        paused = false;
    }

    /// <summary>
    /// Pauses the game if not already paused. Sets Time.timeScale to 0. Is handled by InputAction.
    /// </summary>
    /// <param name="_context"></param>
    public void PauseGame(InputAction.CallbackContext _context)
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            root.style.display = DisplayStyle.Flex;
            paused = true;
        }
        
    }

    /// <summary>
    /// Resumes the game if not already resumed. Sets Time.timeScale to 1. Is handled by InputAction.
    /// </summary>
    /// <param name="_context"></param>
    public void ResumeGame(InputAction.CallbackContext _context)
    {
        if (paused)
        {
            Time.timeScale = 1;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            root.style.display = DisplayStyle.None;
            paused = false;
        }
        
    }

    /// <summary>
    /// Resumes the game if not already resumes. Sets Time.timeScale to 1.
    /// </summary>
    private void ResumeGame()
    {
        if (paused)
        {
            Time.timeScale = 1;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            root.style.display = DisplayStyle.None;
            paused = false;
        }
    }

    /// <summary>
    /// Loads main menu scene.
    /// </summary>
    private void ExitGame()
    {
        Time.timeScale = 1.0f;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
