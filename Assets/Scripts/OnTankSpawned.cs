using UnityEngine;
using Unity.Netcode;
public class OnTankSpawned : NetworkBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.GetString("ActiveVehicle") == "None")
        {
            return;
        }
        switch (PlayerPrefs.GetString("ActiveVehicle"))
        {
            case "none":
                Destroy(gameObject);
                break;
            case "tank":
                NetworkObject.GetComponent<SC_FPSController>().enabled = false;
                NetworkObject.GetComponent<SC_FPSController>().playerCamera.gameObject.SetActive(false);
                NetworkObject.GetComponent<CapsuleCollider>().enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                transform.position = Vector3.down * 5;
                break;
        }
        
    }
}
