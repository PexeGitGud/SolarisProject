using UnityEngine;

public class WFC_ModuleDressing : MonoBehaviour
{
    [field: SerializeField]
    private GameObject groundPlaceholder;

    [field: SerializeField]
    public float probability { get; private set; } = 1.0f;

    void Start()
    {
        Destroy(groundPlaceholder);
    }
}