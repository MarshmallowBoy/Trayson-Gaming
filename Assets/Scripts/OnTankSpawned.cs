using UnityEngine;
using Unity.Netcode;
public class OnTankSpawned : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        if (PlayerPrefs.GetString("ActiveVehicle") == string.Empty)
        {
            PlayerPrefs.SetString("ActiveVehicle", "none");
        }
        switch (PlayerPrefs.GetString("ActiveVehicle"))
        {
            case "none":
                //DestroyTankServerRpc();
                Destroy(gameObject);
                break;
            case "tank":
                NetworkObject.GetComponent<SC_FPSController>().HealthBar.SetActive(false);
                NetworkObject.GetComponent<SC_FPSController>().enabled = false;
                NetworkObject.GetComponent<SC_FPSController>().playerCamera.gameObject.SetActive(false);
                NetworkObject.GetComponent<CapsuleCollider>().enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                transform.position = Vector3.down * 5;
                break;
        }
    }

    [Rpc(SendTo.Server)]
    public void DestroyTankServerRpc()
    {
        DestroyTankRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DestroyTankRpc()
    {
        Destroy(gameObject);
    }

}
