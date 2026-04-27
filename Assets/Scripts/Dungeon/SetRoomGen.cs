using UnityEngine;
using System.Collections.Generic;

public class SetRoomGen : MonoBehaviour
{
    public List<GameObject> Dungeons;
    [SerializeField] float scale = 1f;

    [Header("Table Bounds (centered on this position)")]
    public Vector2 tableSize;
    public Vector3 tableCenter;

    void Start()
    {
        if (Dungeons == null || Dungeons.Count == 0)
        {
            Debug.LogWarning("No dungeon prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, Dungeons.Count);

        Vector3 randomPosition = new Vector3(
            Random.Range(-tableSize.x / 2, tableSize.x / 2),
            0,
            Random.Range(-tableSize.y / 2, tableSize.y / 2)
        ) + tableCenter;

        GameObject dungeon = Instantiate(Dungeons[randomIndex], randomPosition, Quaternion.identity);
        dungeon.transform.localScale = Vector3.one * scale;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(tableCenter, new Vector3(tableSize.x, 0, tableSize.y));
    }
}