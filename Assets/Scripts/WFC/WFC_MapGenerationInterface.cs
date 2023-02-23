using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class WFC_MapGenerationInterface : MonoBehaviour
{
    [field: SerializeField]
    private TMP_InputField mapSizeX_IF, mapSizeY_IF;
    [field: SerializeField]
    private Button generateMap_Button;
    [field: SerializeField]
    private TMP_Dropdown collapseMode_Dropdown;
    [field: SerializeField]
    private Button toggle;
    [field: SerializeField]
    private Vector2 normalPanelSize, minimizedPanelSize;
    [field: SerializeField]
    private GameObject options;
    [field: SerializeField]
    private WFC_Map wfcMap;
    [field: SerializeField]
    private Camera mainCamera;
    [field: SerializeField]
    bool minimized = false;

    void Start()
    {
        mapSizeX_IF.onEndEdit.AddListener(OnMapSizeXChanged);
        mapSizeY_IF.onEndEdit.AddListener(OnMapSizeYChanged);
        generateMap_Button.onClick.AddListener(OnGenerateMap);
        collapseMode_Dropdown.onValueChanged.AddListener(OnCollapseModeChanged);
        toggle.onClick.AddListener(OnToggle);
    }

    void OnMapSizeXChanged(string newXValue)
    {
        wfcMap.SetMapSizeX(int.Parse(newXValue));
    }
    void OnMapSizeYChanged(string newYValue)
    {
        wfcMap.SetMapSizeY(int.Parse(newYValue));
    }
    void OnGenerateMap()
    {
        int dist = Math.Max(int.Parse(mapSizeX_IF.text), int.Parse(mapSizeY_IF.text));
        mainCamera.transform.position = new Vector3(0, dist, 0);

        wfcMap.Startup();
    }
    void OnCollapseModeChanged(int newMode)
    {
        wfcMap.SetCollapseMode((WFC_Map.CollapseMode) newMode);
    }
    void OnToggle()
    {
        options.SetActive(!(minimized = !minimized));
        toggle.transform.rotation = Quaternion.Euler(0, 0, minimized ? 180 : 0);

        GetComponent<RectTransform>().sizeDelta = minimized ? minimizedPanelSize : normalPanelSize;
    }
}