using Unity.VisualScripting;
using UnityEngine;

public class MainMenuCommands : MonoBehaviour
{
    public void QuitAppleCashus()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
