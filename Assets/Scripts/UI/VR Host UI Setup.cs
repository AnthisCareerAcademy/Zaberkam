using UnityEngine;

public class VRHostUISetup : MonoBehaviour
{
    [SerializeField] Camera cam;

    void Awake()
    {
        Canvas hostUI = GameObject.Find("VRHostUI").GetComponent<Canvas>();
        if (hostUI)
        {
            hostUI.worldCamera = cam;
        }
    }
}
