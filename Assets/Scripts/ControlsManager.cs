using TMPro;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    [Header("Default Values")]
    [SerializeField] int stickControlsDefault;
    [SerializeField] int turnControlsDefault;
    
    [Header("Control References")]
    [SerializeField] GameObject moveAndStrafe;
    [SerializeField] GameObject moveOnly;
    [SerializeField] GameObject snapTurn;
    [SerializeField] GameObject continuousTurn;
    
    [Header("Dropdown References")]
    [SerializeField] TMP_Dropdown stickDropdown;
    [SerializeField] TMP_Dropdown turnDropdown;

    void Start()
    {
        stickDropdown.value = stickControlsDefault;
        turnDropdown.value = turnControlsDefault;
        
        ChangeStickControls();
    }
    
    public void ChangeStickControls()
    {
        switch (stickDropdown.value)
        {
            // First option: move and strafe
            case 0:
                moveAndStrafe.SetActive(true);
                moveOnly.SetActive(false);
                DisableTurn();
                break;
            // Second option: move and turn
            case 1:
                moveAndStrafe.SetActive(false);
                moveOnly.SetActive(true);
                EnableTurn();
                break;
            // Third option: turn only
            case 2:
                moveAndStrafe.SetActive(false);
                moveOnly.SetActive(false);
                EnableTurn();
                break;
            // Fourth option (and default): do nothing
            default:
                moveAndStrafe.SetActive(false);
                moveOnly.SetActive(false);
                DisableTurn();
                break;
        }
    }
        
    public void ChangeTurnControls()
    {
        switch (turnDropdown.value)
        {
            // First option: snap turn
            case 0:
                snapTurn.SetActive(true);
                continuousTurn.SetActive(false);
                break;
            // Second option (and default): continuous turn
            default:
                snapTurn.SetActive(false);
                continuousTurn.SetActive(true);
                break;
        }
    }

    private void DisableTurn()
    {
        // When turn is disabled, turn off the dropdown and controls.
        snapTurn.SetActive(false);
        continuousTurn.SetActive(false);
        turnDropdown.gameObject.SetActive(false);
    }
    
    private void EnableTurn()
    {
        // When turn is enabled, turn on the dropdown and set the controls to the proper value.
        turnDropdown.gameObject.SetActive(true);
        ChangeTurnControls();
    }
}
