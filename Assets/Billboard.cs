using Unity.Netcode;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera Camera;
    public NetworkObject networkObject;
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += InitializeCameraSelection;
        Camera = Camera.main;
    }

    public void InitializeCameraSelection(ulong PlayerID)
    {
        Camera = Camera.main;
    }

    void Update()
    {
        if (Camera != null)
        {
            transform.LookAt(Camera.transform.position);
        }
    }
}
