using UnityEngine;

public class BillboardLook : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    public bool active = true;
    
    void Update()
    {
        if (active)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }
    }
}
