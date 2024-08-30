using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardGenerator : MonoBehaviour
{
        // ---------------------------------------- PROPERTIES ----------------------------------------

    // * MasterNode for default Value for Reference
    private GameObject masterNode;    
    private NodeScript masterNodeScript;

    [SerializeField] public GameObject[,] gNodeArray;
    [SerializeField] public List<GameObject> gNodeList;
    
    // ! Array Size Controls
    private int arrayColumnLength = 9;                          // Array Dimensions - Column
    private int arrayRowLength = 9;                             // Array Dimensions - Row
    private int arrayTotalNodes;

    public GameObject nodePrefab;
    public List<int> startNodeValueMap;

    public Transform gNodeArrayTransform;


    // ---------------------------------------- START AND UPDATE METHODS ----------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Board Generator Started");

        masterNode = GameObject.Find("MasterNode");
        masterNodeScript = masterNode.GetComponent<NodeScript>();

        gNodeArray = new GameObject[arrayColumnLength, arrayRowLength];
        arrayTotalNodes = arrayColumnLength * arrayRowLength;
        
        gNodeList = new List<GameObject>();
        gNodeArrayTransform = gameObject.transform;

        NodeGenerator();
        NodePositionSetter();
        
        GenerateNodeArray();

        // Generate Initial Value Map
        startNodeValueMap = NodeValueMapper();
        NodeValueUpdater(startNodeValueMap);
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }



    // *---------------------------------------- BOARD CREATION METHODS METHODS ----------------------------------------
                                                  //* Called in Start Method
    public void NodeGenerator()
    {
        for(int i = 1; i <= arrayTotalNodes; i++) {
            GameObject gNode = Instantiate(nodePrefab, gNodeArrayTransform);
            gNode.name = gNode + "(" + i + ")";
            gNodeList.Add(gNode);
        }
    }

    public void NodePositionSetter()
    {
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeList[nodeCounter].transform.position = new Vector3(i * 2.0f, 0, j * 2.0f); // Example positioning
                nodeCounter++;
            }
        }
    }



    // *---------------------------------------- ARRAY GENERATION AND VALUE SET METHODS ----------------------------------------
                                                    //* Called in Start Method
    // START METHODS
    public void GenerateNodeArray()              // Generate a Length x Row array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 

        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeArray[i,j] = gNodeList[nodeCounter];   // Set curent gNodeList object to current array position
                nodeCounter++;              // Increment gNodeList index
            }
        }
    }

    // NodeValue Updater Part1
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
        
        // Update nodeValueMap based on position and board state
        nodeValueMap = NodeValueMapIndexer(nodeValueMap);

        // NodeValueMapDebugDisplayValue(nodeValueMap);  // DEBUG METHOD 

        Debug.Log("gNodeArray nodeValues Mapped to List");

        return nodeValueMap;
    }
    
    // NodeValue Updater Part2
    public List<int> NodeValueMapIndexer(List<int> nodeValueMap)
    {
        // Counter to increment through index in nodeValueMap
        int mapIndex = 0;     

        // Map node values to nodeValueMap based on current board state
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){                
                // Left Index Check
                if(j == 0){      // Check left index is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != 0){      // Check left position
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0){     // Check left mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Right Index Check
                if(j == arrayRowLength-1){      // Check right mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != arrayRowLength-1){       // Check right position
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0){     // Check right mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Top Index Check
                if(i == 0){      // Check top mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(gNodeArray[i-1, j].GetComponent<NodeScript>().libertyValue == 0){
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Bottom Index Check
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
        Debug.Log("gNodeArray MapIndex Updated");

        return nodeValueMap;
    }

    // NodeValue Updater Part3
    public void NodeValueUpdater(List<int> nodeValueMap)
    {
        int arrayIndex = 0;

        // Map nodeValueMap values to nodeArray
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                // TODO - Update here to check state of node to change liberty Value
                if(nodeValueMap[arrayIndex] < 0) {
                    currentNodeScript.nodeValue = 0;
                }
                else {
                    currentNodeScript.nodeValue = nodeValueMap[arrayIndex];
                }
                
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
                // Debug.Log(currentNode.GetComponent<NodeScript>().name + "'s nodeValue is " + currentNode.GetComponent<NodeScript>().nodeValue);

                arrayIndex += 1;
            }
        }

        Debug.Log("gNodeArray Update Complete");
    }





    // *---------------------------------------- DEBUG METHODS ----------------------------------------
                                          //* Called in Update Method 

    public void NodeValueDisplayDebug()
    {
        string rowValues = "\n NodeValues \n";    
        
        for(int i = 0; i < arrayColumnLength; i ++){
            for(int j = 0; j < arrayRowLength; j ++){
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().nodeValue + "  ";   
            }
            rowValues += "\n";
        }
        Debug.Log(rowValues);
    }

    public void SheepValueDisplayDebug()
    {
        string rowValues = "\n SheepValues \n";    
        
        for(int i = 0; i < arrayColumnLength; i ++){
            for(int j = 0; j < arrayRowLength; j ++){
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().sheepValue + "  ";   
            }
            rowValues += "\n";
        }
        Debug.Log(rowValues);
    }

    // Debug Value Setters
    public void DebugControls()
    {
        if(Input.GetKeyDown(KeyCode.I)) { ArrayPositionBlackSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.U)) { ArrayPositionWhiteSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.Y)) { ArrayPositionEmptySheepSetter(); }

        if(Input.GetKeyDown(KeyCode.P)) { DisplayArray(); }
        
        if(Input.GetKeyDown(KeyCode.G)) { SheepValueDisplayDebug(); }
        if(Input.GetKeyDown(KeyCode.H)) { NodeValueDisplayDebug(); }

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

    public void DisplayArray(){                      // Debugs Array Nodes in Console
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                Debug.Log(gNodeArray[i,j].name);
            }
        }
    }

    // Debug for displaying NodeValueMap - Disabled
    public void NodeValueMapDebugDisplayValue(List<int> nodeValueMap)
    {
        int debugCounter = 0;
        foreach(int mapValue in nodeValueMap)
        {
            Debug.Log("nodeValueMapValue[" + debugCounter + "] is " + mapValue);
            debugCounter += 1;
        }
    }
}
    
