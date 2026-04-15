using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabbableObjectSpawner : XRBaseInteractable
{
    // Thanks to Project-NSX on Reddit for an answer on how to spawn grabbable objects.
    [Header("Object Spawning")]
        [Tooltip("The object to spawn. It must have an XRGrabInteractable script.")]
        [SerializeField] GameObject grabbableObject;
        
        [Tooltip("Where to spawn the object.")]
        [SerializeField] Transform spawnPoint;
    
    [Header("Visual Settings")]
        [Tooltip("The mesh of the object sitting in the tray.")]
        [SerializeField] MeshRenderer objectVisualMesh;
        
        [Tooltip("The material to use by default.")]
        [SerializeField] Material defaultMaterial;
        
        [Tooltip("The material to use when the object can be grabbed.")]
        [SerializeField] Material validMaterial;
        
        [Tooltip("The material to use when the object cannot be grabbed.")]
        [SerializeField] Material invalidMaterial;
    
    [Header("Spawn Costs")]
        private ResourcePool resourcePool;
        
        [Tooltip("How many resources to consume when spawning an object.")]
        [SerializeField] float cost;
        
        [Tooltip("Display canvas for the spawner's cost.")]
        [SerializeField] TextMeshProUGUI costText;
    
    [Header("Resource Pool Display Elements")]
        [Tooltip("Display canvases for the resource pool. Add either 1 (for a generic display) or 2 (for wrist watch-style displays) in the order left-right.")]
        [SerializeField] GameObject[] resourceDisplays;

    private bool hovering;

    void Start()
    {
        resourcePool = FindFirstObjectByType<ResourcePool>();
        if (resourcePool) costText.text = "Cost: " + cost + " " + resourcePool.ResourceName;
        else costText.text = "Cost: " + cost + " Lifeblood";
    }

    void Update()
    {
        if (!resourcePool) resourcePool = FindFirstObjectByType<ResourcePool>();
        
        if (hovering)
        {
            objectVisualMesh.material = resourcePool.Resources >= cost ? validMaterial : invalidMaterial;
        }
        else 
        {
            objectVisualMesh.material = defaultMaterial;
        }
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        hovering = true;
        
        // Display the HUDs.
        DirectInteractorUIActivator interactor = args.interactorObject as DirectInteractorUIActivator;
        if (interactor) interactor.DisplayWristUI();
        
        base.OnHoverEntered(args);
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        hovering = false;
        
        // Display the HUDs.
        DirectInteractorUIActivator interactor = args.interactorObject as DirectInteractorUIActivator;
        if (interactor) interactor.HideWristUI();
        
        base.OnHoverExited(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (resourcePool.Resources >= cost)
        {
            resourcePool.Resources -= cost;

            SpawnObjectServerRpc();

        }

        base.OnSelectEntered(args);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectServerRpc(ServerRpcAttribute rpcParams = default)
    {
        GameObject obj = Instantiate(grabbableObject, spawnPoint.position, Quaternion.identity);
        XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();

        NetworkObject netObj = obj.GetComponent<NetworkObject>();
        netObj.Spawn();
    }
}
