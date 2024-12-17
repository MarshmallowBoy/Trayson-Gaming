using Newtonsoft.Json.Bson;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleMainCamera : MonoBehaviour
{
    public GameObject Console;
    private void Start()
    {
        //NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Console.SetActive(!Console.activeInHierarchy);
            Console.GetComponent<Console>().InputField.ActivateInputField();
        }

    }

    private void OnServerStarted()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
