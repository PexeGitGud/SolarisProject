using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WFC_Slot : MonoBehaviour
{
    [field: SerializeField] 
    private GameObject placeholderObj;
    public Vector2Int coord = new Vector2Int(-1, -1);

    [field: Space(5), Header("Modules")]
    public WFC_Module[] possibleModules;
    [field: SerializeField]
    public bool collapsed { get; private set; } = false;
    [field: SerializeField]
    public WFC_Module collapsedModule { get; private set; }

    [field: Space(5), Header("Module Dressings")]
    public WFC_ModuleDressing[] possibleModuleDressings;
    [field: SerializeField]
    public bool moduleDressed { get; private set; } = false;
    [field: SerializeField]
    public WFC_ModuleDressing selectedModuleDressing { get; private set; }

    public void CollapseModule()
    {
        collapsedModule = Instantiate(GetWeightedModule(), transform);
        UpdatePossibleModuleDressings();
        collapsed = true;
        Destroy(placeholderObj);
    }

    WFC_Module GetWeightedModule()
    {
        float n = possibleModules.Length;

        float totalRatio = 0;
        foreach (WFC_Module pm in possibleModules)
            totalRatio += n * pm.probabilityPercent;

        float weightedRandom = Random.Range(0, totalRatio);

        int weightedRandomIndex = 0;
        foreach (WFC_Module pm in possibleModules)
        {
            if ((weightedRandom -= n * pm.probabilityPercent) <= 0.0f)
                break;
            weightedRandomIndex++;
        }

        return possibleModules[weightedRandomIndex];
    }

    void UpdatePossibleModuleDressings()
    {
        List<WFC_ModuleDressing> updatedModulesList = new List<WFC_ModuleDressing>();

        foreach(WFC_ModuleDressing moduleDressing in possibleModuleDressings) 
        {
            foreach(GroundConnector groundConnector in moduleDressing.possibleSlotConnectors)
            {
                if (groundConnector == collapsedModule.slotConnector)
                    updatedModulesList.Add(moduleDressing);
            }
        }

        possibleModuleDressings = updatedModulesList.ToArray();
    }

    public void CollapseModuleDressing()
    {
        selectedModuleDressing = Instantiate(GetWeightedModuleDressing(), transform);
        moduleDressed = true;
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

    public void TurnRed()
    {
        placeholderObj.GetComponent<Renderer>().material.color = Color.red;
    }

    public WFC_Module[] RotateModules()
    {
        List<WFC_Module> rotatedPossibleModules = new List<WFC_Module>();

        foreach (WFC_Module module in possibleModules)
        {
            rotatedPossibleModules.Add(module);

            if (module.rotated)
                continue;

            module.rotated = true;

            for (int i = 1; i < 4; i++)
            {
                WFC_Module moduleInstance = Instantiate(module);
                moduleInstance.RotateModule(i);

                string path = "Assets/Prefabs/WFC/Modules/" + module.name + "-R" + i + ".prefab";
                GameObject rotatedPrefab = PrefabUtility.SaveAsPrefabAsset(moduleInstance.gameObject, path);

                rotatedPossibleModules.Add(rotatedPrefab.GetComponent<WFC_Module>());
            }
        }

        return rotatedPossibleModules.ToArray();
    }

    public WFC_ModuleDressing[] RotateModuleDressings()
    {
        List<WFC_ModuleDressing> rotatedPossibleModuleDressings = new List<WFC_ModuleDressing>();

        foreach (WFC_ModuleDressing moduleDressing in possibleModuleDressings)
        {
            rotatedPossibleModuleDressings.Add(moduleDressing);

            if (moduleDressing.rotated)
                continue;

            moduleDressing.rotated = true;

            for (int i = 1; i < 4; i++)
            {
                WFC_ModuleDressing moduleDressingInstance = Instantiate(moduleDressing);
                moduleDressingInstance.RotateModuleDressing(i);

                string path = "Assets/Prefabs/WFC/Modules/ModuleDressings/" + moduleDressing.name + "-R" + i + ".prefab";
                GameObject rotatedPrefab = PrefabUtility.SaveAsPrefabAsset(moduleDressingInstance.gameObject, path);

                rotatedPossibleModuleDressings.Add(rotatedPrefab.GetComponent<WFC_ModuleDressing>());
            }
        }

        return rotatedPossibleModuleDressings.ToArray();
    }
}

[CustomEditor(typeof(WFC_Slot))]
public class WFC_SlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        WFC_Slot myTarget = (WFC_Slot)target;

        if (GUILayout.Button("Create Rotated Module Variants"))
            myTarget.possibleModules = myTarget.RotateModules();
        if (GUILayout.Button("Create Rotated Module Dressing Variants"))
            myTarget.possibleModuleDressings = myTarget.RotateModuleDressings();
    }
}