using Unity.Netcode;
using UnityEngine;

public class NetcodeCameraCheck : NetworkBehaviour
{
    [SerializeField] Camera myCamera;
    [SerializeField] Canvas myUI;

    public override void OnNetworkSpawn()
    {
        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (!IsOwner)
        {
            myCamera.enabled = false;
            myUI.enabled = false;
            if (listener != null)
            {
                listener.enabled = false;
            }
        }
    }
}
