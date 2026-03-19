using Unity.Netcode;
using UnityEngine;

public class NetcodeCameraCheck : NetworkBehaviour
{
    [SerializeField] Camera myCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            myCamera.enabled = false;
        }
    }
}
