using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TND.XeSS;
public class Console : MonoBehaviour
{
    public TextMeshProUGUI Output;
    public ScrollRect ScrollRect;
    public TMP_InputField InputField;
    public string[] CommandStrings;
    public string[] CommandInputParameter;
    public UnityEvent[] CommandOutputParameter;
    public ConsoleCommands ConsoleCommands;

    public void HelpCommand()
    {
        foreach (string command in CommandStrings)
        {
            PrintMessageToConsole(command);
        }
    }
    public void Error()
    {
        PrintMessageToConsole("Error: Invalid command, use 'cmd' for a list of commands.");
    }

    public void PrintMessageToConsole(string Message)
    {
        if (Message == "`" || Message == string.Empty)
        {
            return;
        }
        InputField.text = string.Empty;
        Output.text += Message + "\n";
        Invoke("ResetScrollRect", 0.1f);
    }

    void ResetScrollRect()
    {
        ScrollRect.verticalNormalizedPosition = 0;
        InputField.ActivateInputField();
    }

    public int FindCommandIndexParameter(string Command)
    {
        for (int i = 0; i < CommandInputParameter.Length; i++)
        {
            if (CommandInputParameter[i].ToLower() == Command.ToLower())
            {
                return i;
            }
        }
        return 0;
    }

    public void SendCommand(string Command)
    {
        if(Command == "`" || Command == string.Empty)
        {
            return;
        }
        if (Command.Contains(' '))
        {
            ConsoleCommands.CurrentParameter = Command.Split(' ')[1];
        }
        CommandOutputParameter[FindCommandIndexParameter(Command.Split(' ')[0])].Invoke();
        ConsoleCommands.CurrentParameter = string.Empty;
    }
}
