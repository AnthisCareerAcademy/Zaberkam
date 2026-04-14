using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject joinServerMenu;

    [Header("Join Server")]
    public TMP_InputField codeInput;

    // MAIN MENU
    public void OnPlayPressed()
    {
        mainMenu.SetActive(false);
        joinServerMenu.SetActive(true);
    }

    public void OnSettingsPressed()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OnMeetCharactersPressed()
    {
        Debug.Log("Meet Characters pressed.");
        //Later; load 3D character showcase scene or animate a panel?? (TBD soon)
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    //Settings!!
    public void OnBackFromSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Server Join (which theoretically we'll add Julians code where the TODO is at
    public void OnJoinPressed()
    {
        string code = codeInput.text;
        Debug.Log("Attempting to join server with code: " + code);

        //TODO: add the networking code that allows the pc user(s) to join the vr
    }

    public void OnBackFromJoin()
    {
        joinServerMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}