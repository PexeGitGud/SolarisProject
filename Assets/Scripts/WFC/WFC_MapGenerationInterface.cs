using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WFC_MapGenerationInterface : MonoBehaviour
{
    public TMP_InputField mapSizeX_IF, mapSizeY_IF;
    public Button generateMap_Button;
    public TMP_Dropdown collapseMode_Dropdown;
    public WFC_Map WFC_Map;

    void Start()
    {
        mapSizeX_IF.onEndEdit.AddListener(OnMapSizeXChanged);
        mapSizeY_IF.onEndEdit.AddListener(OnMapSizeYChanged);
        generateMap_Button.onClick.AddListener(WFC_Map.Startup);
        collapseMode_Dropdown.onValueChanged.AddListener(OnCollapseModeChanged);
    }

    void OnMapSizeXChanged(string newXValue)
    {
        WFC_Map.SetMapSizeX(int.Parse(newXValue));
    }
    void OnMapSizeYChanged(string newYValue)
    {
        WFC_Map.SetMapSizeY(int.Parse(newYValue));
    }
    void OnCollapseModeChanged(int newMode)
    {
        WFC_Map.SetCollapseMode((WFC_Map.CollapseMode) newMode);
    }
}