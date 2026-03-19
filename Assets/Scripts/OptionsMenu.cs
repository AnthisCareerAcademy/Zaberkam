using UnityEngine;
using UnityEngine.InputSystem;

public class OptionsMenu : MonoBehaviour
{
    [Header("Menu Setup")] [SerializeField]
    GameObject optionsMenu;

    [SerializeField] Transform cameraTransform;
    [SerializeField] float distance;
    [SerializeField] InputActionProperty settingsActionLeft;
    [SerializeField] InputActionProperty settingsActionRight;

    void Start()
    {
        transform.LookAt(cameraTransform);
    }

    void Update()
    {
        if (settingsActionLeft.action.ReadValue<float>() > 0 || settingsActionRight.action.ReadValue<float>() > 0)
        {
            OpenOptionsMenu();
        }

        if (Vector3.Distance(transform.position, cameraTransform.position) >= 1.75f)
        {
            transform.LookAt(cameraTransform);
        }
    }

    void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        transform.position = cameraTransform.position + cameraTransform.rotation * Vector3.forward * distance;
        transform.LookAt(cameraTransform);
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
}
