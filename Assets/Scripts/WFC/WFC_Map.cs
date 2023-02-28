using UnityEngine;
using System.Collections.Generic;

public class WFC_Map : MonoBehaviour
{
    [field: SerializeField]
    private int mapSizeX = 16, mapSizeY = 16;
    [field: SerializeField]
    private int tileSize = 1;
    [field: SerializeField]
    private WFC_Slot slotPrefab;

    [field: SerializeField]
    private WFC_Slot[,] map;

    public enum CollapseMode
    {
        Gradual,
        Instantaneous,
        Manual
    }

    [field: SerializeField] 
    private CollapseMode collapseMode;

    bool stopCollapsing = true;
    bool mapCollapsed = false;
    bool mapDressingCollapsed = false;

    void Update()
    {
        if(mapCollapsed && !mapDressingCollapsed)
        {
            CollapseModuleDressings(map);
        }

        if (stopCollapsing)
            return;

        CollapseModules(map);
    }

    public void Startup()
    {
        DestroyMap(map);
        map = CreateNewEmptyMap(mapSizeX, mapSizeY, tileSize, slotPrefab);
        stopCollapsing = mapCollapsed = mapDressingCollapsed = false;
    }

    WFC_Slot[,] CreateNewEmptyMap(int mapSizeX, int mapSizeY, int tileSize, WFC_Slot slotPrefab)
    {
        WFC_Slot[,] newMap = new WFC_Slot[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                Vector3 offset = new Vector3(transform.position.x + tileSize * x - (mapSizeX / 2) + (tileSize / 2.0f), 0, transform.position.z + tileSize * y - (mapSizeY / 2) + (tileSize / 2.0f));

                WFC_Slot newSlot = Instantiate(slotPrefab, offset, Quaternion.identity, transform);
                newSlot.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                newSlot.coord = new Vector2Int(x, y);
                newSlot.name = x + "-" + y;

                newMap[x, y] = newSlot;
            }
        }

