using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeArrayScript : MonoBehaviour
{
    private GameObject masterNode;    // Basic Node Value for Reference
    private NodeScript masterNodeScript;

    [SerializeField] public GameObject[,] gNodeArray;
    int arrayColumnLength = 3;                          // Array Dimensions - Column
    int arrayRowLength = 3;                             // Array Dimensions - Row

    [SerializeField] public List<GameObject> gNodeList;
    public GameObject gNode1;
    public GameObject gNode2; 
    public GameObject gNode3;
    public GameObject gNode4; 
    public GameObject gNode5; 
    public GameObject gNode6; 
    public GameObject gNode7; 
    public GameObject gNode8; 
    public GameObject gNode9; 


    // Start is called before the first frame update
    void Start()
    {
        masterNode = GameObject.Find("MasterNode");
        masterNodeScript = masterNode.GetComponent<NodeScript>();

        gNodeArray = new GameObject[3,3];

        // gNode List and Script Reference
        gNodeList = new List<GameObject> { gNode1, gNode2, gNode3, gNode4, gNode5, gNode6, gNode7, gNode8, gNode9 };
    
        Generate3x3NodeArray();
        ArrayPositionNodeValueSetter();
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }

    public void DebugControls()
    {
        if(Input.GetKeyDown(KeyCode.O)) { ArrayPositionNodeValueSetter(); }
        if(Input.GetKeyDown(KeyCode.I)) { ArrayPositionBlackSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.U)) { ArrayPositionWhiteSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.Y)) { ArrayPositionEmptySheepSetter(); }

        if(Input.GetKeyDown(KeyCode.P)) { DisplayArray(); }
    }

    // STARTUP METHODS
    public void Generate3x3NodeArray()              // Generate a 3x3 array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 

        for(int i = 0; i < arrayColumnLength; i ++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j ++)
            {
                gNodeArray[i,j] = gNodeList[nodeCounter];   // Set curent gNodeList object to current array position
                nodeCounter = nodeCounter + 1;              // Increment gNodeList index
            }
        }
    }

    public void DisplayArray()                      // Debugs Array Nodes in Console
    {
        for(int i = 0; i < arrayColumnLength; i ++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j ++)
            {
                Debug.Log(gNodeArray[i,j].name);
            }
        }
    }
    

    public void ArrayPositionNodeValueSetter()      // Displays Array based on nodeValues
    {
        List<int> nodeArrayMapper = new List<int>();  // List to hold update values for arrayNodes
        foreach(GameObject node in gNodeList)         // Loops over all nodes in gNodeList
        {
            nodeArrayMapper.Add(masterNodeScript.nodeValueList[4]);     // Assigns max NodeValue to each position
        }
        
        int mappingCounter = 0;     // Counter to increment through index in nodeArrayMapper

        // Map node values to nodeArrayMapper based on current board state
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                
                // Left
                if(j == 0)      // Check left index is not out of range 
                {
                    nodeArrayMapper[mappingCounter] -= 1;
                }
                if(j != 0)      // Check left position
                {
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0)     // Check left index is not null
                    {
                        nodeArrayMapper[mappingCounter] -= 1;
                    }
                }
                // Right
                if(j == arrayRowLength-1)      // Check right index is not out of range 
                {
                    nodeArrayMapper[mappingCounter] -= 1;
                }
                if(j != arrayRowLength-1)       // Check right position
                {
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0)     // Check right index is not null
                    {
                        nodeArrayMapper[mappingCounter] -= 1;
                    }
                }
                // Top 
                if(i == 0)      // Check top index is not out of range 
                {
                    nodeArrayMapper[mappingCounter] -= 1;
                }
                if(i != 0)      //  Check top position
                {
                    if(gNodeArray[i-1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        nodeArrayMapper[mappingCounter] -= 1;
                    }
                }
                // Bottom
                if(i == arrayColumnLength-1)      // Check bottom index is not out of range 
                {
                    nodeArrayMapper[mappingCounter] -= 1;
                }
                if(i != arrayColumnLength-1)      // Check bottom position 
                {
                    if(gNodeArray[i+1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        nodeArrayMapper[mappingCounter] -= 1;
                    }
                }

                mappingCounter += 1;
            }
        }
        
        if(1 == 1)      // Debug to display nodeArrayMapper values
        {
            int counter = 0;
            foreach(int mapperValue in nodeArrayMapper)
            {
                Debug.Log("nodeArrayMapperValue[" + counter + "] is " + mapperValue);
                counter += 1;
            }
        }

        int nodeSetterCounter = 0;

        // Map nodeArrayMapper values to nodeArray
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.libertyValue = currentNodeScript.libertyValueList[1];

                if(j != 0)      // Check left position
                {
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0)     // Check left index is not null
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                if(j == arrayRowLength-1)      // Check right index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(j != arrayRowLength-1)       // Check right position
                {
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0)     // Check right index is not null
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                if(i == 0)      // Check top index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(i != 0)      //  Check top position
                {
                    if(gNodeArray[i-1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                if(i == arrayColumnLength-1)      // Check bottom index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(i != arrayColumnLength-1)      // Check bottom position 
                {
                    if(gNodeArray[i+1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
                Debug.Log(currentNode.GetComponent<NodeScript>().name + "'s nodeValue is " + currentNode.GetComponent<NodeScript>().nodeValue);
            }
        }
    }

    public void ArrayPositionBlackSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.BlackSheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().settingState = true;
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

    public void ArrayPositionWhiteSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.WhiteSheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().settingState = true;
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

    public void ArrayPositionEmptySheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.EmptySheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().settingState = true;
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

}
    
