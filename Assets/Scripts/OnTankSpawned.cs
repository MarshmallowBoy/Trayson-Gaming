using UnityEngine;
using Unity.Netcode;
public class OnTankSpawned : NetworkBehaviour
{
    private void Start()
    {
        NetworkObject.GetComponent<SC_FPSController>().enabled = false;
        NetworkObject.GetComponent<SC_FPSController>().playerCamera.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
