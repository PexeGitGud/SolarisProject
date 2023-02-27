using UnityEngine;

public enum GroundConnector
{
    Grass,
    Rock_Stripe,
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

    [field: Header("Module Rotation")]
    public bool rotated = false;
    [field: SerializeField]
    private int rotationID = 0;

    [field: Header("Module Dressing")]
    public WFC_ModuleDressing[] possibleModuleDressings;
    [field: SerializeField]
    public WFC_ModuleDressing selectedModuleDressing { get; private set; }
    [field: SerializeField]
    public bool moduleDressed { get; private set; } = false;

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
        float n = possibleModuleDressings.Length;

        float totalRatio = 0;
        foreach (WFC_ModuleDressing pmd in possibleModuleDressings)
            totalRatio += n * pmd.probabilityPercent;

        float weightedRandom = Random.Range(0, totalRatio);

        int weightedRandomIndex = 0;
        foreach (WFC_ModuleDressing pmd in possibleModuleDressings)
        {
            if ((weightedRandom -= n * pmd.probabilityPercent) < 0.0f)
                break;
            weightedRandomIndex++;
        }

        return possibleModuleDressings[weightedRandomIndex];
    }
}