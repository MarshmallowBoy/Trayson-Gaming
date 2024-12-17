using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCommands : MonoBehaviour
{
    public GameObject MapSelection;
    public void QuitAppleCashus()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ToggleSelectMapMenu()
    {
        MapSelection.SetActive(!MapSelection.activeInHierarchy);
    }
}
