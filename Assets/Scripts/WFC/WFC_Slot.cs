using UnityEngine;

public class WFC_Slot : MonoBehaviour
{
    public GameObject placeholderObj;
    public WFC_Module[] possibleModules;
    public Vector2 coord = new Vector2(-1, -1);
    public bool collapsed = false;
    public WFC_Module collapsedModule;

    public void Collapse()
    {
        collapsedModule = GetWeightedModule();
        Instantiate(collapsedModule, transform);
        Destroy(placeholderObj);
        collapsed = true;
    }

    WFC_Module GetWeightedModule()
    {
        float totalRatio = 0;
        foreach (WFC_Module pm in possibleModules)
            totalRatio += pm.probability;

        float weightedRandom = Random.Range(0, totalRatio);

        int weightedRandomIndex = 0;
        foreach (WFC_Module pm in possibleModules)
        {
            if ((weightedRandom -= pm.probability) < 0.0f)
                break;
            weightedRandomIndex++;
        }

        return possibleModules[weightedRandomIndex];
    }

    public void TurnRed()
    {
        placeholderObj.GetComponent<Renderer>().material.color = Color.red;
    }
}