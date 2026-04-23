using System;
using Unity.Netcode;
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
    [SerializeField] float camSpeed;

    [Header("Item Placement Options")]
    [SerializeField] PlacementActionReferences actions;
    [SerializeField] PlacementCosts costs;
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
    
    private Transform tableCam;
    private Transform originalTableTransform;
    private Transform originalCam;
    
    RaycastHit hit;

    void Start()
    {
        resourcePool = GetComponent<ResourcePool>();
        if (!resourcePool) Debug.LogError("ResourcePool not found");
        controller = GetComponent<CharacterController>();
        if (!controller) Debug.LogError("CharacterController not found");
        
        previewMesh.mesh = SetMeshFromGameObject(items.firstItem);
        previewRenderer.materials = SetMaterials(items.firstItem);

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
                itemToPlace = items.firstItem;
                itemCost = costs.firstItem;
                break;
            case 1:
                itemToPlace = items.secondItem;
                itemCost = costs.secondItem;
                break;
            case 2:
                itemToPlace = items.thirdItem;
                itemCost = costs.thirdItem;
                break;
            case 3:
                itemToPlace = items.fourthItem;
                itemCost = costs.fourthItem;
                break;
            default:
                itemToPlace = null;
                itemCost = 0f;
                break;
        }
        
        // Check if there's a placeable surface nearby.
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, placementDistance))
        {
            resourcePool.PreviewCost(itemCost);
            pointer.transform.position = hit.point;
            pointer.SetActive(true);
        }
        else
        {
            resourcePool.PreviewCost();
            pointer.SetActive(false);
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
                    previewMesh.mesh = SetMeshFromGameObject(items.firstItem);
                    previewRenderer.materials = SetMaterials(items.firstItem);
                    break;
                case 1:
                    previewMesh.mesh = SetMeshFromGameObject(items.secondItem);
                    previewRenderer.materials = SetMaterials(items.secondItem);
                    break;
                case 2:
                    previewMesh.mesh = SetMeshFromGameObject(items.thirdItem);
                    previewRenderer.materials = SetMaterials(items.thirdItem);
                    break;
                case 3:
                    previewMesh.mesh = SetMeshFromGameObject(items.fourthItem);
                    previewRenderer.materials = SetMaterials(items.fourthItem);
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

    Mesh SetMeshFromGameObject(GameObject go)
    {
        MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
        return mf.sharedMesh;
    }

    Material[] SetMaterials(GameObject go)
    {
        MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();
        return mr.sharedMaterials;
    }
}
