using UnityEngine;

public enum Connector
{
    Grass,
    Rock_Stripe,
    Sand,
    Water
}

public class WFC_Module : MonoBehaviour
{
    [field: Header("Connectors")]
    [field: SerializeField]
    public Connector N_Connector { get; private set; }
    [field: SerializeField]
    public Connector E_Connector { get; private set; }
    [field: SerializeField]
    public Connector S_Connector { get; private set; }
    [field: SerializeField]
    public Connector W_Connector { get; private set; }
    [field: SerializeField]
    public float probability { get; private set; } = 1.0f;

    [field: Header("Module Rotation")]
    public bool rotated = false;
    [field: SerializeField]
    private int rotationID = 0;

    [field: Header("Module Dressing")]
    [field: SerializeField]
    private WFC_ModuleDressing[] possibleModuleDressings;
    [field: SerializeField]
    private WFC_ModuleDressing selectedModuleDressing;
    [field: SerializeField]
    public bool moduleDressed { get; private set; } = false;

    public void RotateModule(int newRotation)
    {
        rotationID = newRotation;

        for (int i = 0; i < rotationID; i++)
        {
            Connector aux = N_Connector;
            N_Connector = W_Connector;
            W_Connector = S_Connector;
            S_Connector = E_Connector;
            E_Connector = aux;

            transform.Rotate(0, 90, 0);

            rotated = true;
        }
    }

    public void InstatiateModuleDressing()
    {
        moduleDressed = true;

        if (possibleModuleDressings.Length <= 0)
            return;

        selectedModuleDressing = Instantiate(GetWeightedModuleDressing(), transform);
    }

    WFC_ModuleDressing GetWeightedModuleDressing()
    {
        float totalRatio = 0;
        foreach (WFC_ModuleDressing pm in possibleModuleDressings)
            totalRatio += pm.probability;

        float weightedRandom = Random.Range(0, totalRatio);

        int weightedRandomIndex = 0;
        foreach (WFC_ModuleDressing pm in possibleModuleDressings)
        {
            if ((weightedRandom -= pm.probability) < 0.0f)
                break;
            weightedRandomIndex++;
        }

        return possibleModuleDressings[weightedRandomIndex];
    }
}