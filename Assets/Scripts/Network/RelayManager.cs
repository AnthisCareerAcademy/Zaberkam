using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] Button startButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;
    [SerializeField] TextMeshProUGUI hostText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));

        startButton.onClick.AddListener(SwitchScene);
        
        UpdateHostStatus();
    }

    async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
        
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        
        codeText.text = "Code: " + joinCode;
        codeText.gameObject.SetActive(true);

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        UpdateHostStatus();
    }

    async void JoinRelay(string JoinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
        
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        
        NetworkManager.Singleton.StartClient();

        UpdateHostStatus();
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
    public void PrintMessage()
    {
        Debug.Log("Button was clicked!");
    }
}

