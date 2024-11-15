using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    // References to buttons in the main menu
    public Canvas canvas;
    public Canvas playCanvas;
    public Canvas pauseCanvas;
    public Button playButton;
    public Button backButton;
    public Button back2Button;
    public Button optionsButton;
    public Button hostButton;
    public Button clientButton;
    public Button quitButton;
    public GameObject optionsPanel;  // If you have an options menu as a panel

    // Initialization
    private void Start()
    {
        // Assigning button click listeners
        playButton.onClick.AddListener(OnPlayButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        back2Button.onClick.AddListener(OnBackButtonClicked);
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        if(playCanvas != null)
        {
            playCanvas.enabled = false;
        }

        if(pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
        }
        // If you have an options panel, make sure it's inactive at the start
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseCanvas.enabled = !pauseCanvas.enabled;
            Cursor.lockState = CursorLockMode.Locked;
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            Cursor.visible = !Cursor.visible;
        }
    }

    // Start the game (Load the main game scene)
    void OnPlayButtonClicked()
    {
        // Load the scene for the game, e.g., "GameScene" (replace with your actual game scene name)
        //SceneManager.LoadScene("GameScene");
        canvas.enabled = false;
        playCanvas.enabled = true;
    }

    void OnBackButtonClicked()
    {
        canvas.enabled = true;
        playCanvas.enabled = false;
    }
    void OnHostButtonClicked()
    {
        playCanvas.enabled = false;
    }

    void OnClientButtonClicked()
    {
        playCanvas.enabled = false;
    }

    // Show or hide the options menu
    void OnOptionsButtonClicked()
    {
        // If you have an options panel, toggle its visibility
        if (optionsPanel != null)
        {
            bool isActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(!isActive);
        }
    }

    // Quit the game
    void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        // If running in Unity editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a standalone build, quit the application
        Application.Quit();
#endif
    }
}
