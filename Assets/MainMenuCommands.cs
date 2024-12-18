using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class MainMenuCommands : MonoBehaviour
{
    public GameObject MapSelection;
    public void QuitAppleCashus()
    {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void ToggleSelectMapMenu()
    {
        MapSelection.SetActive(!MapSelection.activeInHierarchy);
    }

    public void LoadMap(int index)
    {
        SceneManager.LoadScene(index);
    }
}
