using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] GameObject desktopPlayer;
    [SerializeField] Vector3 pcSpawn;
    [SerializeField] GameObject VrPlayer;
    [SerializeField] Vector3 vrSpawn;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnVrServerRpc();
        }
        else
        {
            SpawnPcServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnPcServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        GameObject player = Instantiate(desktopPlayer, pcSpawn, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVrServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        GameObject player = Instantiate(VrPlayer, vrSpawn, Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
