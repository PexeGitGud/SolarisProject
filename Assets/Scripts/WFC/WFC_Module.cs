using UnityEngine;

public class WFC_Module : MonoBehaviour
{
    public string moduleName;
    public WFC_Module[] possibleNeighbors_N;
    public WFC_Module[] possibleNeighbors_E;
    public WFC_Module[] possibleNeighbors_S;
    public WFC_Module[] possibleNeighbors_W;
    public float probability = 0.5f;
}