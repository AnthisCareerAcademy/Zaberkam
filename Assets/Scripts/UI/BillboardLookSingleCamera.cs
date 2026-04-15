using Interfaces;
using UnityEngine;

public class BillboardLookSingleCamera : MonoBehaviour
{
    private Transform cameraTransform;
    public bool active = true;
    
    void Update()
    {
        if (active)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>().IsPlayer) cameraTransform = other.gameObject.transform;
    }
}
