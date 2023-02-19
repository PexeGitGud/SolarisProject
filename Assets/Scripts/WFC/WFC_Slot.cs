using UnityEngine;

public class WFC_Slot : MonoBehaviour
{
    public GameObject placeholderObj;
    public WFC_Module[] possibleModules;
    public Vector2 coord = new Vector2(-1, -1);
    public bool collapsed = false;
    public WFC_Module collapsedModule;

    public void Collapse(WFC_Module collapsedModule)
    {
        collapsed = true;
        this.collapsedModule = Instantiate(collapsedModule, transform);
        Destroy(placeholderObj);
    }

    public void TurnRed()
    {
        placeholderObj.GetComponent<Renderer>().material.color = Color.red;
    }
}