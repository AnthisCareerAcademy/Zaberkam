using UnityEngine;

public class ConnectorTransform : MonoBehaviour
{
    public bool IsConnected { get; private set; } = false;
    public void Connect()
    {
        if (IsConnected) return;

        IsConnected = true;
    }
}
