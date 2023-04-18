using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using MyBox;

public class MainMenuUI : MonoBehaviour
{
    [Separator("Musin", true)]
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip clip;

    VisualElement root;

    private GameSettingsSave saveSettings;

    private Button selectedButton;

    private Button play;
    private Button options;
    private Button exit;

    private Button apply;
    private Button back;

    private Button general;
    private Button sound;

    Slider masterSlider;
    Slider musicSlider;

    private VisualElement mainMenu;
    private VisualElement optionsMenu;

    private VisualElement generalSettings;
    private VisualElement soundSettings;

    private void OnEnable()
    {
        source = GetComponent<AudioSource>();
        saveSettings = new GameSettingsSave();
        root = GetComponent<UIDocument>().rootVisualElement;

        source.clip = clip;
        source.loop = true;
        source.volume = .5f;
        source.Play();

        mainMenu = root.Q<VisualElement>("MainMenu");
        optionsMenu = root.Q<VisualElement>("OptionsMenu");

        generalSettings = root.Q<VisualElement>("GeneralOptions");
        soundSettings = root.Q<VisualElement>("SoundOptions");

        optionsMenu.style.display = DisplayStyle.None;

        play = root.Q<Button>("Play");
        options = root.Q<Button>("Options");
        exit = root.Q<Button>("Exit");

        apply = root.Q<Button>("Apply");
        back = root.Q<Button>("Back");

        general = root.Q<Button>("General");
        sound = root.Q<Button>("Sound");

        masterSlider = root.Q<Slider>("MasterSlider");
        musicSlider = root.Q<Slider>("MusicSlider");

        play.clicked += () => StartGame();
        options.clicked += () => OpenOptions();
        exit.clicked += () => ExitGame();

        general.clicked += () => ChangeOptionsMenu(general);
        sound.clicked += () => ChangeOptionsMenu(sound);

        apply.clicked += () => ApplySettings();
        back.clicked += () => BackToMainMenu();

        masterSlider.value = LoadDataFromFile().masterSoundVolume;
        musicSlider.value = LoadDataFromFile().musicSoundVolume;
    }

    /// <summary>
    /// Loads school scene.
    /// </summary>
    private void StartGame()
    {
        Debug.Log("Game started");
        SceneManager.LoadScene("School", LoadSceneMode.Single);
    }

    /// <summary>
    /// Displays options menu and hides main menu.
    /// </summary>
    private void OpenOptions()
    {
        mainMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
        selectedButton = general;
        soundSettings.style.display = DisplayStyle.None;

        generalSettings.style.display = DisplayStyle.Flex;
    }   

    /// <summary>
    /// Chenges menu based on selected option
    /// </summary>
    /// <param name="_button"></param>
    private void ChangeOptionsMenu(Button _button)
    {
        selectedButton = _button;

        if (selectedButton == general)
        {
            soundSettings.style.display = DisplayStyle.None;
            generalSettings.style.display = DisplayStyle.Flex;
        }
        else if (selectedButton == sound)
        {
            generalSettings.style.display = DisplayStyle.None;
            soundSettings.style.display = DisplayStyle.Flex;
        }
        

    }

    /// <summary>
    /// Closes game.
    /// </summary>
    private void ExitGame()
    {
        Debug.Log("Application closed");
        Application.Quit();
    }

    /// <summary>
    /// Applies settings
    /// </summary>
    private void ApplySettings()
    {
        saveSettings.SetPaths();
        saveSettings.CreatePlayerData(GetMasterVolume(), GetMusicVolume());
        saveSettings.SaveData();
        saveSettings.ApplySettings();
    }

    /// <summary>
    /// Hides option menu and displays main menu.
    /// </summary>
    private void BackToMainMenu()
    {
        optionsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Returns Mixer master volume value.
    /// </summary>
    /// <returns></returns>
    private float GetMasterVolume()
    {
        return masterSlider.value;
    }

    /// <summary>
    /// Returns Mixer music volume value.
    /// </summary>
    /// <returns></returns>
    private float GetMusicVolume()
    {
        return musicSlider.value;
    }

    /// <summary>
    /// Returns GameSettings object from json file.
    /// </summary>
    /// <returns></returns>
    private GameSettings LoadDataFromFile()
    {
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";
        string persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "GameSettings.json";

        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        GameSettings data = JsonUtility.FromJson<GameSettings>(json);
        reader.Close();

        return data;
    }
}
