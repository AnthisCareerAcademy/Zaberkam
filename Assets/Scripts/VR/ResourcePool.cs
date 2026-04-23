using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePool : MonoBehaviour
{
    [Header("Default Values")]
        [Tooltip("Starting amount of resources.")]
        [SerializeField] float resources = 100;
        
        [Tooltip("Maximum amount of resources.")]
        [SerializeField] float maxResources = 100;
        
        [Tooltip("Name of resource (used in UI).")]
        [SerializeField] string resourceName = "Lifeblood";

        [Tooltip("Amount of resource recovered each second.")]
        [SerializeField] float recoveryRate = 1;

    [Header("Display Options (optional)")]
        [Tooltip("UI text component that displays how many resources are left")]
        [SerializeField] TextMeshProUGUI[] resourceText;
        
        [Tooltip("UI slider component showing how close to full the resource pool is.")]
        [SerializeField] Slider[] resourceSlider;

    private float preview;
    
    // We only want to *read* the resource name in other scripts, not change it.
    public string ResourceName => resourceName;

    public float Resources
    {
        get => resources;
        set => resources = Mathf.Clamp(value, 0, maxResources);
    }
    
    void Update()
    {
        Resources += Time.deltaTime * recoveryRate;

        if (resourceText.Length > 0)
        {
            foreach (var t in resourceText)
            {
                t.text = ResourceName + ": " + Mathf.Floor(resources);
            }
        }

        if (resourceSlider.Length > 0)
        {
            float width = 1 - preview / resources;
            if (width < 0) width = 1;
            
            foreach (var t in resourceSlider)
            {
                t.value = resources / maxResources;
                
                t.handleRect.anchorMin = new Vector2(width, 0);
                t.handleRect.anchorMax = Vector2.one;
                t.handleRect.sizeDelta = Vector2.zero;
                t.handleRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void PreviewCost(float amount = 0f)
    {
        preview = amount;
    }
}
