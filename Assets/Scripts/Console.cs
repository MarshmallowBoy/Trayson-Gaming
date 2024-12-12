using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public TextMeshProUGUI Output;
    public ScrollRect ScrollRect;
    public TMP_InputField InputField;
    public void PrintMessageToConsole(string Message)
    {
        InputField.text = string.Empty;
        Output.text += Message + "\n";
        Invoke("ResetScrollRect", 0.1f);
    }

    void ResetScrollRect()
    {
        ScrollRect.verticalNormalizedPosition = 0;
    }
}
