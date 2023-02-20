using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class WFC_MapGenerationInterface : MonoBehaviour
{
    public TMP_InputField mapSizeX_IF, mapSizeY_IF;
    public Button generateMap_Button;
    public TMP_Dropdown collapseMode_Dropdown;
    public Button toggle;
    public Vector2 normalPanelSize, minimizedPanelSize;
    public GameObject options;
    public WFC_Map WFC_Map;
    public Camera mainCamera;

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
        WFC_Map.SetMapSizeX(int.Parse(newXValue));
    }
    void OnMapSizeYChanged(string newYValue)
    {
        WFC_Map.SetMapSizeY(int.Parse(newYValue));
    }
    void OnGenerateMap()
    {
        int dist = Math.Max(int.Parse(mapSizeX_IF.text), int.Parse(mapSizeY_IF.text));
        mainCamera.transform.position = new Vector3(0, dist, 0);

        WFC_Map.Startup();
    }
    void OnCollapseModeChanged(int newMode)
    {
        WFC_Map.SetCollapseMode((WFC_Map.CollapseMode) newMode);
    }
    void OnToggle()
    {
        options.SetActive(!(minimized = !minimized));
        toggle.transform.rotation = Quaternion.Euler(0, 0, minimized ? 180 : 0);

        GetComponent<RectTransform>().sizeDelta = minimized ? minimizedPanelSize : normalPanelSize;
    }
}