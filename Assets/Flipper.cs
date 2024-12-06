using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Flipper : NetworkBehaviour
{
    public Animator anim;
    public Transform cam;
    public NetworkObject NetworkObjectMain;
    public int Damage;
    float NextTimeToFire = 0;
    public float Delay;
    public float Knockback = 0;
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "FlipperIdle" && NextTimeToFire < Time.time)
        {
            NextTimeToFire = Time.time + Delay;
            anim.SetTrigger("Slap");
            Invoke("Fire", 0.4f);
        }
    }

    void Fire()
    {
        foreach(var _hit in Physics.RaycastAll(cam.position, cam.forward, 3))
        {
            if (_hit.transform.CompareTag("Player"))
            {
                if (_hit.transform.GetComponent<NetworkObject>().OwnerClientId != NetworkObjectMain.OwnerClientId)
                {
                    Debug.Log("PlayerID: " + _hit.transform.GetComponent<NetworkObject>().OwnerClientId);
                    _hit.transform.GetComponent<Heath>().health -= Damage;
                    //_hit.transform.GetComponent<SC_FPSController>().ExternalVector = (transform.position - _hit.transform.position).normalized * Knockback;
                    SendExternalVectorRpc(_hit.transform.GetComponent<NetworkObject>().OwnerClientId, (transform.position - _hit.transform.position).normalized * Knockback);
                }
            }
            Debug.Log(_hit.transform.name);
        }
    }
    [Rpc(SendTo.Server)]
    void SendExternalVectorRpc(ulong ID, Vector3 ExternalVector)
    {
        ReceiveExternalVectorRpc(ID, ExternalVector);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ReceiveExternalVectorRpc(ulong ID, Vector3 ExternalVector)
    {
        NetworkManager.Singleton.ConnectedClients[ID].PlayerObject.GetComponent<SC_FPSController>().ExternalVector = ExternalVector;
        Debug.Log(NetworkManager.Singleton.ConnectedClients[ID].PlayerObject.name);
    }
}
