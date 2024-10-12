using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class BoardGenerator : MonoBehaviour
{

    //* ---------------------------------------- PROPERTIES ----------------------------------------
    // MasterNode for default Value for Reference
    private GameObject masterNode;

    [SerializeField] public GameObject[,] gNodeArray;
    [SerializeField] public List<GameObject> gNodeList;
    
    // ! Array Size Controls
    public int arrayColumnLength = 19;                  // Array Dimensions - Column
    public int arrayRowLength = 19;                     // Array Dimensions - Row
    public float nodeSpacingValue = 1;                   // Space Between Nodes
    private int arrayTotalNodes;                        // arrayColumnLength * arrayRowLength

    public GameObject nodePrefab;
    public List<int> startNodeValueMap;
    public List<int> currentNDValueMap;

    public Transform gNodeArrayTransform;

    public List<Node> sheepGroupList;



    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                     //* Generates Node GameObjects and gNodeArray 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Board Generator Started");

        masterNode = GameObject.Find("MasterNode");

        gNodeArray = new GameObject[arrayColumnLength, arrayRowLength];
        arrayTotalNodes = arrayColumnLength * arrayRowLength;
        
        gNodeList = new List<GameObject>();
        gNodeArrayTransform = gameObject.transform;

        InstantiateNodes();
        SetNodeTransformPosition();
        
        BuildNodeArray();
        AdjacentSheepNodeMapper(gNodeList);

        // Generate Initial Value Map
        startNodeValueMap = CreateNodeValueMap();
        UpdateBoardNodeValues(startNodeValueMap);
        NodeDisplayUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }



    // *---------------------------------------- BOARD NODE CREATION METHODS ----------------------------------------
                                               //* Called in Start Method
    public void InstantiateNodes()
    {
        for(int i = 1; i <= arrayTotalNodes; i++) {
            GameObject gNode = Instantiate(nodePrefab, gNodeArrayTransform);
            gNode.name = "Node (" + i + ")";
            gNode.GetComponent<NodeScript>().nodeID = i;                            //! Sets NodeID in NodeScript
            gNodeList.Add(gNode);
        }
    }


    public void SetNodeTransformPosition()
    {
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeList[nodeCounter].transform.position = new Vector3(i * nodeSpacingValue, 0, j * nodeSpacingValue); // Example positioning
                nodeCounter++;
            }
        }
    }



    // *---------------------------------------- ARRAY GENERATION AND VALUE SET METHODS ----------------------------------------
                                                    //* Called in Start Method

    // ------------------------------ //* gNodeArray Assigner 0 ------------------------------
    public void BuildNodeArray()                         // Generate a Length x Row array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeArray[i,j] = gNodeList[nodeCounter];   // Set curent gNodeList object to current array position
                
                // Maps Board Array position for reference
                NodeScript NDScript = gNodeList[nodeCounter].GetComponent<NodeScript>();
                NDScript.arrayPosition[0] = i;
                NDScript.arrayPosition[1] = j;
                
                // Add Array Position to Node Name
                gNodeArray[i,j].name = gNodeArray[i,j].name + " [" + i + "," + j+ "]";

                nodeCounter++;                              // Increment gNodeList index
            }
        }
    }



    // *---------------------------------------- SHEEP GROUP METHODS ----------------------------------------
                                //* Loops over gNodeList and assigns adjacent Nodes 
                                            //* CALLED IN START() METHOD
    
    public void AdjacentSheepNodeMapper(List<GameObject> gNodeList)
    {
        foreach(GameObject currentND in gNodeList)
        {
            NodeScript NDScript = currentND.GetComponent<NodeScript>();
            int[] arrayPos = NDScript.arrayPosition;   
        
            // Left index
            if(arrayPos[0] == 0){      // left index is not out of range 
                NDScript.leftND = null;
            }
            if(arrayPos[0] != 0){      // left index is a node
                NDScript.leftND = gNodeArray[arrayPos[0] - 1, arrayPos[1]].gameObject;
                NDScript.leftNDScript = gNodeArray[arrayPos[0] - 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Right index
            if(arrayPos[0] == arrayColumnLength - 1){      // right index is not out of range 
                NDScript.rightND = null;
            }
            else if(arrayPos[0] != arrayColumnLength - 1){      // right index is a node
                NDScript.rightND = gNodeArray[arrayPos[0] + 1, arrayPos[1]].gameObject;
                NDScript.rightNDScript = gNodeArray[arrayPos[0] + 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Bottom index
            if(arrayPos[1] == 0){      // bottom index is not out of range 
                NDScript.bottomND = null;
            }
            if(arrayPos[1] != 0){      // bottom index is a node
                NDScript.bottomND = gNodeArray[arrayPos[0], arrayPos[1] - 1].gameObject;
                NDScript.bottomNDScript = gNodeArray[arrayPos[0], arrayPos[1] - 1].GetComponent<NodeScript>();
            }

            // Top index
            if(arrayPos[1] == arrayRowLength - 1){      // top index is not out of range 
                NDScript.topND = null;
            }
            else if(arrayPos[1] != arrayRowLength - 1){      // top index is a node
                NDScript.topND = gNodeArray[arrayPos[0], arrayPos[1] + 1].gameObject;
                NDScript.topNDScript = gNodeArray[arrayPos[0], arrayPos[1] + 1].GetComponent<NodeScript>();
            }

        }
    }




    // *---------------------------------------- NODE VALUE DISPLAY AND UPDATE METHODS ----------------------------------------
                                            //* Called in Start Method and TargetNode.cs
    public void BoardUpdaterFunction()                                      // Calls Functions 1, 2, 3, 4  --------------
    {
        List<int> NDValueMap = CreateNodeValueMap();
        UpdateBoardNodeValues(NDValueMap);
        NodeDisplayUpdate();
    }

    // --------------------------------------------- // BoardUpdater Part 1 ---------------------------------------------
    public List<int> CreateNodeValueMap()                                      // Displays Array based on nodeValues
    {
        List<int> NDValueMap = new List<int>();  // List to hold update values for arrayNodes
        
        // Reset all node liberty values to 1
        foreach(GameObject node in gNodeList){         // Loops over all nodes in gNodeList    
            NodeScript NDScript = node.GetComponent<NodeScript>();  // Set liberty value based on masterNode
            // No Sheep Placed
            if(NDScript.sheepVal == NDScript.sheepValList[0]){
                NDValueMap.Add(NDScript.NDValueList[4]);     // Assigns max NodeValue to each position
                
                NDScript.libertyVal = NDScript.libertyValList[1];
            }
            // Sheep placed
            else {
                NDValueMap.Add(NDScript.NDValueList[0]);     // Assigns min NodeValue to each position
                
                NDScript.libertyVal = NDScript.libertyValList[0];
            }
        }
        
        // Update NDValueMap based on position and board state
        List<int> NDValueMapUpdated = MapNewNodeValues(NDValueMap);

        // NodeValueMapDebugDisplayValue(NDValueMap);  // DEBUG METHOD 

        // Debug.Log("gNodeArray nodeValues Mapped to List");

        return NDValueMapUpdated;
    }
    // ---------------------------------------- -----// BoardUpdater Part 2 //* Called in Step 1 ------------------------
    public List<int> MapNewNodeValues(List<int> NDValueMap)            // Maps nodeValues for updating in Step 3
    {
        // Counter to increment through index in NDValueMap
        int mapIndex = 0;     

        // Map node values to NDValueMap based on current board state
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){                
                // Left Index Check
                if(j == 0){      // Check left index is not out of range 
                    NDValueMap[mapIndex] -= 1;
                }
                if(j != 0){      // Check left position
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyVal == 0){     // Check left mapIndex is not null
                        NDValueMap[mapIndex] -= 1;
                    }
                }

                // Right Index Check
                if(j == arrayRowLength-1){      // Check right mapIndex is not out of range 
                    NDValueMap[mapIndex] -= 1;
                }
                if(j != arrayRowLength-1){       // Check right position
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyVal == 0){     // Check right mapIndex is not null
                        NDValueMap[mapIndex] -= 1;
                    }
                }

                // Top Index Check
                if(i == 0){      // Check top mapIndex is not out of range 
                    NDValueMap[mapIndex] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(gNodeArray[i-1, j].GetComponent<NodeScript>().libertyVal == 0){
                        NDValueMap[mapIndex] -= 1;
                    }
                }

                // Bottom Index Check
                if(i == arrayColumnLength-1){      // Check bottom mapIndex is not out of range 
                    NDValueMap[mapIndex] -= 1;
                }
                if(i != arrayColumnLength-1){      // Check bottom position 
                    if(gNodeArray[i+1, j].GetComponent<NodeScript>().libertyVal == 0){
                        NDValueMap[mapIndex] -= 1;
                    }
                }

                mapIndex += 1;
            }
        }
        // Debug.Log("gNodeArray MapIndex Updated");

        return NDValueMap;
    }
    // --------------------------------------------- // BoardUpdater Part 3 ---------------------------------------------
    public void UpdateBoardNodeValues(List<int> NDValueMap)                    // Sets Node Display values to Map Values
    {
        int arrayIndex = 0;

        // Map NDValueMap values to NDArray
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){
                GameObject currentND = gNodeArray[i,j];
                NodeScript currentNDScript = currentND.GetComponent<NodeScript>();
                
                // TODO - Update here to check state of node to change liberty Value
                if(NDValueMap[arrayIndex] < 0) {
                    currentNDScript.NDValue = 0;
                }
                else {
                    currentNDScript.NDValue = NDValueMap[arrayIndex];
                }
                
                // currentND.GetComponent<NodeScript>().SetGrassTileDisplay();
                // Debug.Log(currentND.GetComponent<NodeScript>().name + "'s NDValue is " + currentND.GetComponent<NodeScript>().NDValue);

                arrayIndex += 1;
            }
        }

        // Debug.Log("gNodeArray Update Complete");
    }
    // --------------------------------------------- // BoardUpdater Part 4 ---------------------------------------------
    public void NodeDisplayUpdate()                                         // Calls SetGrassTileDisplay on each Node
    {
        foreach(GameObject node in gNodeList)
        {
            node.GetComponent<NodeScript>().SetGrassTileDisplay();
        }
    }





    // GNODE LIST MAPPER
    // public List<int> AdjacentNodeMapIndexer(List<int> NDValueMap)
    // {
    //     // Counter to increment through index in NDValueMap
    //     int index = 0;     

    //     // Map node values to NDValueMap based on current board state
    //     foreach(GameObject node in gNodeList)
    //     {
    //         NodeScript nScript = node.GetComponent<NodeScript>();
    //         List<NodeScript> adjList = nScript.adjNDScriptList;

    //         foreach(NodeScript adjScript in adjList)
    //         {
    //             if(adjScript == null || adjScript.libertyVal == 0)
    //             {
    //                 NDValueMap[index] -= 1;
    //                 Debug.Log("Adj Script " + adjScript.name + " is null at " + node.name);
    //             }
    //         }
            
    //         index += 1;
    //     }   
        
    //     return NDValueMap;
    // }




    // --------------------------------------------- // gNodeValue Updater Part 3 ---------------------------------------------
                                                  //* Calls SetGrassTileDisplay
    // public void NodeValueListUpdater(List<int> NDValueMap)
    // {
    //     int arrayIndex = 0;

    //     // Map NDValueMap values to NDArray
    //     foreach(GameObject node in gNodeList)
    //     {         

    //         NodeScript currentNDScript = node.GetComponent<NodeScript>();
            
    //         if(NDValueMap[arrayIndex] < 0) {
    //             currentNDScript.NDValue = 0;
    //         }
    //         else {
    //             currentNDScript.NDValue = NDValueMap[arrayIndex];
    //         }
            
    //         node.GetComponent<NodeScript>().SetGrassTileDisplay();
    //         Debug.Log(node.GetComponent<NodeScript>().name + "'s NDValue is " + node.GetComponent<NodeScript>().NDValue);

    //         arrayIndex += 1;
            
    //     }

    //     // Debug.Log("gNodeArray Update Complete");
    // }







    // *---------------------------------------- DEBUG METHODS ----------------------------------------
                                          //* Called in Update Method 
    
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


    public void NodeValueDisplayDebug()
    {
        string rowValues = "\n NodeValues \n";    
        
        for(int i = 0; i < arrayColumnLength; i ++){
            for(int j = 0; j < arrayRowLength; j ++){
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().NDValue + "  ";   
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
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().sheepVal + "  ";   
            }
            rowValues += "\n";
        }
        Debug.Log(rowValues);
    }


    public void ArrayPositionBlackSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentND = gNodeArray[i,j];
                NodeScript currentNDScript = currentND.GetComponent<NodeScript>();
                
                currentNDScript.BlackSheepSetter();           // Resets NDValue of all Nodes to 4 before Setting

                currentND.GetComponent<NodeScript>().SetGrassTileDisplay();
            }
        }
    }

    public void ArrayPositionWhiteSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentND = gNodeArray[i,j];
                NodeScript currentNDScript = currentND.GetComponent<NodeScript>();
                
                currentNDScript.WhiteSheepSetter();           // Resets NDValue of all Nodes to 4 before Setting

                currentND.GetComponent<NodeScript>().SetGrassTileDisplay();
            }
        }
    }

    public void ArrayPositionEmptySheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentND = gNodeArray[i,j];
                NodeScript currentNDScript = currentND.GetComponent<NodeScript>();
                
                currentNDScript.libertyVal = currentNDScript.libertyValList[1];     // Set all libertyVal to 1 (Empty sheep object)

                currentNDScript.EmptySheepSetter();           // Resets NDValue of all Nodes to 4 before Setting

                List<int> NDValueMap = CreateNodeValueMap();
                UpdateBoardNodeValues(NDValueMap);
                currentND.GetComponent<NodeScript>().SetGrassTileDisplay();
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
    public void NodeValueMapDebugDisplayValue(List<int> NDValueMap)
    {
        int debugCounter = 0;
        foreach(int mapValue in NDValueMap)
        {
            Debug.Log("NDValueMapValue[" + debugCounter + "] is " + mapValue);
            debugCounter += 1;
        }
    }
}
    