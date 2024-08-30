using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
    //* ---------------------------------------- PROPERTIES ----------------------------------------
    public GameObject grassContainer;
    public GameObject parentNode;
    public GameObject nodeArray;

    public Material selectionMaterial;
    private List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;

    private NodeScript parentNodeScript;
    private BoardGenerator boardGeneratorScript;
    // private NodeArrayScript nodeArrayScript; // Replaced by boardGeneratorScript

    public bool nodeSelected = false;





    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                 //* Assigns Parent Object/Parent Scripts and GrassTileList 
                                          //* Calls Placement Method on Nodes 
    // Start is called before the first frame update
    void Start()
    {

        grassContainer = gameObject.transform.parent.gameObject;

        // node reference - parent reference
        parentNode = grassContainer.transform.parent.gameObject;
        parentNodeScript = parentNode.GetComponent<NodeScript>();
        
        // board array reference - highest parent
        nodeArray = parentNode.transform.parent.gameObject;
        boardGeneratorScript = nodeArray.GetComponent<BoardGenerator>();

        tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();
        
        foreach(Renderer renderer in tileRendererList) {
            tileMaterialList.Add(renderer.material);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        PlaceBlackSheepMethod();
        PlaceWhiteSheepMethod();
    }





    //* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
                            //* Highlights/Resets selected Nodes Color by changing GrassTiles materials 
    private void OnMouseEnter()
    {
        nodeSelected = true;

        foreach(Renderer renderer in tileRendererList) {
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
        nodeSelected = false;
        int colorCounter = 0;

        foreach(Renderer renderer in tileRendererList) {
            renderer.material = tileMaterialList[colorCounter];
            colorCounter++;
        }
    }




    //* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    //* Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                //* Calls BoardGeneratorScript and NodeScript 
    public void PlaceBlackSheepMethod()
    {
        // Check if the left mouse button was clicked
        if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse0))
        {
            parentNodeScript.BlackSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            // BoardGenerator Script
            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("Black Sheep Set");
        }
    }

    public void PlaceWhiteSheepMethod()
    {
        // Check if the right mouse button was clicked
        if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse1))
        {
            parentNodeScript.WhiteSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            // BoardGenerator Script
            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("White Sheep Set!");
        }
    }

    public void PlaceEmptySheepMethod()
    {
        // Check if the middle mouse button was clicked
        if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse2))
        {
            parentNodeScript.EmptySheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("Empty Sheep Set!");
        }
    }

}
