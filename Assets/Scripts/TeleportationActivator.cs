using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportationActivator : MonoBehaviour
{
    [SerializeField] XRRayInteractor teleportInteractor;
    [SerializeField] XRRayInteractor rayInteractor;
    [SerializeField] InputActionProperty teleportAction;
    
    void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
        teleportAction.action.performed += ActionPerformed;
        rayInteractor.uiHoverEntered.AddListener(x => DisableTeleport());
    }

    void Update()
    {
        if (teleportAction.action.WasReleasedThisFrame())
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }

    void ActionPerformed(InputAction.CallbackContext obj)
    {
        if (rayInteractor && rayInteractor.IsOverUIGameObject())
        {
            return;
        }
        teleportInteractor.gameObject.SetActive(true);
    }

    public void DisableTeleport()
    {
        teleportInteractor.gameObject.SetActive(false);
    }
}
