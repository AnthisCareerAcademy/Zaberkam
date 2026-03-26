using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<ConnectorTransform> connectors;
    public ConnectorTransform GetRandomOpenConnector()
    {
        var openConnectors = connectors.FindAll(c => !c.IsConnected);
        if (openConnectors.Count == 0) return null;
        return openConnectors[Random.Range(0, openConnectors.Count)];
    }
    
}
