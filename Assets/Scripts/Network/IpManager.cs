using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;


public class IpManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button startButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI hostText;
    [SerializeField] ushort port = 7777;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(() => StartClient(ipInput.text));
        startButton.onClick.AddListener(SwitchScene);

        UpdateHostStatus();
    }

    void StartHost()
    {
        string localIP = GetLocalIPAddress();

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(localIP, port);

        NetworkManager.Singleton.StartHost();

        infoText.text = $"Host IP: {localIP}:{port}";
        infoText.gameObject.SetActive(true);

        UpdateHostStatus();
    }

    void StartClient(string ip)
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("0.0.0.0", port);

        NetworkManager.Singleton.StartClient();

        UpdateHostStatus();
    }

    string GetLocalIPAddress()
    {
        string localIp = "127.0.0.1";

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIp = ip.ToString();
                break;
            }
        }
        return localIp;
    }

    void UpdateHostStatus()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            hostText.text = "Host: True";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            hostText.text = "Host: False";
        }
        else
        {
            hostText.text = "Host: Not Connected";
        }
    }

    void SwitchScene()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {

            NetworkManager.Singleton.SceneManager.LoadScene("Game Scene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("You need to be connected to the network");
        }
    }
}
