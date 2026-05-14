using Unity.Netcode;
using UnityEngine;


public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] GameObject desktopPlayer;
    [SerializeField] Vector3 pcSpawn;
    [SerializeField] GameObject hostPlayer;
    [SerializeField] Vector3 hostSpawn;
    private GameObject host;
    private GameObject client;

    private void Awake()
    {
        host = GameObject.Find("PC Overseer(Clone)");
    }

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
        pcSpawn = GameObject.FindWithTag("Respawn").transform.position;
        client = Instantiate(desktopPlayer, pcSpawn, Quaternion.identity);
        client.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        client = GameObject.Find("RangerClass(Clone)");
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVrServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        host = Instantiate(hostPlayer, hostSpawn, Quaternion.identity);
        host.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        host = GameObject.Find("PC Overseer(Clone)");
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
