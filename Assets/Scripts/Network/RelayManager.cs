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
    public TMP_InputField joinCodeInput;
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;
    [SerializeField] TextMeshProUGUI hostText;
    [SerializeField] GameObject menuUI;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("RelayManager Awake() is running");

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
        
        Debug.Log("Join Button listerner added ");
        UpdateHostStatus();
        
        DontDestroyOnLoad(gameObject);
    }

    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            codeText.text = "Code: " + joinCode;
            codeText.gameObject.SetActive(true);

            var relayServerData = allocation.ToRelayServerData("udp");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            menuUI?.SetActive(false);

            UpdateHostStatus();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    async void JoinRelay(string JoinCode)
    {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
            
            var relayServerData = joinAllocation.ToRelayServerData("udp");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
            
            menuUI?.SetActive(false);

            UpdateHostStatus();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void JoinRelayButton()
    {
        JoinRelay(joinCodeInput.text);
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
}

