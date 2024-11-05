using Unity.Netcode.Components;
using UnityEngine;

public class ClientAuthoritativeNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
