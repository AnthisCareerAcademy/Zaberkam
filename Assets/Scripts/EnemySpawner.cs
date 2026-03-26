using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> doors;
    [SerializeField] int maxEnemies = 5;

    private BoxCollider box;
    private List<GameObject> openDoors;
    private bool roomActive = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int EnemyCount => spawnedEnemies.Count;
    public Material transMat;

    void Start()
    {
        box = GetComponent<BoxCollider>();
        StartCoroutine(GetOpenDoors());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roomActive)
        {
            roomActive = true;
            foreach (var door in openDoors)
            {
                door.SetActive(true);
                door.GetComponent<Renderer>().material = transMat;
            }
            TrySpawnEnemies();
        }
    }

    void Update()
    {
        spawnedEnemies.RemoveAll(e => e == null);

        if (spawnedEnemies.Count == 0 && roomActive)
        {
            foreach (var door in openDoors)
                door.SetActive(false);
        }
    }

    public void TrySpawnEnemies()
    {
        while (spawnedEnemies.Count < maxEnemies)
        {
            if (enemyPrefabs.Count == 0) return;

            Bounds bounds = box.bounds;
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                transform.position.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Count);
                GameObject enemy = Instantiate(enemyPrefabs[randomIndex], hit.position, Quaternion.identity, transform);
                spawnedEnemies.Add(enemy);
            }
        }
    }

    private IEnumerator GetOpenDoors()
    {
        yield return new WaitForSeconds(.5f);
        openDoors = new List<GameObject>();

        foreach (var door in doors)
        {
            if (!door.activeSelf)
                openDoors.Add(door);
        }
    }
}