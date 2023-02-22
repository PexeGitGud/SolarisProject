using UnityEngine;
using System.Collections.Generic;

public class WFC_Map : MonoBehaviour
{
    [SerializeField] int mapSizeX = 16, mapSizeY = 16;
    [SerializeField] int tileSize = 1;
    [SerializeField] WFC_Slot slotPrefab;

    [SerializeField] WFC_Slot[,] map;

    public enum CollapseMode
    {
        Gradual,
        Instantaneous,
        Manual
    }

    [SerializeField] CollapseMode collapseMode;

    bool stopUpdate = true;

    void Update()
    {
        if (stopUpdate)
            return;

        switch(collapseMode)
        {
            case CollapseMode.Gradual:
                stopUpdate = !CollapseLowestEntropy(map);
                break;
            case CollapseMode.Instantaneous:
                while (CollapseLowestEntropy(map)) ;
                stopUpdate = true;
                break; 
            case CollapseMode.Manual:
                if (Input.anyKey)
                    stopUpdate = !CollapseLowestEntropy(map);
                break;
        }
    }

    public void Startup()
    {
        DestroyMap(map);
        map = CreateNewEmptyMap(mapSizeX, mapSizeY, tileSize, slotPrefab);
        stopUpdate = false;
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
            return false;

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
            map[coordX, coordY + 1].possibleModules = UpdatePossibleModules(map[coordX, coordY + 1].possibleModules, 0, collapsedSlot.collapsedModule.N_Connector);
        //right
        if (coordX + 1 < map.GetLength(0))
            map[coordX + 1, coordY].possibleModules = UpdatePossibleModules(map[coordX + 1, coordY].possibleModules, 1, collapsedSlot.collapsedModule.E_Connector);
        //down
        if (coordY - 1 >= 0)
            map[coordX, coordY - 1].possibleModules = UpdatePossibleModules(map[coordX, coordY - 1].possibleModules, 2, collapsedSlot.collapsedModule.S_Connector);
        //left
        if (coordX - 1 >= 0)
            map[coordX - 1, coordY].possibleModules = UpdatePossibleModules(map[coordX - 1, coordY].possibleModules, 3, collapsedSlot.collapsedModule.W_Connector);
    }

    WFC_Module[] UpdatePossibleModules(WFC_Module[] possibleModules, int coord, Connector connector)
    {
        List<WFC_Module> updatedModulesList= new List<WFC_Module>();

        switch (coord)
        {
            case 0:
                foreach (WFC_Module module in possibleModules)
                    if (module.S_Connector == connector)
                        updatedModulesList.Add(module);
                break;
            case 1:
                foreach (WFC_Module module in possibleModules)
                    if (module.W_Connector == connector)
                        updatedModulesList.Add(module);
                break;

            case 2:
                foreach (WFC_Module module in possibleModules)
                    if (module.N_Connector == connector)
                        updatedModulesList.Add(module);
                break;

            case 3:
                foreach (WFC_Module module in possibleModules)
                    if (module.E_Connector == connector)
                        updatedModulesList.Add(module);
                break;
        }

        return updatedModulesList.ToArray();
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

    public void SetMapSizeX(int newMapSizeX) => mapSizeX = newMapSizeX; 
    public void SetMapSizeY(int newMapSizeY) => mapSizeY = newMapSizeY; 
    public void SetCollapseMode(CollapseMode newMode) => collapseMode = newMode; 
}