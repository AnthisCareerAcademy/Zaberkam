using UnityEngine;

public class BoxSpawnChance : MonoBehaviour
{
    [SerializeField] GameObject box;

    private void Start()
    {
        if (Random.value < 0.25f) Instantiate(box, transform.position, Quaternion.identity);
    }
}
