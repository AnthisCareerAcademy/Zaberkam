using System;
using System.Net;
using Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public struct PlacementActionReferences
{
    public InputActionReference
        place, view, firstItem, secondItem, thirdItem, fourthItem, switchItem;
}

[Serializable]
public struct PlaceableItems
{
    public PlacementProperties
        FirstItem, SecondItem, ThirdItem, FourthItem;
}

public class Overseer : NetworkBehaviour
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
    [SerializeField] Image crosshair;
    [SerializeField] float camSpeed;

    [Header("Item Placement Options")]
    [SerializeField] PlacementActionReferences actions;
    [SerializeField] PlaceableItems items;
    [SerializeField] MeshFilter previewMesh;
    [SerializeField] MeshRenderer previewRenderer;
    [SerializeField] GameObject pointer;
    [SerializeField] float placementDistance = 2f;
    [SerializeField] float scale = 0.1f;

    private ResourcePool resourcePool;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    private int currentItemID;
    private float itemCost;
    private GameObject itemToPlace;
    private Color crosshairPlaceColor = new(0.5f, 0f, 0.67f);
    private Color crosshairSwapColor = new(1f, 1f, 0f);
    
    private Transform tableCam;
    private Transform originalTableTransform;
    private Transform originalCam;
    
    private RaycastHit hit;
    private PlacementProperties switchItem;

    void Start()
    {
        resourcePool = GetComponent<ResourcePool>();
        if (!resourcePool) Debug.LogError("ResourcePool not found");
        controller = GetComponent<CharacterController>();
        if (!controller) Debug.LogError("CharacterController not found");

        previewMesh.mesh = items.FirstItem.Mesh.sharedMesh;
        previewRenderer.materials = items.FirstItem.Renderer.sharedMaterials;

        Unpause();
    }

    void Awake()
    {
        tableCam = GameObject.Find("TableCam")?.transform;

        if (!tableCam)
        {
            tableCam = new GameObject("TableCam").transform;
            tableCam.SetParent(transform);
            tableCam.position = cam.transform.position;
            tableCam.Translate(Vector3.up * 1.5f);
            tableCam.rotation = Quaternion.LookRotation(-transform.up);
        }

        originalTableTransform = new GameObject("TableCamRef").transform;
        
        originalTableTransform.position = tableCam.position;
        originalTableTransform.rotation = tableCam.rotation;

        originalCam = new GameObject("OriginalCamRef").transform;
        originalCam.SetParent(transform);
        
        originalCam.position = cam.transform.position;
        originalCam.rotation = cam.transform.rotation;
    }

    void Update()
    {
        CheckPause();
        
        if (actions.view.action.IsPressed())
        {
            ChangeCamera(tableCam, camSpeed);
            DoTableLook();
        }
        else
        {
            tableCam.position = originalTableTransform.position;
            ChangeCamera(originalCam, camSpeed * 2);
            DoMove();
            DoLook();
        }

        if (!Cursor.visible)
        {
            HandleAction(actions.place, Place);
            
            // The item swaps just change the current item id.
            HandleAction(actions.firstItem, () => currentItemID = 0); 
            HandleAction(actions.secondItem, () => currentItemID = 1);
            HandleAction(actions.thirdItem, () => currentItemID = 2);
            HandleAction(actions.fourthItem, () => currentItemID = 3);
        }
    }

    void FixedUpdate()
    {
        switch (currentItemID)
        {
            case 0:
                itemToPlace = items.FirstItem.SpawnedItem;
                itemCost = items.FirstItem.Cost;
                break;
            case 1:
                itemToPlace = items.SecondItem.SpawnedItem;
                itemCost = items.SecondItem.Cost;
                break;
            case 2:
                itemToPlace = items.ThirdItem.SpawnedItem;
                itemCost = items.ThirdItem.Cost;
                break;
            case 3:
                itemToPlace = items.FourthItem.SpawnedItem;
                itemCost = items.FourthItem.Cost;
                break;
            default:
                itemToPlace = null;
                itemCost = 0f;
                break;
        }
        
        // Check if there's a placeable surface nearby.
        resourcePool.PreviewCost();
        pointer.SetActive(false);
        crosshair.color = Color.white;
        
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, placementDistance))
        {
            // Check if a new item is being targeted.
            switchItem = hit.collider.gameObject.GetComponent<PlacementProperties>();
            
            if (switchItem)
            {
                crosshair.color = crosshairSwapColor;
                HandleAction(actions.switchItem, Switch);
                return;
            }
            
            // Show the pointer.
            crosshair.color = crosshairPlaceColor;
            resourcePool.PreviewCost(itemCost);
            pointer.transform.position = hit.point;
            pointer.SetActive(true);
        }
    }

    void DoLook()
    {
        Vector2 lookInput = look.action.ReadValue<Vector2>() * (mouseSensitivity * Time.deltaTime);

        // Limit vertical rotation.
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Turn the camera and the player.
        if (!Cursor.visible)
        {
            originalCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * lookInput.x);
        }
    }
    
    void DoTableLook()
    {
        Vector2 lookInput = look.action.ReadValue<Vector2>() * (mouseSensitivity * 0.025f * Time.deltaTime);
        
        // Move the camera around.
        if (!Cursor.visible) tableCam.Translate(new Vector3(lookInput.x, lookInput.y, 0f));
    }

    void DoMove()
    {
        Vector2 movement = move.action.ReadValue<Vector2>().normalized;
        
        // Apply movement only if the mouse is locked (the game is paused). Otherwise, just gravity.
        if (!Cursor.visible)
        {
            velocity.x = movement.x * moveSpeed;
            velocity.z = movement.y * moveSpeed;
        }

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
        if (input.action.WasReleasedThisFrame())
        {
            action();
            switch (currentItemID)
            {
                case 0:
                    previewMesh.mesh = items.FirstItem.Mesh.sharedMesh;
                    previewRenderer.materials = items.FirstItem.Renderer.sharedMaterials;
                    break;
                case 1:
                    previewMesh.mesh = items.SecondItem.Mesh.sharedMesh;
                    previewRenderer.materials = items.SecondItem.Renderer.sharedMaterials;
                    break;
                case 2:
                    previewMesh.mesh = items.ThirdItem.Mesh.sharedMesh;
                    previewRenderer.materials = items.ThirdItem.Renderer.sharedMaterials;
                    break;
                case 3:
                    previewMesh.mesh = items.FourthItem.Mesh.sharedMesh;
                    previewRenderer.materials = items.FourthItem.Renderer.sharedMaterials;
                    break;
            }
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
        // The pointer is active if the object can be placed. Otherwise, skip.
        if (!pointer.activeSelf) return; 

        if (itemToPlace && resourcePool.Resources >= itemCost)
        {
            resourcePool.Resources -= itemCost;
            GameObject newObj = Instantiate(itemToPlace, pointer.transform.position, itemToPlace.transform.rotation);
            newObj.transform.localScale = Vector3.one * scale;
        }
    }
    
    void Switch()
    { 
        switch (currentItemID)
        {
            case 0:
                items.FirstItem = switchItem;
                break;
            case 1:
                items.SecondItem = switchItem;
                break;
            case 2:
                items.ThirdItem = switchItem;
                break;
            case 3:
                items.FourthItem = switchItem;
                break;
        }
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
