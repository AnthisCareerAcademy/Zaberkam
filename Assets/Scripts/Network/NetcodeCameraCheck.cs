using Unity.Netcode;
using UnityEngine;

public class NetcodeCameraCheck : NetworkBehaviour
{
    [SerializeField] Camera myCamera;

    public override void OnNetworkSpawn()
    {
        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (!IsOwner)
        {
            myCamera.enabled = false;
            if (listener != null)
            {
                listener.enabled = false;
            }
        }
    }
}
