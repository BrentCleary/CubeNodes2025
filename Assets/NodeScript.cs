using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

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
    
    private List<string> transitionStatesList = new List<string> { "beingCaptured" };             // States for transition

    // TEMP VARS FOR TEST
    public bool settingState;
    public bool settingSheep;


    // Start is called before the first frame update
    void Start()
    {
        nodeValue = nodeValueList[4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
        libertyValue = 1;
        placeAbleBool = true;
        sheepValue = sheepValueList[0];

        foreach(GameObject tile in GrassTileList)
        {
            Debug.Log("GameObject is " + tile);
        }

        foreach(GameObject tile in sheepTileList)
        {
            Debug.Log("SheepObject is " + tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        NodeValueSetter();
    }

    // Test Method for Node Value Setting
    public void NodeValueSetter()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0)){  // Press 0 on Keypad to set nodeValue to 0
            settingState = true;
            Debug.Log("Button Pressed = 0 and settingState = " + settingState);
            nodeValue = nodeValueList[0];
        }

        if(Input.GetKeyDown(KeyCode.Keypad1)){  // Press 1 on Keypad to set nodeValue to 1
            settingState = true;
            nodeValue = nodeValueList[1];
            Debug.Log("Button Pressed = 1");
        }

        if(Input.GetKeyDown(KeyCode.Keypad2)){  // Press 2 on Keypad to set nodeValue to 2
            settingState = true;
            nodeValue = nodeValueList[2];
            Debug.Log("Button Pressed = 2");
        }

        if(Input.GetKeyDown(KeyCode.Keypad3)){  // Press 3 on Keypad to set nodeValue to 3
            settingState = true;
            nodeValue = nodeValueList[3];
            Debug.Log("Button Pressed = 3");
        }

        if(Input.GetKeyDown(KeyCode.Keypad4)){  // Press 4 on Keypad to set nodeValue to 4
            settingState = true;
            nodeValue = nodeValueList[4];
            Debug.Log("Button Pressed = 4");
        }
    }

    public void SetGrassTileDisplayLoop()
    {
        bool isActive = true;                   // Sets initialize bool

        if(settingState)
        {
            for (int i = nodeValue; i >= 0; i--)    // Sets all tiles from nodeValue and lower true
            {
                GrassTileList[i].GetComponent<MeshRenderer>().enabled = isActive;
            }
            for (int i = nodeValueList.Count- 1; i > nodeValue; i--)   // Sets all tiles from nodeValue and higher false
            {
                GrassTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;
            }
        }
        settingState = false;
    }


    public void BlackSheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[1];         // Set sheep value to blackSheep index
        libertyValue = libertyValueList[0];     // Set libertyValue to 0
        nodeValue = nodeValueList[0];           // nodeValue is 0

        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;      // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled = isActive;      // Set Current SheepTile active
    }

    public void WhiteSheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[2];         // Set sheep value to whiteSheep index
        libertyValue = libertyValueList[0];     // Set libertyValue to 0
        nodeValue = nodeValueList[0];           // nodeValue is 0

        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;      // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled = isActive;      // Set Current SheepTile active
    }

    public void EmptySheepSetter()              // Sets node to Black Sheep Object
    {
        sheepValue = sheepValueList[0];         // Set sheep value to whiteSheep index
        libertyValue = libertyValueList[0];     // Set libertyValue to 0
        nodeValue = nodeValueList[0];           // nodeValue is 0

        bool isActive = true;
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;      // Sets all SheepTiles to inactive
        }
        
        sheepTileList[sheepValue].GetComponent<MeshRenderer>().enabled = isActive;      // Set Current SheepTile active
    }


    // public void SetGrassTileDisplay()
    // {
    //     bool isActive = true;

    //     if(nodeValue == nodeValueList[0] && settingState)               // Set Value for 0
    //     {
    //         GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[1].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[2].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;

    //         settingState = false;
    //         Debug.Log("SetGrassTile 0 Triggered and settingState = " + settingState);
    //     }

        
    //     if(nodeValue == nodeValueList[1] && settingState)               // Set Value for 1
    //     {
    //         GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[2].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
    //         settingState = false;
    //         Debug.Log("SetGrassTile 1 Triggered and settingState = " + settingState);
    //     }

    //     if(nodeValue == nodeValueList[2] && settingState)               // Set Value for 2
    //     {
    //         GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
    //         GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
    //         settingState = false;
    //         Debug.Log("SetGrassTile 2 Triggered and settingState = " + settingState);
    //     }

    //     if(nodeValue == nodeValueList[3] && settingState)               // Set Value for 3
    //     {
    //         GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[3].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
    //         settingState = false;
    //         Debug.Log("SetGrassTile 3 Triggered and settingState = " + settingState);
    //     }

    //     if(nodeValue == nodeValueList[4] && settingState)               // Set Value for 3
    //     {
    //         GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[3].GetComponent<MeshRenderer>().enabled = isActive;
    //         GrassTileList[4].GetComponent<MeshRenderer>().enabled = isActive;
            
    //         settingState = false;
    //         Debug.Log("SetGrassTile 3 Triggered and settingState = " + settingState);
    //     }

    //     settingState = true;
    // }

}
