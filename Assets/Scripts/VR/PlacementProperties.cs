using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Interfaces
{
    public class PlacementProperties : MonoBehaviour
    {
        // Place this on ALL object spawners.
        public GameObject SpawnedItem;
        [SerializeField] GameObject visual;
        [NonSerialized] public Mesh Mesh;
        [NonSerialized] public Material[] Materials;
        public float Cost;

        public void Start()
        {
            Mesh = visual.GetComponent<MeshFilter>().sharedMesh;
            Materials = visual.GetComponent<MeshRenderer>().sharedMaterials;
        }
    }
}
