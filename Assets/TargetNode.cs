using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
    public GameObject grassContainer;
    public GameObject parentNode;
    public GameObject nodeArray;
    public Material selectionMaterial;
    private List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;
    private NodeScript parentNodeScript;
    private NodeArrayScript nodeArrayScript;

    // Start is called before the first frame update
    void Start()
    {

        grassContainer = gameObject.transform.parent.gameObject;

        // node reference - parent reference
        parentNode = grassContainer.transform.parent.gameObject;
        parentNodeScript = parentNode.GetComponent<NodeScript>();
        
        // board array reference - highest parent
        nodeArray = parentNode.transform.parent.gameObject;
        nodeArrayScript = nodeArray.GetComponent<NodeArrayScript>();

        tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();
        
        foreach(Renderer renderer in tileRendererList)
        {
            tileMaterialList.Add(renderer.material);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        foreach(Renderer renderer in tileRendererList)
        {
            renderer.material = selectionMaterial;
        }

        Debug.Log(parentNode.name);

        Debug.Log("OnMouseEnter Activated on " + gameObject.name);
        
        // Material Debuggeer for checking material display - Disabled 08222024.2059 
        // foreach(Material material in tileMaterialList)
        // {
        //     Debug.Log($"{material}");
        // }
    }


    private void OnMouseExit()
    {
        int colorCounter = 0;
        foreach(Renderer renderer in tileRendererList)
        {
            renderer.material = tileMaterialList[colorCounter];
            colorCounter++;
        }
    }

    // To be called in the TargetNode script from parentNodeScript
    public void OnMouseDown()
    {
        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(0))
        {
            parentNodeScript.BlackSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();
            List<int> nodeValueMap = nodeArrayScript.NodeValueMapper();
            nodeArrayScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("Object clicked!");
        }

        // Check if the left mouse button was clicked
        if (Input.GetMouseButtonDown(1))
        {
            parentNodeScript.WhiteSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();
            List<int> nodeValueMap = nodeArrayScript.NodeValueMapper();
            nodeArrayScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("Object clicked!");
        }
    }

}
