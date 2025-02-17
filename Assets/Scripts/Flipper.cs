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
                    _hit.transform.GetComponent<Heath>().DoDamage(Damage);
                    SendExternalVectorRpc(_hit.transform.GetComponent<NetworkObject>().OwnerClientId, (_hit.transform.position - (transform.position + (Vector3.down * 1.5f))).normalized * Knockback);
                }
            }
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
    }
}
