using UnityEngine;

namespace Interfaces
{
    public class PlacementProperties
    {
        // Place this on ALL object spawners.
        public GameObject SpawnedItem;
        public Mesh Mesh;
        public Material[] Materials;
        public float Cost;
    }
}
