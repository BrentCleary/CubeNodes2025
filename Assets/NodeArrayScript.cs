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

    public Array BoardArray3x3;
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

    public List<int> startNodeValueMap;

    // Start is called before the first frame update
    void Start()
    {
        masterNode = GameObject.Find("MasterNode");
        Debug.Log("MasterNode found with name of " + masterNode.name);
        masterNodeScript = masterNode.GetComponent<NodeScript>();

        gNodeArray = new GameObject[3,3];

        // gNode List and Script Reference
        gNodeList = new List<GameObject> { gNode1, gNode2, gNode3, gNode4, gNode5, gNode6, gNode7, gNode8, gNode9 };

        Generate3x3NodeArray();
        startNodeValueMap = NodeValueMapper();
        NodeValueUpdater(startNodeValueMap);
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }


    // STARTUP METHODS
    public void Generate3x3NodeArray()              // Generate a 3x3 array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 

        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeArray[i,j] = gNodeList[nodeCounter];   // Set curent gNodeList object to current array position
                nodeCounter++;              // Increment gNodeList index
            }
        }
    }

    public void DisplayArray(){                      // Debugs Array Nodes in Console
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                Debug.Log(gNodeArray[i,j].name);
            }
        }
    }
    
    

    // Debug Value Setters
    public void DebugControls()
    {
        if(Input.GetKeyDown(KeyCode.O)) { NodeValueMapper(); }
        if(Input.GetKeyDown(KeyCode.I)) { ArrayPositionBlackSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.U)) { ArrayPositionWhiteSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.Y)) { ArrayPositionEmptySheepSetter(); }

        if(Input.GetKeyDown(KeyCode.P)) { DisplayArray(); }
    }

    public List<int> NodeValueMapper()      // Displays Array based on nodeValues
    {
        List<int> nodeValueMap = new List<int>();  // List to hold update values for arrayNodes
        
        // Reset all node liberty values to 1
        foreach(GameObject node in gNodeList){         // Loops over all nodes in gNodeList    
            NodeScript nodeScript = node.GetComponent<NodeScript>();  // Set liberty value based on masterNode
            // Node Holds No Sheep
            if(nodeScript.sheepValue == nodeScript.sheepValueList[0]){
                nodeValueMap.Add(masterNodeScript.nodeValueList[4]);     // Assigns max NodeValue to each position
                nodeScript.libertyValue = nodeScript.libertyValueList[1];
            }
            // Node does Hold Sheep
            else {
                nodeValueMap.Add(masterNodeScript.nodeValueList[0]);     // Assigns max NodeValue to each position
                nodeScript.libertyValue = nodeScript.libertyValueList[0];
            }
        }
        
        // Counter to increment through index in nodeValueMap
        int mapIndex = 0;     

        // Map node values to nodeValueMap based on current board state
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){                
                // Left
                if(j == 0){      // Check left index is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != 0){      // Check left position
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0){     // Check left mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Right
                if(j == arrayRowLength-1){      // Check right mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != arrayRowLength-1){       // Check right position
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0){     // Check right mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Top 
                if(i == 0){      // Check top mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(gNodeArray[i-1, j].GetComponent<NodeScript>().libertyValue == 0){
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Bottom
                if(i == arrayColumnLength-1){      // Check bottom mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(i != arrayColumnLength-1){      // Check bottom position 
                    if(gNodeArray[i+1, j].GetComponent<NodeScript>().libertyValue == 0){
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                mapIndex += 1;
            }
        }
        
        // if(1 == 1)      // Debug to display nodeValueMap values
        // {
        //     int debugCounter = 0;
        //     foreach(int mapValue in nodeValueMap)
        //     {
        //         Debug.Log("nodeValueMapValue[" + debugCounter + "] is " + mapValue);
        //         debugCounter += 1;
        //     }
        // }

        return nodeValueMap;
        
    }
    
    public void NodeValueUpdater(List<int> nodeValueMap)
    {
        int arrayIndex = 0;

        // Map nodeValueMap values to nodeArray
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                // TODO - Update here to check state of node to change liberty Value
                if(nodeValueMap[arrayIndex] < 0)
                {
                    currentNodeScript.nodeValue = 0;
                }
                else
                {
                    currentNodeScript.nodeValue = nodeValueMap[arrayIndex];
                }
                
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
                // Debug.Log(currentNode.GetComponent<NodeScript>().name + "'s nodeValue is " + currentNode.GetComponent<NodeScript>().nodeValue);
            
                arrayIndex += 1;
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
                
                currentNodeScript.libertyValue = currentNodeScript.libertyValueList[1];     // Set all libertyValue to 1 (Empty sheep object)

                currentNodeScript.EmptySheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().settingState = true;
                List<int> nodeValueMap = NodeValueMapper();
                NodeValueUpdater(nodeValueMap);
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

}
    
