using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct PlacementActionReferences
{
    public InputActionReference
        place, view, firstItem, secondItem, thirdItem, fourthItem;
}

[Serializable]
public struct PlacementCosts
{
    public float
        firstItem, secondItem, thirdItem, fourthItem;
}

[Serializable]
public struct PlaceableItems
{
    public GameObject
        firstItem, secondItem, thirdItem, fourthItem;
}

[Serializable]
public struct ItemPreviews
{
    public GameObject
        firstItem, secondItem, thirdItem, fourthItem;
}

public class Overseer : MonoBehaviour
{
    [Header("Movement Options")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float gravity = -10f;

    [Header("Camera Options")]
    [SerializeField] InputActionReference look;
    [SerializeField] InputActionReference pause;
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] GameObject cam;
    [SerializeField] Transform originalCam;
    [SerializeField] Transform tableCam;
    [SerializeField] float camSpeed;

    [Header("Item Placement Options")]
    [SerializeField] PlacementActionReferences actions;
    [SerializeField] PlacementCosts costs;
    [SerializeField] PlaceableItems items;
    [SerializeField] ItemPreviews previews;

    private ResourcePool resourcePool;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private bool viewing;

    private int currentItemID;

    void Start()
    {
        resourcePool = GetComponent<ResourcePool>();
        if (!resourcePool) Debug.LogError("ResourcePool not found");
        controller = GetComponent<CharacterController>();
        if (!controller) Debug.LogError("CharacterController not found");

        Unpause();
    }

    void Update()
    {
        CheckPause();

        if (!Cursor.visible)
        {
            if (!viewing)
            {
                DoLook();
                DoMove();
            }
            
            HandleAction(actions.place, Place);
            HandleAction(actions.view, View);
            HandleAction(actions.firstItem, FirstItem);
            HandleAction(actions.secondItem, SecondItem);
            HandleAction(actions.thirdItem, ThirdItem);
            HandleAction(actions.fourthItem, FourthItem);
        }
        
        if (viewing) ChangeCamera(tableCam, camSpeed);
        else ChangeCamera(originalCam, camSpeed * 2);
        viewing = false;
        
        previews.firstItem.SetActive(currentItemID == 0);
        previews.secondItem.SetActive(currentItemID == 1);
        previews.thirdItem.SetActive(currentItemID == 2);
        previews.fourthItem.SetActive(currentItemID == 3);
    }

    void DoLook()
    {
        Vector2 lookInput = look.action.ReadValue<Vector2>() * (mouseSensitivity * Time.deltaTime);

        // Limit vertical rotation.
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Turn the camera and the player
        originalCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x);
    }

    void DoMove()
    {
        Vector2 movement = move.action.ReadValue<Vector2>().normalized;
        velocity.x = movement.x * moveSpeed;
        velocity.z = movement.y * moveSpeed;

        bool isGrounded = controller.isGrounded;

        // Apply gravity.
        if (!isGrounded) velocity.y += gravity * Time.deltaTime;

        // Perform jumps.
        if (isGrounded && jump.action.IsPressed())
        {
            velocity.y = jumpHeight;
        }

        velocity = transform.rotation * velocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAction(InputActionReference input, Action action)
    {
        if (input.action.IsPressed())
        {
            action();
        }
    }

    void CheckPause()
    {
        // Unlock cursor on pause. Change to a pause menu eventually.
        if (pause.action.WasPressedThisFrame())
        {
            Pause();
        }

        if (actions.place.action.WasPressedThisFrame())
        {
            Unpause();
        }
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Place()
    {
        RaycastHit hit;
        if (Physics.Raycast(originalCam.position, originalCam.forward, out hit, 0.5f))
        {
            Debug.DrawRay(originalCam.position, originalCam.forward * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(originalCam.position, originalCam.forward * 1000, Color.white);
        }
    }

    void View()
    {
        viewing = true;
    }

    void FirstItem()
    {
        currentItemID = 0;
    }

    void SecondItem()
    {
        currentItemID = 1;
    }

    void ThirdItem()
    {
        currentItemID = 2;
    }

    void FourthItem()
    {
        currentItemID = 3;
    }
    
    void ChangeCamera(Transform newCam, float speed = 1f)
    {
        if (speed == 0)
        {
            cam.transform.SetPositionAndRotation(newCam.position, newCam.rotation);
        }
        else
        {
            float t = speed * Time.deltaTime;
            cam.transform.SetPositionAndRotation(
                Vector3.Lerp(cam.transform.position, newCam.transform.position, t),
                Quaternion.Lerp(cam.transform.rotation, newCam.transform.rotation, t)
            );
        }

        if (Vector3.Distance(cam.transform.position, newCam.position) < 0.025f)
        {
            cam.transform.SetPositionAndRotation(newCam.position, newCam.rotation);
        }
    }
}
