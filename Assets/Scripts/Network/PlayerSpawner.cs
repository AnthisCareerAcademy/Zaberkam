using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] GameObject desktopPlayer;
    [SerializeField] Vector3 pcSpawn;
    [SerializeField] GameObject VrPlayer;
    [SerializeField] Vector3 vrSpawn;
    private GameObject host;
    private GameObject client;

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
        client = Instantiate(desktopPlayer, pcSpawn, Quaternion.identity);
        client.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        client = GameObject.Find("PC Player(Clone)");
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVrServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        host = Instantiate(VrPlayer, vrSpawn, Quaternion.identity);
        host.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        host = GameObject.Find("Complete VR Setup(Clone)");
    }

    public void DestroyHost()
    {
        if (host != null )
        {
            Destroy(host);
        }
    }

    public void DestoryClient()
    {
        if (client != null)
        {
            Destroy(client);
        }
    }
}
