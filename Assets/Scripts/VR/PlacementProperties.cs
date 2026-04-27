using UnityEngine;
using UnityEngine.Serialization;

namespace Interfaces
{
    public class PlacementProperties : MonoBehaviour
    {
        // Place this on ALL object spawners.
        [Tooltip("The prefab to instantiate.")]
        public GameObject SpawnedItem;
        
        [Tooltip("The mesh from the Visual object.")]
        public MeshFilter Mesh;
        
        [Tooltip("The material(s) from the Visual object.")]
        public MeshRenderer Renderer;
        
        [Tooltip("How much the item costs to spawn.")]
        public float Cost;
    }
}
