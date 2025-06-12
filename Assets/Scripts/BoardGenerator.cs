using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class BoardGenerator : MonoBehaviour
{
    //* ---------------------------------------- PROPERTIES ----------------------------------------

    [SerializeField] public List<GameObject> ND_List;
    [SerializeField] public GameObject[,] ND_Arr;
    public Transform ND_ArrTransform;
    
    [Header("Board Array Settings")] // ! Array Size Controls
    public int arrColLen = 9;                                              // Array Dimensions - Column
    public int arrRowLen = 9;                                                 // Array Dimensions - Row
    public int arrNDCount => arrColLen * arrRowLen;                         // arrColLen * arrRowLen
    public float ND_Spacing = 1.8f;                                             // Space Between Nodes


    public GameObject ND_Prefab;
    public List<int> startNDValMap;
    public List<int> crntNDValMap;

    public List<Node> Shp_Grp_List;

    public void CreateBoard()  //? Called in GameManagerScript                      // Instantiates Variables, Calls Methods Below
    {
        ND_Arr = new GameObject[arrColLen, arrRowLen];
        ND_ArrTransform = gameObject.transform;

        ND_List = new List<GameObject>();

        InstantiateNodes();
        SetNodeTransformPosition();
        
        BuildNodeArray();
        AdjacentSheepNodeMapper(ND_List);
    }
    // *---------------------------------------- CreateBoard Methods ----------------------------------------
    public void InstantiateNodes()                                                  // Instantiates Nodes, Assigns names and values, Adds them to ND_List
    {
        for(int i = 0; i < arrNDCount; i++) {
            GameObject gNode = Instantiate(ND_Prefab, ND_ArrTransform);
            gNode.name = $"Node ({i})";
            gNode.GetComponent<NodeScript>().nodeID = i;                            //! Sets NodeID in NodeScript
            ND_List.Add(gNode);
        }
    }
    public void SetNodeTransformPosition()                                          // Sets Node transform.position ( uses NodeSpacingValue, array vals  )
    {
        int nodeCounter = 0;                                // Increments Node reference in ND_List 
        for(int i = 0; i < arrColLen; i ++){         // Assigns positions to each gNode in ND_Arr
            for(int j = 0; j < arrRowLen; j ++){
                ND_List[nodeCounter].transform.position = new Vector3(i * ND_Spacing, 0, j * ND_Spacing); // Example positioning
                nodeCounter++;
            }
        }
    }
    public void BuildNodeArray()                                                    // Generate Array using Length x Row using nodes in ND_List
    {               
        int nodeCounter = 0;                                                        // Increments Node reference in ND_List 
        for(int i = 0; i < arrColLen; i ++){                                // Assigns positions to each gNode in ND_Arr
            for(int j = 0; j < arrRowLen; j ++){
                ND_Arr[i,j] = ND_List[nodeCounter];                           // Set curent ND_List object to current array position
                
                // Maps Board Array position for reference
                NodeScript NDScript = ND_List[nodeCounter].GetComponent<NodeScript>();
                NDScript.arrayPosition[0] = i;
                NDScript.arrayPosition[1] = j;
                
                // Add Array Position to Node Name
                ND_Arr[i,j].name = ND_Arr[i,j].name + " [" + i + "," + j+ "]";

                nodeCounter++;                              // Increment ND_List index
            }
        }
    }
    public void AdjacentSheepNodeMapper(List<GameObject> ND_List)                 // Loops over ND_List and assigns adjacent Nodes
    {
        foreach(GameObject crntND in ND_List)
        {
            NodeScript NDScript = crntND.GetComponent<NodeScript>();
            int[] arrayPos = NDScript.arrayPosition;   
        
            // Left index
            if(arrayPos[0] == 0){      // left index is out of range 
                NDScript.leftND = null;
            }
            if(arrayPos[0] != 0){      // left index is a node
                NDScript.leftND = ND_Arr[arrayPos[0] - 1, arrayPos[1]].gameObject;
                NDScript.leftNDScript = ND_Arr[arrayPos[0] - 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Right index
            if(arrayPos[0] == arrColLen - 1){      // right index is out of range 
                NDScript.rightND = null;
            }
            else if(arrayPos[0] != arrColLen - 1){      // right index is a node
                NDScript.rightND = ND_Arr[arrayPos[0] + 1, arrayPos[1]].gameObject;
                NDScript.rightNDScript = ND_Arr[arrayPos[0] + 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Bottom index
            if(arrayPos[1] == 0){      // bottom index is out of range 
                NDScript.bottomND = null;
            }
            if(arrayPos[1] != 0){      // bottom index is a node
                NDScript.bottomND = ND_Arr[arrayPos[0], arrayPos[1] - 1].gameObject;
                NDScript.bottomNDScript = ND_Arr[arrayPos[0], arrayPos[1] - 1].GetComponent<NodeScript>();
            }

            // Top index
            if(arrayPos[1] == arrRowLen - 1){      // top index is out of range 
                NDScript.topND = null;
            }
            else if(arrayPos[1] != arrRowLen - 1){      // top index is a node
                NDScript.topND = ND_Arr[arrayPos[0], arrayPos[1] + 1].gameObject;
                NDScript.topNDScript = ND_Arr[arrayPos[0], arrayPos[1] + 1].GetComponent<NodeScript>();
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
        
        foreach(GameObject crntND in ND_List)                                         // Loops over all nodes in ND_List    
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
        for(int i = 0; i < arrColLen; i++){         // Assigns positions to each gNode in ND_Arr
            for(int j = 0; j < arrRowLen; j++){                
                // Left Index Check
                if(j == 0){      // Check left index is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(j != 0){      // Check left position
                    if(ND_Arr[i, j-1].GetComponent<NodeScript>().libertyVal == 0){     // Check left crntNDval is not null
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Right Index Check
                if(j == arrRowLen-1){      // Check right crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(j != arrRowLen-1){       // Check right position
                    if(ND_Arr[i, j+1].GetComponent<NodeScript>().libertyVal == 0){     // Check right crntNDval is not null
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Top Index Check
                if(i == 0){      // Check top crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(ND_Arr[i-1, j].GetComponent<NodeScript>().libertyVal == 0){
                        newValueMap[crntNDval] -= 1;
                    }
                }

                // Bottom Index Check
                if(i == arrColLen-1){      // Check bottom crntNDval is not out of range 
                    newValueMap[crntNDval] -= 1;
                }
                if(i != arrColLen-1){      // Check bottom position 
                    if(ND_Arr[i+1, j].GetComponent<NodeScript>().libertyVal == 0){
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
        for(int i = 0; i < arrColLen; i++)         // Assigns positions to each gNode in ND_Arr
        {    
            for(int j = 0; j < arrRowLen; j++)
            {
                GameObject crntND = ND_Arr[i,j];
                NodeScript crntNDScript = crntND.GetComponent<NodeScript>();
                
                // Debug.Log("NDmapValue" + arrayIndex + " is " + NDValueMap[arrayIndex]);

                if(NDValueMap[arrayIndex] < 0) {
                    crntNDScript.NDValue = 0;
                }
                else {
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
        foreach(GameObject crntNode in ND_List)
        {
            NodeScript crntNDScrpt = crntNode.GetComponent<NodeScript>();
            crntNDScrpt.UpdateNodeDisplay();
        }
    }


    // *---------------------------------------- Ko Check Methods ---------------------------------
    public List<int> Create_ShpValMap()  //? Called in GameManagerScript               // Displays Array based on nodeValues
    {
        List<int> ShpValMap = new List<int>();                                         // List to hold update values for arrayNodes
        
        foreach(GameObject crntND in ND_List)                                         // Loops over all nodes in ND_List    
        {    
            NodeScript NDScript = crntND.GetComponent<NodeScript>();                    // Set liberty value based on masterNode
            ShpValMap.Add(NDScript.sheepVal);
        }

        return ShpValMap;
    }

    
    public bool Check_Map_For_Ko(List<int> prevShpValMap, int ND_ID, int ShpVal)        // Map of board state before last move, ND_ID and Value
    {
        bool isKo = false;                                                              // Ko is initially set false

        // Debug.Log("prevShpValMap[" + ND_ID + "] : " + prevShpValMap[ND_ID] );

        List<int> newShpValMap = prevShpValMap.ToList();                                         // Create a new copy of ShpValMap for updating and comparing
        newShpValMap[ND_ID] = ShpVal;                                                   // Change List val to ShpVal at ND_ID index (index list matches ND_ID)
        // Debug.Log("newShpValMap[" + ND_ID + "] : " + newShpValMap[ND_ID] );

        bool sequenceCheck = newShpValMap.SequenceEqual(prevShpValMap);
        Debug.Log("sequenceCheck: " + sequenceCheck);

            List<int> differingIndices = FindDifferences(newShpValMap, prevShpValMap);
            if (differingIndices.Count > 0)
            {
                Debug.Log("Sequences differ at indices: " + string.Join(", ", differingIndices));
            }
            else
            {
                Debug.Log("Sequences are identical.");
            }

        if(sequenceCheck)                                               // Compare to prev ShpValMap for Board state
        {
            isKo = true;                                                                // If they match, the move will violate KO rules
            Debug.Log("** KO is True **");
            LogListValues<int>(prevShpValMap, "prevShpValMap");
            LogListValues<int>(newShpValMap, "newShpValMap");
        }
        else
        {
            isKo = false;
        }

        return isKo;                                                                    // Return the Ko bool Value
    }
    // DEBUG METHOD FOR KO CHECK METHODS
    List<int> FindDifferences(List<int> newShpValMap, List<int> prevShpValMap)
    {
        List<int> differences = new List<int>();

        for (int i = 0; i < newShpValMap.Count; i++)
        {
            if (newShpValMap[i] != prevShpValMap[i])
            {
                differences.Add(i); // Record the index where they differ
            }
        }

        return differences;
    
    }


    void LogListValues<T>(List<T> list, string listName)
    {
        string values = string.Join(", ", list);
        Debug.Log($"{listName} values: [{values}]");
    }



    /* Depreciated Methods - Commented out for reference
    // GNODE LIST MAPPER
    // public List<int> AdjacentNodeMapIndexer(List<int> NDValueMap)
    // {
    //     // Counter to increment through index in NDValueMap
    //     int index = 0;     

    //     // Map node values to NDValueMap based on current board state
    //     foreach(GameObject node in ND_List)
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
    //     foreach(GameObject node in ND_List)
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

    //     // Debug.Log("ND_Arr Update Complete");
    // }

    */

}
    