        return newMap;
    }

    #region Module
    void CollapseModules(WFC_Slot[,] map)
    {
        switch (collapseMode)
        {
            case CollapseMode.Gradual:
                stopCollapsing = !CollapseLowestEntropy(map);
                break;
            case CollapseMode.Instantaneous:
                while (CollapseLowestEntropy(map)) ;
                stopCollapsing = true;
                break;
            case CollapseMode.Manual:
                if (Input.anyKey)
                    stopCollapsing = !CollapseLowestEntropy(map);
                break;
        }
    }

    bool CollapseLowestEntropy(WFC_Slot[,] map)
    {
        if (map == null)
            return false;

        WFC_Slot slot = GetLowestEntropySlot(map);
        if (!slot)
        {
            mapCollapsed = true;
            return false;
        }

        if (slot.possibleModules.Length == 0)
        {
            slot.TurnRed();
            Debug.LogWarning("Slot: " + slot.name + " have 0 possible modules - Starting Over...");
            Invoke("Startup", 2.0f);
            return false;
        }

        slot.CollapseModule();
        PropagatePossibleNeighbors(map, slot);

        return true;
    }

    WFC_Slot GetLowestEntropySlot(WFC_Slot[,] map)
    {
        List<WFC_Slot> lowestEntropySlotList = new List<WFC_Slot>();
        int lowestEntropy = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].collapsed)
                    continue;

                if (lowestEntropy == 0)
                    lowestEntropy = map[i, j].possibleModules.Length;

                if (lowestEntropy > map[i, j].possibleModules.Length)
                {
                    lowestEntropy = map[i, j].possibleModules.Length;
                    lowestEntropySlotList.Clear();
                }

                if (lowestEntropy == map[i, j].possibleModules.Length)
                {
                    lowestEntropySlotList.Add(map[i, j]);
                }
            }
        }

        if (lowestEntropySlotList.Count > 0)
            return lowestEntropySlotList[Random.Range(0, lowestEntropySlotList.Count)];

        return null;
    }

    void PropagatePossibleNeighbors(WFC_Slot[,] map, WFC_Slot collapsedSlot) //propagate
    {
        int coordX = collapsedSlot.coord.x;
        int coordY = collapsedSlot.coord.y;

        //up
        if (coordY + 1 < map.GetLength(1)) 
        {
            List<WFC_Module> updatedModulesList = new List<WFC_Module>();

            foreach (WFC_Module module in map[coordX, coordY + 1].possibleModules)
                if (module.groundConnectors.S_Connector == collapsedSlot.collapsedModule.groundConnectors.N_Connector)
                    updatedModulesList.Add(module);

            map[coordX, coordY + 1].possibleModules = updatedModulesList.ToArray();
        }
        //right
        if (coordX + 1 < map.GetLength(0))
        {
            List<WFC_Module> updatedModulesList = new List<WFC_Module>();

            foreach (WFC_Module module in map[coordX + 1, coordY].possibleModules)
                if (module.groundConnectors.W_Connector == collapsedSlot.collapsedModule.groundConnectors.E_Connector)
                    updatedModulesList.Add(module);

            map[coordX + 1, coordY].possibleModules = updatedModulesList.ToArray();
        }
        //down
        if (coordY - 1 >= 0) 
        {
            List<WFC_Module> updatedModulesList = new List<WFC_Module>();

            foreach (WFC_Module module in map[coordX, coordY - 1].possibleModules)
                if (module.groundConnectors.N_Connector == collapsedSlot.collapsedModule.groundConnectors.S_Connector)
                    updatedModulesList.Add(module);

            map[coordX, coordY - 1].possibleModules = updatedModulesList.ToArray();
        }
        //left
        if (coordX - 1 >= 0)
        {
            List<WFC_Module> updatedModulesList = new List<WFC_Module>();

            foreach (WFC_Module module in map[coordX - 1, coordY].possibleModules)
                if (module.groundConnectors.E_Connector == collapsedSlot.collapsedModule.groundConnectors.W_Connector)
                    updatedModulesList.Add(module);

            map[coordX - 1, coordY].possibleModules = updatedModulesList.ToArray();
        }
    }

    void DestroyMap(WFC_Slot[,] map)
    {
        if (map == null)
            return;

        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                Destroy(map[i, j].gameObject);
            }
        }
    }
    #endregion

    #region Module Dressing
    void CollapseModuleDressings(WFC_Slot[,] map)
    {
        switch (collapseMode)
        {
            case CollapseMode.Gradual:
                mapDressingCollapsed = !CollapseLowestDressingEntropy(map);
                break;
            case CollapseMode.Instantaneous:
                while (CollapseLowestDressingEntropy(map)) ;
                mapDressingCollapsed = true;
                break;
            case CollapseMode.Manual:
                if (Input.anyKey)
                    mapDressingCollapsed = !CollapseLowestDressingEntropy(map);
                break;
        }
    }

    bool CollapseLowestDressingEntropy(WFC_Slot[,] map)
    {
        if (map == null)
            return false;

        WFC_Slot slot = GetLowestEntropyDressingSlot(map);
        if(!slot) 
            return false;

        if (slot.possibleModuleDressings.Length == 0)
        {
            slot.TurnRed();
            Debug.LogWarning("Slot: " + slot.name + " have 0 possible modules dressings - Starting Over...");
            Invoke("Startup", 2.0f);
            return false;
        }

        slot.CollapseModuleDressing();
        PropagatePossibleDressingNeighbors(map, slot);

        return true;
    }

    WFC_Slot GetLowestEntropyDressingSlot(WFC_Slot[,] map)
    {
        List<WFC_Slot> lowestEntropySlotList = new List<WFC_Slot>();
        int lowestEntropy = 0;

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].moduleDressed)
                    continue;

                if (lowestEntropy == 0)
                    lowestEntropy = map[i, j].possibleModuleDressings.Length;

                if (lowestEntropy > map[i, j].possibleModuleDressings.Length)
                {
                    lowestEntropy = map[i, j].possibleModuleDressings.Length;
                    lowestEntropySlotList.Clear();
                }

                if (lowestEntropy == map[i, j].possibleModuleDressings.Length)
                {
                    lowestEntropySlotList.Add(map[i, j]);
                }
            }
        }

        if (lowestEntropySlotList.Count > 0)
            return lowestEntropySlotList[Random.Range(0, lowestEntropySlotList.Count)];

        return null;
    }

    void PropagatePossibleDressingNeighbors(WFC_Slot[,] map, WFC_Slot collapsedSlot)
    {
        int coordX = collapsedSlot.coord.x;
        int coordY = collapsedSlot.coord.y;

        //up
        if (coordY + 1 < map.GetLength(1))
        {
            List<WFC_ModuleDressing> updatedModuleDressingsList = new List<WFC_ModuleDressing>();

            foreach (WFC_ModuleDressing moduleDressing in map[coordX, coordY + 1].possibleModuleDressings)
                if (moduleDressing.dressingConnectors.S_Connector == collapsedSlot.selectedModuleDressing.dressingConnectors.N_Connector)
                    updatedModuleDressingsList.Add(moduleDressing);

            map[coordX, coordY + 1].possibleModuleDressings = updatedModuleDressingsList.ToArray();
        }
        //right
        if (coordX + 1 < map.GetLength(0))
        {
            List<WFC_ModuleDressing> updatedModulesList = new List<WFC_ModuleDressing>();

            foreach (WFC_ModuleDressing moduleDressing in map[coordX + 1, coordY].possibleModuleDressings)
                if (moduleDressing.dressingConnectors.W_Connector == collapsedSlot.selectedModuleDressing.dressingConnectors.E_Connector)
                    updatedModulesList.Add(moduleDressing);

            map[coordX + 1, coordY].possibleModuleDressings = updatedModulesList.ToArray();
        }
        //down
        if (coordY - 1 >= 0)
        {
            List<WFC_ModuleDressing> updatedModulesList = new List<WFC_ModuleDressing>();

            foreach (WFC_ModuleDressing moduleDressing in map[coordX, coordY - 1].possibleModuleDressings)
                if (moduleDressing.dressingConnectors.N_Connector == collapsedSlot.selectedModuleDressing.dressingConnectors.S_Connector)
                    updatedModulesList.Add(moduleDressing);

            map[coordX, coordY - 1].possibleModuleDressings = updatedModulesList.ToArray();
        }
        //left
        if (coordX - 1 >= 0)
        {
            List<WFC_ModuleDressing> updatedModulesList = new List<WFC_ModuleDressing>();

            foreach (WFC_ModuleDressing moduleDressing in map[coordX - 1, coordY].possibleModuleDressings)
                if (moduleDressing.dressingConnectors.E_Connector == collapsedSlot.selectedModuleDressing.dressingConnectors.W_Connector)
                    updatedModulesList.Add(moduleDressing);

            map[coordX - 1, coordY].possibleModuleDressings = updatedModulesList.ToArray();
        }
    }
    #endregion

    public void SetMapSizeX(int newMapSizeX) => mapSizeX = newMapSizeX; 
    public void SetMapSizeY(int newMapSizeY) => mapSizeY = newMapSizeY; 
    public void SetCollapseMode(CollapseMode newMode) => collapseMode = newMode; 
}