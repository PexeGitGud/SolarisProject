using UnityEngine;

public class WFC_ModuleDressing : MonoBehaviour
{
    public enum DressingConnector
    {
        None,
        Rock
    }

    [field: SerializeField]
    private GameObject groundPlaceholder;

    [field: SerializeField]
    public Connectors<DressingConnector> dressingConnectors { get; private set; }
    [field: SerializeField]
    public float probability { get; private set; } = 1.0f;

    void Start()
    {
        Destroy(groundPlaceholder);
    }
}