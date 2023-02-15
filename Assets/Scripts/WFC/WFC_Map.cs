using UnityEngine;
using System.Collections.Generic;

public class WFC_Map : MonoBehaviour
{
    [SerializeField] int mapSize = 16;
    [SerializeField] int tileSize = 1;
    [SerializeField] WFC_Slot slotPrefab;

    [SerializeField] WFC_Slot[,] map;

    bool autoCollapse = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Startup();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyMap(map);
            map = null;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CollapseLowestEntropy(map);
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            autoCollapse = !autoCollapse;
        }

        if(autoCollapse)
        {
            CollapseLowestEntropy(map);
        }
    }

    void Startup()
    {
        DestroyMap(map);
        map = CreateNewEmptyMap(mapSize, mapSize, tileSize, slotPrefab);
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

    void CollapseLowestEntropy(WFC_Slot[,] map)
    {
        if (map == null)
            return;

        WFC_Slot slot = GetLowestEntropySlot(map);

        if (slot)
        {
            if (slot.possibleModules.Length == 0)
            {
                //implement a backtracking in oreder to redo some steps in order for this not be an issue
                Debug.LogWarning("Slot: " + slot.name + " have 0 possible modules - Please Start Over...");
                return;
            }

            WFC_Module collapsedModule = slot.possibleModules[Random.Range(0, slot.possibleModules.Length)];
            slot.Collapse(collapsedModule);
            NeightbourPossibleModulesReduction(map, slot);
        }
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

    void NeightbourPossibleModulesReduction(WFC_Slot[,] map, WFC_Slot collapsedSlot) //propagate
    {
        //for(int i = (int)collapsedSlot.coord.x - 1; i <= (int)collapsedSlot.coord.x + 1; i++)
        //{
        //    for (int j = (int)collapsedSlot.coord.y - 1; j <= (int)collapsedSlot.coord.y + 1; j++)
        //    {
        //        if (i < 0 || j < 0 || i >= map.GetLength(0) || j >= map.GetLength(1))
        //            continue;

        //        //Debug.Log(i + "/" + j);
        //        if (map[i, j].collapsed)
        //            continue;

        //        map[i, j].possibleModules = UpdatePossibleModules(map[i,j].possibleModules, collapsedSlot.collapsedModule.possibleNeighbors);
        //    }
        //}

        //up
        if(collapsedSlot.coord.y + 1 < map.GetLength(1))
            map[(int)collapsedSlot.coord.x, (int)collapsedSlot.coord.y + 1].possibleModules = UpdatePossibleModules(map[(int)collapsedSlot.coord.x, (int)collapsedSlot.coord.y + 1].possibleModules, collapsedSlot.collapsedModule.possibleNeighbors);
        //right
        if (collapsedSlot.coord.x + 1 < map.GetLength(0))
            map[(int)collapsedSlot.coord.x + 1, (int)collapsedSlot.coord.y].possibleModules = UpdatePossibleModules(map[(int)collapsedSlot.coord.x + 1, (int)collapsedSlot.coord.y].possibleModules, collapsedSlot.collapsedModule.possibleNeighbors);
        //down
        if (collapsedSlot.coord.y - 1 >= 0)
            map[(int)collapsedSlot.coord.x, (int)collapsedSlot.coord.y - 1].possibleModules = UpdatePossibleModules(map[(int)collapsedSlot.coord.x, (int)collapsedSlot.coord.y - 1].possibleModules, collapsedSlot.collapsedModule.possibleNeighbors);
        //left
        if (collapsedSlot.coord.x - 1 >= 0)
            map[(int)collapsedSlot.coord.x - 1, (int)collapsedSlot.coord.y].possibleModules = UpdatePossibleModules(map[(int)collapsedSlot.coord.x - 1, (int)collapsedSlot.coord.y].possibleModules, collapsedSlot.collapsedModule.possibleNeighbors);
    }

    WFC_Module[] UpdatePossibleModules(WFC_Module[] oldModules, WFC_Module[] newModules)
    {
        List<WFC_Module> updatedModulesList= new List<WFC_Module>();

        for (int i = 0; i < oldModules.Length; i++)
        {
            for (int j = 0; j < newModules.Length; j++)
            {
                if (oldModules[i].moduleName == newModules[j].moduleName)
                {
                    updatedModulesList.Add(oldModules[i]);
                }
            }
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
}