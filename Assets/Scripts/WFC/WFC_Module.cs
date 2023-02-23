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
    public Connector N_Connector, E_Connector, S_Connector, W_Connector;

    public bool rotated = false;
    public int rotation = 0;

    public float probability = 1.0f;

    [Header("Module Dressing")]
    public float moduleDressingProbability = 0.5f;
    public GameObject[] possibleModuleDressings;
    public GameObject selectedModuleDressing;

    public void RotateModule(int newRotation)
    {
        rotation = newRotation;

        for (int i = 0; i < rotation; i++)
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
        if (possibleModuleDressings.Length <= 0)
            return;

        if (Random.Range(0.0f, 1.0f) > moduleDressingProbability)
            return;

        int rand = Random.Range(0, possibleModuleDressings.Length);

        selectedModuleDressing = Instantiate(possibleModuleDressings[rand], transform);
    }
}