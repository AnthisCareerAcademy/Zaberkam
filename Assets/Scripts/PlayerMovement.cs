using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public CharacterController controller;

    private Vector3 velocity;
    private AttributeManager attributeManager;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        attributeManager = GetComponent<AttributeManager>();

        if (controller == null)
            Debug.LogError("CharacterController is missing!");

        if (attributeManager == null)
            Debug.LogError("AttributeManager is missing!");
    }

    void Update()
    {
        // Ground check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * attributeManager.currentSpeed * Time.deltaTime);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}