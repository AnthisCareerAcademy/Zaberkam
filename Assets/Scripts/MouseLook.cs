using UnityEngine;
public class MouseLook : MonoBehaviour
{
   public float mouseSensitivity = 500f;
   public Transform playerBody;
   private float xRotation = 0f;
   void Start()
   {
       Cursor.lockState = CursorLockMode.Locked; // Lock cursor to screen center
   }
   void Update()
   {
       // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate camera vertically (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player horizontally (yaw)
        playerBody.Rotate(Vector3.up * mouseX);

   }
}