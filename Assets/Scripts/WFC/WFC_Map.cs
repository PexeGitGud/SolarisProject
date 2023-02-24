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

        switch(collapseMode)
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
                Vector3 offset = new Vector3(transform.position.x + tileSize * x - (mapSizeX / 2), 0, transform.position.z + tileSize * y - (mapSizeY / 2));

                WFC_Slot newSlot = Instantiate(slotPrefab, offset, Quaternion.identity, transform);
                newSlot.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
                newSlot.coord = new Vector2(x, y);
                newSlot.name = x + "-" + y;

                newMap[x, y] = newSlot;
            }
        }

        return newMap;
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
            Debug.LogWarning("Slot: " + slot.name + " have 0 possible modules - Please Start Over...");
            Invoke("Startup", 2.0f);
            return false;
        }

        slot.Collapse();
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
        int coordX = (int)collapsedSlot.coord.x;
        int coordY = (int)collapsedSlot.coord.y;

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

    void CollapseModuleDressings(WFC_Slot[,] map)
    {
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j].collapsedModule.moduleDressed)
                    continue;

                map[i, j].collapsedModule.InstatiateModuleDressing();
            }
        }

        mapDressingCollapsed = true;
    }

    public void SetMapSizeX(int newMapSizeX) => mapSizeX = newMapSizeX; 
    public void SetMapSizeY(int newMapSizeY) => mapSizeY = newMapSizeY; 
    public void SetCollapseMode(CollapseMode newMode) => collapseMode = newMode; 
}