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

    [SerializeField] public List<GameObject> gNodeList;
    [SerializeField] public GameObject[,] gNodeArray;
    public Transform gNodeArrayTransform;
    
    // ! Array Size Controls
    private int arrayColumnLength = 3;                                              // Array Dimensions - Column
    private int arrayRowLength = 3;                                                 // Array Dimensions - Row
    private int arrayTotalNodes => arrayColumnLength * arrayRowLength;              // arrayColumnLength * arrayRowLength
    
    private float nodeSpacingValue = 2;                                             // Space Between Nodes

    public GameObject nodePrefab;
    public List<int> startNodeValueMap;
    public List<int> currentNDValueMap;

    public List<Node> sheepGroupList;


    public void CreateBoard()  //? Called in GameManagerScript                      // Instantiates Variables, Calls Methods Below
    {
        gNodeArray = new GameObject[arrayColumnLength, arrayRowLength];
        gNodeArrayTransform = gameObject.transform;

        gNodeList = new List<GameObject>();

        InstantiateNodes();
        SetNodeTransformPosition();
        
        BuildNodeArray();
        AdjacentSheepNodeMapper(gNodeList);
    }
    // *---------------------------------------- CreateBoard Methods ----------------------------------------
    public void InstantiateNodes()                                                  // Instantiates Nodes, Assigns names and values, Adds them to gNodeList
    {
        for(int i = 1; i <= arrayTotalNodes; i++) {
            GameObject gNode = Instantiate(nodePrefab, gNodeArrayTransform);
            gNode.name = "Node (" + i + ")";
            gNode.GetComponent<NodeScript>().nodeID = i;                            //! Sets NodeID in NodeScript
            gNodeList.Add(gNode);
        }
    }
    public void SetNodeTransformPosition()                                          // Sets Node transform.position ( uses NodeSpacingValue, array vals  )
    {
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeList[nodeCounter].transform.position = new Vector3(i * nodeSpacingValue, 0, j * nodeSpacingValue); // Example positioning
                nodeCounter++;
            }
        }
    }
    public void BuildNodeArray()                                                    // Generate Array using Length x Row using nodes in gNodeList
    {               
        int nodeCounter = 0;                                                        // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){                                // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeArray[i,j] = gNodeList[nodeCounter];                           // Set curent gNodeList object to current array position
                
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
    public void AdjacentSheepNodeMapper(List<GameObject> gNodeList)                 // Loops over gNodeList and assigns adjacent Nodes
    {
        foreach(GameObject crntND in gNodeList)
        {
            NodeScript NDScript = crntND.GetComponent<NodeScript>();
            int[] arrayPos = NDScript.arrayPosition;   
        
            // Left index
            if(arrayPos[0] == 0){      // left index is out of range 
                NDScript.leftND = null;
            }
            if(arrayPos[0] != 0){      // left index is a node
                NDScript.leftND = gNodeArray[arrayPos[0] - 1, arrayPos[1]].gameObject;
                NDScript.leftNDScript = gNodeArray[arrayPos[0] - 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Right index
            if(arrayPos[0] == arrayColumnLength - 1){      // right index is out of range 
                NDScript.rightND = null;
            }
            else if(arrayPos[0] != arrayColumnLength - 1){      // right index is a node
                NDScript.rightND = gNodeArray[arrayPos[0] + 1, arrayPos[1]].gameObject;
                NDScript.rightNDScript = gNodeArray[arrayPos[0] + 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Bottom index
            if(arrayPos[1] == 0){      // bottom index is out of range 
                NDScript.bottomND = null;
            }
            if(arrayPos[1] != 0){      // bottom index is a node
                NDScript.bottomND = gNodeArray[arrayPos[0], arrayPos[1] - 1].gameObject;
                NDScript.bottomNDScript = gNodeArray[arrayPos[0], arrayPos[1] - 1].GetComponent<NodeScript>();
            }

            // Top index
            if(arrayPos[1] == arrayRowLength - 1){      // top index is out of range 
                NDScript.topND = null;
            }
            else if(arrayPos[1] != arrayRowLength - 1){      // top index is a node
                NDScript.topND = gNodeArray[arrayPos[0], arrayPos[1] + 1].gameObject;
                NDScript.topNDScript = gNodeArray[arrayPos[0], arrayPos[1] + 1].GetComponent<NodeScript>();
            }

        }
    }


    public void UpdateBoardNodeValues()  //? Called GameManagerScript               // Calls Functions Step1, Step2, Step3 
    {
        List<int> NDValueMap = Create_NDValueMap_Step1();
        List<int> NDValueMapUpdated = Set_NDValueMap_Step2(NDValueMap);                 // Update NDValueMap based on position and board state
        Update_NDValues_Step3(NDValueMapUpdated);
    }
    // *---------------------------------------- Node Value Update Methods ----------------------------------
    public List<int> Create_NDValueMap_Step1()                                      // Displays Array based on nodeValues
    {
        
        List<int> NDValueMap = new List<int>();                                         // List to hold update values for arrayNodes
        
        foreach(GameObject crntND in gNodeList)                                         // Loops over all nodes in gNodeList    
        {    
            NodeScript NDScript = crntND.GetComponent<NodeScript>();                    // Set liberty value based on masterNode

            if(NDScript.sheepVal == NDScript.sheepValList[0])                           // No Sheep
            {
                NDValueMap.Add(NDScript.NDValueList[4]);                                // Assigns max NodeValue to each position
                NDScript.libertyVal = NDScript.libertyValList[1];
            }
            else                                                                        // Sheep placed
            {
                NDValueMap.Add(NDScript.NDValueList[0]);                                // Assigns min NodeValue to each position
                NDScript.libertyVal = NDScript.libertyValList[0];
            }
        }

        return NDValueMap;
    }
    public List<int> Set_NDValueMap_Step2(List<int> NDValueMap)                     // Maps nodeValues for updating in Step 3
    {
        List<int> newValueMap = NDValueMap;

        int crntNDval = 0;     

        // Map node values to NDValueMap based on current board state
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){                
                // Left Index Check
                if(j == 0){      // Check left index is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(j != 0){      // Check left position
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyVal == 0){     // Check left crntNDval is not null
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Right Index Check
                if(j == arrayRowLength-1){      // Check right crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(j != arrayRowLength-1){       // Check right position
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyVal == 0){     // Check right crntNDval is not null
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Top Index Check
                if(i == 0){      // Check top crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(gNodeArray[i-1, j].GetComponent<NodeScript>().libertyVal == 0){
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Bottom Index Check
                if(i == arrayColumnLength-1){      // Check bottom crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(i != arrayColumnLength-1){      // Check bottom position 
                    if(gNodeArray[i+1, j].GetComponent<NodeScript>().libertyVal == 0){
                        newValueMap[crntNDval] -= 1;
                    }
                }

                crntNDval += 1;
            }
        }

        return newValueMap;
    }
    public void Update_NDValues_Step3(List<int> NDValueMap)                         // Sets Node Display values to Map Values
    {
        int arrayIndex = 0;

        // Map NDValueMap values to NDArray
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {    
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject crntND = gNodeArray[i,j];
                NodeScript crntNDScript = crntND.GetComponent<NodeScript>();
                
                Debug.Log("NDmapValue" + arrayIndex + " is " + NDValueMap[arrayIndex]);

                if(NDValueMap[arrayIndex] < 0) 
                {
                    crntNDScript.NDValue = 0;
                }
                else 
                {
                    crntNDScript.NDValue = NDValueMap[arrayIndex];
                }

                // Debug.Log(crntND.name + " NDValue is " + crntNDScript.NDValue);
                arrayIndex += 1;
            }
        }
    }


    // *---------------------------------------- Node Display Update Method ---------------------------------
    public void UpdateBoardDisplay()  //? Called in GameManagerScript               // Updated Display of Nodes
    {
        foreach(GameObject crntNode in gNodeList)
        {
            NodeScript crntNDScrpt = crntNode.GetComponent<NodeScript>();
            crntNDScrpt.UpdateNodeDisplay();
        }
    }



    /* Depreciated Methods - Commented out for reference
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

    //         NodeScript crntNDScript = node.GetComponent<NodeScript>();
            
    //         if(NDValueMap[arrayIndex] < 0) {
    //             crntNDScript.NDValue = 0;
    //         }
    //         else {
    //             crntNDScript.NDValue = NDValueMap[arrayIndex];
    //         }
            
    //         node.GetComponent<NodeScript>().SetGrassTileDisplay();
    //         Debug.Log(node.GetComponent<NodeScript>().name + "'s NDValue is " + node.GetComponent<NodeScript>().NDValue);

    //         arrayIndex += 1;
            
    //     }

    //     // Debug.Log("gNodeArray Update Complete");
    // }

    */

}
    