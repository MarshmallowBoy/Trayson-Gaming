using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Unity.Netcode;
public class MainMenuCommands : MonoBehaviour
{
    public GameObject MapSelection;
    public void QuitAppleCashus()
    {
        Application.Quit();
    }

    public void ToggleSelectMapMenu()
    {
        MapSelection.SetActive(!MapSelection.activeInHierarchy);
    }

    public void LoadMap(string name)
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
