using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DirectInteractorUIActivator : XRDirectInteractor
{
    [SerializeField] GameObject wristUI;
    
    public void DisplayWristUI()
    {
        wristUI.SetActive(true);
    }

    public void HideWristUI()
    {
        wristUI.SetActive(false);
    }
}
