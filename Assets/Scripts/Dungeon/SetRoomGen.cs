using UnityEngine;
using System.Collections.Generic;

public class SetRoomGen : MonoBehaviour
{
    public List<GameObject> Dungeons;
    [SerializeField] float scale = 1f;

    [Header("Table Bounds (centered on this position)")]
    public Vector2 tableSize;
    public Vector3 tableCenter;
    private Vector3 dungeonCenter;

    void Start()
    {
        
        if (Dungeons == null || Dungeons.Count == 0)
        {
            Debug.LogWarning("No dungeon prefabs assigned!");
            return;
        }

        int randomIndex = Random.Range(0, Dungeons.Count);

        

        GameObject dungeon = Instantiate(Dungeons[randomIndex], tableCenter, Quaternion.identity);
        dungeon.transform.localScale = Vector3.one * scale;
        dungeon.transform.Rotate(Vector3.up, 90);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(tableCenter, new Vector3(tableSize.x, 0, tableSize.y));
    }
}