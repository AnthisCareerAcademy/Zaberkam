using TMPro;
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
        [Tooltip("A reference to the resource pool that should be drained to spawn objects.")]
        [SerializeField] ResourcePool resourcePool;
        
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
        costText.text = "Cost: " + cost + " " + resourcePool.ResourceName;
    }

    void Update()
    {
        resourcePool ??= FindFirstObjectByType<ResourcePool>();
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
        IXRHoverInteractor interactor = args.interactorObject;
        
        // The only time the second one is activated/deactivated is when the right hand triggers the spawner.
        if (resourceDisplays.Length == 2 && interactor.handedness == InteractorHandedness.Right)
        {
            resourceDisplays[1].SetActive(true);
        }
        else
        {
            resourceDisplays[0].SetActive(true);
        }
        
        base.OnHoverEntered(args);
    }
    
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        hovering = false;
        
        // Display the HUDs.
        IXRHoverInteractor interactor = args.interactorObject;
        
        // The only time the second one is activated/deactivated is when the right hand triggers the spawner.
        if (resourceDisplays.Length == 2 && interactor.handedness == InteractorHandedness.Right)
        {
            resourceDisplays[1].SetActive(false);
        }
        else
        {
            resourceDisplays[0].SetActive(false);
        }
        
        base.OnHoverExited(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (resourcePool.Resources >= cost)
        {
            resourcePool.Resources -= cost;
            
            // Instantiate new object.
            GameObject grabbable = Instantiate(grabbableObject, spawnPoint.position, Quaternion.identity);
            
            // Force the player to grab the object.
            XRGrabInteractable interactable = grabbable.GetComponent<XRGrabInteractable>();
            interactionManager.SelectEnter(args.interactorObject, interactable);
            base.OnSelectEntered(args);
        }
    }
}
