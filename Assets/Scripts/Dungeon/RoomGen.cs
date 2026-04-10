using System.Collections.Generic;
using UnityEngine;

public class RoomGen : MonoBehaviour
{
    public Room startRoomPrefab;
    public Room bossRoomPrefab;
    public List<Room> roomPrefabs;
    public List<Room> sideRoomPrefabs;
    public Bounds dungeonBounds;

    public int maxRooms = 15;

    private List<Room> spawnedRooms = new List<Room>();
    private List<Bounds> spawnedBounds = new List<Bounds>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        if (startRoomPrefab == null || bossRoomPrefab == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("Missing room prefabs.");
            return;
        }

        spawnedRooms.Clear();
        spawnedBounds.Clear();

        // ✅ Spawn start room at THIS object's position & rotation
        Room startRoom = Instantiate(startRoomPrefab, transform.position, transform.rotation);
        RegisterRoom(startRoom);

        List<ConnectorTransform> openConnectors = new List<ConnectorTransform>(startRoom.connectors);

        int roomsSpawned = 1;

        while (roomsSpawned < maxRooms && openConnectors.Count > 0)
        {
            int parentIndex = Random.Range(0, openConnectors.Count);
            ConnectorTransform parentConnector = openConnectors[parentIndex];

            if (parentConnector.IsConnected)
            {
                openConnectors.RemoveAt(parentIndex);
                continue;
            }

            Room prefabToUse;

            if (roomsSpawned == maxRooms - 1)
            {
                prefabToUse = bossRoomPrefab;
            }
            else
            {
                bool useSide = sideRoomPrefabs.Count > 0 && Random.value < 0.05f;
                prefabToUse = useSide
                    ? sideRoomPrefabs[Random.Range(0, sideRoomPrefabs.Count)]
                    : roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            }

            // ✅ Spawn new rooms at generator position (instead of 0,0,0)
            Room newRoom = Instantiate(prefabToUse, transform.position, transform.rotation);

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

            if (OutOfBounds(newBounds))
            {
                Destroy(newRoom.gameObject);
                openConnectors.RemoveAt(parentIndex);
                continue;
            }

            RegisterRoom(newRoom);

            ConnectorTransform childConnector = GetBestFacingConnector(newRoom, parentConnector);

            parentConnector.Connect();
            childConnector.Connect();

            foreach (var c in newRoom.connectors)
                if (!c.IsConnected)
                    openConnectors.Add(c);

            roomsSpawned++;
        }

        Debug.Log("Dungeon generation complete.");
    }

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
        foreach (var b in spawnedBounds)
        {
            if (b.Intersects(newBounds))
                return true;
        }

        return false;
    }

    bool OutOfBounds(Bounds newBounds)
    {
        if (dungeonBounds.size == Vector3.zero) return false;

        if (!dungeonBounds.Contains(newBounds.min) ||
            !dungeonBounds.Contains(newBounds.max))
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
        if (dungeonBounds.size == Vector3.zero)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(dungeonBounds.center, dungeonBounds.size);
    }
}