using UnityEditor;
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

    //[field: SerializeField]
    //public DressingConnector[] possibleGroundConnectors { get; private set; }

    [field: SerializeField, Range(0.0f, 1.0f)]
    public float probabilityPercent { get; private set; } = 0.5f;

    [field: Header("Module Rotation")]
    [field: SerializeField]
    private int rotationID = 0;

    void Start()
    {
        Destroy(groundPlaceholder);
    }

    public void RotateModuleDressing(int newRotation)
    {
        rotationID = newRotation;

        for (int i = 0; i < rotationID; i++)
        {
            Connectors<DressingConnector> newConnectors = new Connectors<DressingConnector>();
            newConnectors.N_Connector = dressingConnectors.W_Connector;
            newConnectors.W_Connector = dressingConnectors.S_Connector;
            newConnectors.S_Connector = dressingConnectors.E_Connector;
            newConnectors.E_Connector = dressingConnectors.N_Connector;
            dressingConnectors = newConnectors;

            transform.Rotate(0, 90, 0);
        }
    }
}

[CustomEditor(typeof(WFC_ModuleDressing))]
public class WFC_ModuleDressingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);

        WFC_ModuleDressing myTarget = (WFC_ModuleDressing)target;

        if (GUILayout.Button("Create Rotated Variants"))
        {
            for (int i = 1; i < 4; i++)
            {
                WFC_ModuleDressing moduleDressingInstance = Instantiate(myTarget);
                moduleDressingInstance.RotateModuleDressing(i);

                string path = "Assets/Prefabs/WFC/Modules/ModuleDressings/" + myTarget.name + "-R" + i + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(moduleDressingInstance.gameObject, path);
            }
        }
    }
}