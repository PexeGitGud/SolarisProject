using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WFC_Slot_1 : MonoBehaviour
{
    public GameObject placeholderObj;
    public WFC_Module_1[] possibleModules;
    public Vector2 coord = new Vector2(-1, -1);
    public bool collapsed = false;
    public WFC_Module_1 collapsedModule;

    public void Collapse()
    {
        collapsedModule = GetWeightedModule();
        Instantiate(collapsedModule, transform);
        Destroy(placeholderObj);
        collapsed = true;
    }

    WFC_Module_1 GetWeightedModule()
    {
        float totalRatio = 0;
        foreach (WFC_Module_1 pm in possibleModules)
            totalRatio += pm.probability;

        float weightedRandom = Random.Range(0, totalRatio);

        int weightedRandomIndex = 0;
        foreach (WFC_Module_1 pm in possibleModules)
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

    public WFC_Module_1[] RotateModules()
    {
        List<WFC_Module_1> rotatedPossibleModules = new List<WFC_Module_1>();

        foreach(WFC_Module_1 module in possibleModules)
        {
            rotatedPossibleModules.Add(module);

            if (module.rotated)
                continue;

            module.rotated = true;

            for(int i = 1; i < 4; i++)
            {
                WFC_Module_1 moduleInstance = Instantiate(module);
                moduleInstance.RotateModule(i);

                string path = "Assets/Prefabs/WFC/Test 1/" + module.name + "-R" + i + ".prefab";
                GameObject rotatedPrefab = PrefabUtility.SaveAsPrefabAsset(moduleInstance.gameObject, path);

                rotatedPossibleModules.Add(rotatedPrefab.GetComponent<WFC_Module_1>());
            }
        }

        return rotatedPossibleModules.ToArray();
    }
}

[CustomEditor(typeof(WFC_Slot_1))]
public class WFC_Slot_1Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        WFC_Slot_1 myTarget = (WFC_Slot_1)target;

        if (GUILayout.Button("Create Rotated Variants"))
            myTarget.possibleModules = myTarget.RotateModules();
    }
}