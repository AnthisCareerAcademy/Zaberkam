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

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));

        startButton.onClick.AddListener(SwitchScene);
        
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

            UpdateHostStatus();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
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

