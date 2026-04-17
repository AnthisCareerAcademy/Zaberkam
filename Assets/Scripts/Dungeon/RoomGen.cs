using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MonoBehaviour
{
    public Room startRoomPrefab;
    public Room bossRoomPrefab;
    public List<Room> roomPrefabs;
    public List<Room> sideRoomPrefabs;

    public int maxRooms = 15;

    [Header("Table Bounds (centered on this position)")]
    public Vector2 tableSize;
    public Vector3 tableCenter;

    private List<Room> spawnedRooms = new List<Room>();
    private List<Bounds> spawnedBounds = new List<Bounds>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        if (!startRoomPrefab || !bossRoomPrefab || roomPrefabs.Count == 0)
        {
            Debug.LogError("Missing room prefabs.");
            return;
        }

        spawnedRooms.Clear();
        spawnedBounds.Clear();

        Room startRoom = Instantiate(startRoomPrefab, transform.position, Quaternion.identity);
        RegisterRoom(startRoom);

        List<ConnectorTransform> openConnectors = new List<ConnectorTransform>(startRoom.connectors);

        int roomsSpawned = 1;

        // ---------------------------------------------------------
        // NORMAL ROOM GENERATION
        // ---------------------------------------------------------
        while (roomsSpawned < maxRooms - 1 && openConnectors.Count > 0)
        {
            int parentIndex = Random.Range(0, openConnectors.Count);
            ConnectorTransform parentConnector = openConnectors[parentIndex];

            if (parentConnector.IsConnected)
            {
                openConnectors.RemoveAt(parentIndex);
                continue;
            }

            bool useSide = sideRoomPrefabs.Count > 0 && Random.value < 0.05f;
            Room prefabToUse = useSide
                ? sideRoomPrefabs[Random.Range(0, sideRoomPrefabs.Count)]
                : roomPrefabs[Random.Range(0, roomPrefabs.Count)];

            Room newRoom = Instantiate(prefabToUse);

            if (!AlignRoomToConnector(newRoom, parentConnector))
            {
                Destroy(newRoom.gameObject);
                openConnectors.RemoveAt(parentIndex);
                continue;
            }

            Bounds newBounds = GetBounds(newRoom);

            if (OverlapsExisting(newBounds))
            {
                Destroy(newRoom.gameObject);
                openConnectors.RemoveAt(parentIndex);
                continue;
            }

            // Successful placement
            RegisterRoom(newRoom);

            ConnectorTransform childConnector = GetBestFacingConnector(newRoom, parentConnector);

            parentConnector.Connect();
            childConnector.Connect();

            foreach (var c in newRoom.connectors)
                if (!c.IsConnected)
                    openConnectors.Add(c);

            roomsSpawned++;
        }

        // ---------------------------------------------------------
        // FINAL STEP: PLACE BOSS ROOM (GUARANTEED)
        // ---------------------------------------------------------
        TryPlaceBossRoom(openConnectors);

        Debug.Log("Dungeon generation complete.");
    }

    // ---------------------------------------------------------
    // BOSS ROOM PLACEMENT PASS
    // ---------------------------------------------------------
    void TryPlaceBossRoom(List<ConnectorTransform> openConnectors)
    {
        foreach (var parentConnector in openConnectors)
        {
            if (parentConnector.IsConnected)
                continue;

            Room boss = Instantiate(bossRoomPrefab);

            if (!AlignRoomToConnector(boss, parentConnector))
            {
                Destroy(boss.gameObject);
                continue;
            }

            Bounds b = GetBounds(boss);

            if (OverlapsExisting(b))
            {
                Destroy(boss.gameObject);
                continue;
            }

            // SUCCESS
            RegisterRoom(boss);

            ConnectorTransform child = GetBestFacingConnector(boss, parentConnector);
            parentConnector.Connect();
            child.Connect();

            Debug.Log("Boss room placed successfully.");
            return;
        }

        Debug.LogWarning("Boss room could NOT be placed.");
    }

    // ---------------------------------------------------------
    // ALIGNMENT + BOUNDS
    // ---------------------------------------------------------
    bool AlignRoomToConnector(Room room, ConnectorTransform parentConnector)
    {
        ConnectorTransform childConnector = GetBestFacingConnector(room, parentConnector);
        if (childConnector == null) return false;

        Vector3 parentForward = parentConnector.transform.forward;
        parentForward.y = 0f;
        parentForward.Normalize();

        Vector3 childForward = childConnector.transform.forward;
        childForward.y = 0f;
        childForward.Normalize();

        float angle = Vector3.SignedAngle(childForward, -parentForward, Vector3.up);
        float snappedAngle = Mathf.Round(angle / 90f) * 90f;

        room.transform.Rotate(Vector3.up, snappedAngle);
        Physics.SyncTransforms();

        Vector3 delta = parentConnector.transform.position - childConnector.transform.position;
        room.transform.position += delta;

        Physics.SyncTransforms();
        return true;
    }

    ConnectorTransform GetBestFacingConnector(Room room, ConnectorTransform parentConnector)
    {
        ConnectorTransform best = null;
        float bestDot = -1f;

        foreach (var c in room.connectors)
        {
            if (c.IsConnected) continue;

            Vector3 dirToParent = (parentConnector.transform.position - c.transform.position).normalized;
            float dot = Vector3.Dot(c.transform.forward, dirToParent);

            if (dot > bestDot)
            {
                bestDot = dot;
                best = c;
            }
        }

        return best;
    }

    void RegisterRoom(Room room)
    {
        spawnedRooms.Add(room);
        spawnedBounds.Add(GetBounds(room));
    }

    Bounds GetBounds(Room room)
    {
        BoxCollider box = room.GetComponent<BoxCollider>();
        return box.bounds;
    }

    bool OverlapsExisting(Bounds newBounds)
    {
        if (IsOutsideTable(newBounds))
            return true;

        foreach (var b in spawnedBounds)
            if (b.Intersects(newBounds))
                return true;

        return false;
    }

    bool IsOutsideTable(Bounds b)
    {
        float halfW = tableSize.x / 2f;
        float halfH = tableSize.y / 2f;

        Vector3 c = tableCenter;

        if (b.min.x < c.x - halfW) return true;
        if (b.max.x > c.x + halfW) return true;
        if (b.min.z < c.z - halfH) return true;
        if (b.max.z > c.z + halfH) return true;

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(tableCenter, new Vector3(tableSize.x, 0.1f, tableSize.y));
    }
}