using UnityEngine;

public enum GroundConnector
{
    Grass,
    Sand,
    Water
}

[System.Serializable]
public struct Connectors<T>
{
    public T N_Connector;
    public T E_Connector;
    public T S_Connector;
    public T W_Connector;
}

public class WFC_Module : MonoBehaviour
{
    [field: SerializeField]
    public Connectors<GroundConnector> groundConnectors { get; private set; }
    [field: SerializeField, Range(0.0f, 1.0f)]
    public float probabilityPercent { get; private set; } = 0.5f;

    [field: SerializeField]
    public GroundConnector slotConnector { get; private set; }

    [field: Header("Module Rotation")]
    public bool rotated = false;
    [field: SerializeField]
    private int rotationID = 0;

    public void RotateModule(int newRotation)
    {
        rotationID = newRotation;

        for (int i = 0; i < rotationID; i++)
        {
            Connectors<GroundConnector> newConnectors = new Connectors<GroundConnector>();
            newConnectors.N_Connector = groundConnectors.W_Connector;
            newConnectors.W_Connector = groundConnectors.S_Connector;
            newConnectors.S_Connector = groundConnectors.E_Connector;
            newConnectors.E_Connector = groundConnectors.N_Connector;
            groundConnectors = newConnectors;

            transform.Rotate(0, 90, 0);
        }

        rotated = true;
    }
}