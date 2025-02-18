using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
public class UIController : NetworkBehaviour
{
    public GameObject Console;
    public GameObject Preview;
    public bool MenuEnabled = false;
    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Console.SetActive(!Console.activeInHierarchy);
                Console.GetComponent<Console>().InputField.ActivateInputField();
                MenuEnabled = !Console.activeInHierarchy;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Preview.SetActive(!Preview.activeInHierarchy);
            }
        }
    }
}
