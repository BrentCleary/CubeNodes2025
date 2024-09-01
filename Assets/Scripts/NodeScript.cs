using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

    //* ---------------------------------------- PROPERTIES ----------------------------------------
    public int nodeValue;
    public List<int> nodeValueList = new List<int> { 0, 1, 2, 3, 4 };   // Node Values when not occupied
    public List<GameObject> GrassTileList = new List<GameObject> {};
    
    public int sheepValue;
    public List<int> sheepValueList = new List<int> {0, 1, 2};    // { emptySpace, sheepBlack, sheepWhite }
    public List<GameObject> sheepTileList = new List<GameObject> {};   // { emptySpace, sheepBlack, sheepWhite }

    public int libertyValue;
    public List<int> libertyValueList = new List<int> { 0, 1 };          //  LibertyValue{ 1 , 0 }
    
    public bool placeAbleBool;
    public List<bool> placeAbleValueList = new List<bool> { false, true };   // is node placeable for current player

    public int[] arrayPosition = new int[2];


    //* SHEEP GROUP PARAMETERS
    // Adjacent Nodes
    public GameObject leftNode;
    public GameObject rightNode;
    public GameObject topNode;
    public GameObject bottomNode;
    // Adjacent Node Scripts
    public NodeScript leftNodeScript;
    public NodeScript rightNodeScript;
    public NodeScript bottomNodeScript;
    public NodeScript topNodeScript;

    public int leftNodeLibertyVal;
    public int rightNodeLibertyVal;
    public int bottomNodeLibertyVal;
    public int topNodeLibertyVal;

    public List<Node> sheepGroupList;

    public int groupID;


    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                     //* Sets Initial Node Values to Default on Creation 
    // Start is called before the first frame update
    void Start()
    {
        nodeValue = nodeValueList[4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
        libertyValue = 1;
        placeAbleBool = true;
        sheepValue = sheepValueList[0];
    }

    // Update is called once per frame
    void Update()
    {
        // NodeValueSetterDebug();
    }





    //* ---------------------------------------- GRASS TILE DISPLAY METHODS ----------------------------------------
                                       //* Sets Grass Tile  on/off based on nodeValue 
    public void SetGrassTileDisplayLoop()
    {
        bool isActive = true;       // Sets initialize bool

        for (int i = nodeValue; i >= 0; i--)    // Sets all tiles from nodeValue and lower true
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[i].SetActive(isActive);

        }
        for (int i = nodeValueList.Count- 1; i > nodeValue; i--)   // Sets all tiles above nodeValue false
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[i].SetActive(!isActive);
        }
    }





    // *---------------------------------------- SHEEP SETTER  METHODS ----------------------------------------
                                          //* Called in TargetNode Script 

    public void BlackSheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[1];         // Set sheep value to blackSheep index
        libertyValue = libertyValueList[0];     // Set libertyValue to 0
        nodeValue = nodeValueList[0];           // nodeValue is 0

        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].SetActive(!isActive);      // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].SetActive(isActive);      // Set Current SheepTile active
        // Debug.Log(gameObject.name + " is " + sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled);

    }

    public void WhiteSheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[2];         // Set sheep value to whiteSheep index
        libertyValue = libertyValueList[0];     // Set libertyValue to 0
        nodeValue = nodeValueList[0];           // nodeValue is 0

        // Sets all SheepTiles to inactive
        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].SetActive(!isActive);      // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].SetActive(isActive);      // Set Current SheepTile active 
        // Debug.Log(gameObject.name + " is " + sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled);
    }


    public void EmptySheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[0];         // Set sheep value to emptySheep index
        libertyValue = libertyValueList[1];     // Set libertyValue to 1
        nodeValue = nodeValueList[4];           // nodeValue is reset to 4

        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].SetActive(!isActive);      // Sets all SheepTiles to inactive      // Sets all SheepTiles to inactive    // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].SetActive(isActive);      // Set Current SheepTile active 
        // Debug.Log(gameObject.name + " is " + sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled);
    }





    // *---------------------------------------- SHEEP GROUP METHODS ----------------------------------------
                                //* Loops over gNodeArray and adjacent sheep in groups 
    // public void SheepNodeGroupSetter()
    // {
    //     if(leftNodeScript != null)
    //     {
    //         if(leftNodeScript.sheepValue == sheepValue)
    //         {
    //             leftNodeScript.sheepGroupList.Add(gameObject);
    //         }
    //     }
    // }


    // *---------------------------------------- DEBUG METHODS ----------------------------------------
                                          //* Called in Update Method 

    // Test Method for Node Value Setting - Disabled in Update
    public void NodeValueSetterDebug()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0)){  // Press 0 on Keypad to set nodeValue to 0
            Debug.Log("Button Pressed = 0");
            nodeValue = nodeValueList[0];
        }

        if(Input.GetKeyDown(KeyCode.Keypad1)){  // Press 1 on Keypad to set nodeValue to 1
            nodeValue = nodeValueList[1];
            Debug.Log("Button Pressed = 1");
        }

        if(Input.GetKeyDown(KeyCode.Keypad2)){  // Press 2 on Keypad to set nodeValue to 2
            nodeValue = nodeValueList[2];
            Debug.Log("Button Pressed = 2");
        }

        if(Input.GetKeyDown(KeyCode.Keypad3)){  // Press 3 on Keypad to set nodeValue to 3
            nodeValue = nodeValueList[3];
            Debug.Log("Button Pressed = 3");
        }

        if(Input.GetKeyDown(KeyCode.Keypad4)){  // Press 4 on Keypad to set nodeValue to 4
            nodeValue = nodeValueList[4];
            Debug.Log("Button Pressed = 4");
        }
    }

}
