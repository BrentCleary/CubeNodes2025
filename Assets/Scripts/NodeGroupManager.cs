using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.Globalization;
using System;

public class NodeGroupManager : MonoBehaviour
{
    [System.Serializable]
    public class Group
    {
        public int? GrpID {get; set;}
        public int GrpLibs;
        public int Grp_ShpVal;
        public List<int> ND_ID_List;
        
        public static int groupCounter = -1;

        // Constructor
        public Group()
        {
            groupCounter++;                                                         // Increment the counter and assign it to GrpID
            GrpID = groupCounter;
            ND_ID_List = new List<int>();
        }
    }

    public List<Group> All_Grp_List = new List<Group>();
    public int lastND_ID;
    public int lastND_ShpVal;
    public int? lastGrpID;


    public BoardGenerator brd_Gntr_Script;
    public NodeScript lastND_Script;
    public TargetNode targetNDScript;
    public List<GameObject> All_ND_List;

    public void Start()
    {
        brd_Gntr_Script = GetComponent<BoardGenerator>();
        All_ND_List = brd_Gntr_Script.gNodeList;
    }



    public void UpdateGroups(int crntND_ID) //? Called in GameManagerScript //  
    {
        CreateNewGroup(crntND_ID);
        AssignSheepToGroups(crntND_ID);                                            // Assign All Groups
        CalculateGrpLiberties();                                                    // Update All Group Liberties
        DeleteZeroLibertyGroupMethods();
    }



    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int? CreateNewGroup(int crntND_ID)   //? Called GameManagerScript    // Creates New Group - Called in TargetNode - Returns GrpID
    {
        GameObject crntND = GetNodeWithID(All_ND_List, crntND_ID);
        NodeScript NDScript = crntND.GetComponent<NodeScript>();
        
        //~ Group Attributes
        Group newGrp = new Group();
        int? newGrpID = newGrp.GrpID;
        newGrp.Grp_ShpVal = crntND.GetComponent<NodeScript>().sheepVal;
        
        //~ Associate Group, Node, List
        NDScript.NDgrpID = newGrpID;
        newGrp.ND_ID_List.Add(NDScript.nodeID);

        //~ Update Global Properties
        All_Grp_List.Add(newGrp);        
        lastND_ID = crntND_ID;                                              // ! Sets last placed ND_ID for Script Reference
        lastND_ShpVal = NDScript.sheepVal;


        Debug.Log("NodeGM : CreateNewGroup: Group ID: [ " + newGrpID + " ]");

        return newGrpID;        
    }
    


    //? ---------------------------------------- STATIC METHODS  ----------------------------------------
    //* ---------------------------------------- GetNodeWithID ----------------------------------------
    // ------ Get NODE from nodeID in gNodeArray -------
    public GameObject GetNodeWithID(List<GameObject> gameObjectList, int targetID)
    {
        foreach (GameObject obj in gameObjectList)
        {
            NodeScript nodeScript = obj.GetComponent<NodeScript>();
            if (nodeScript != null && nodeScript.nodeID == targetID)
            {
                return obj; // Found the GameObject with the target ID
            }
        }
        return null; // Return null if no GameObject with the target ID is found
    }
    
    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int? grpID)                                               // Returns Group from AllGrpList by grpID
    {
        Group group = All_Grp_List.FirstOrDefault(g => g.GrpID == grpID);
        
        return group;
    }

    //* ---------------------------------------- JoinGroups ----------------------------------------
    public void JoinGroups(int? newGrpID, int? prevGrpID)                           // Adds prevGrpIDs to newGrp.ND_ID_List - return newGrp.ND_ID_List 
    {
        if(prevGrpID != null && prevGrpID != newGrpID)
        {
            Group newGrp = GetGroup(newGrpID);
            Group prevGrp = GetGroup(prevGrpID);

            foreach(int ID in prevGrp.ND_ID_List) 
            {
                newGrp.ND_ID_List.Add(ID);
                GameObject node = GetNodeWithID(All_ND_List, ID);
                node.GetComponent<NodeScript>().NDgrpID = newGrp.GrpID;
            }

            DeleteGroup(prevGrpID);
        }

        // DeleteGroup(prevGrpID);
        // Debug.Log("GroupManager: JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");
        
    }
    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int? delGrpID)                                         // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
    {
        if(delGrpID != null && delGrpID != lastGrpID) 
        {
            Group grpToDelete = All_Grp_List.FirstOrDefault(g => g.GrpID == delGrpID);
            
            All_Grp_List.Remove(grpToDelete);
            
            Debug.Log("Group #" + grpToDelete.GrpID + " cleared and deleted.");
        }
        else {
            Debug.Log("GrpID was null");
        }
    }

    //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
    public void CalculateGrpLiberties()                                             // Updates Liberties of all Groups in AllGrpList
    {
        int totalGrpLibs = 0;
        
        foreach(Group Grp in All_Grp_List)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            List<GameObject> GrpNDList = new List<GameObject>();
            
            foreach(int ND_ID in Grp.ND_ID_List)
            {
                GameObject crntND = GetNodeWithID(All_ND_List, ND_ID);
                GrpNDList.Add(crntND);
            }

            List<GameObject> countedNDs = new List<GameObject>();                     //! This holds adjacent nodes to prevent double references for lib values 
            
            foreach(GameObject crntND in GrpNDList)                                        // Adds value of node to total liberties
            {
                NodeScript NDScript = crntND.GetComponent<NodeScript>();                  // Node script reference to get all information

                if(NDScript.leftND != null){                                            // If the adj node is not empty
                    if(countedNDs.Contains(NDScript.leftND) == false)                 // If the node is not already in the script (has been counted)
                    {
                        countedNDs.Add(NDScript.leftND);                              // Add it to the liberty node list
                        totalGrpLibs += NDScript.leftNDScript.libertyVal;          // Add it's liberty value to the group ( 1 or 0 )
                    }
                }
                if(NDScript.rightND != null){
                    if(countedNDs.Contains(NDScript.rightND) == false)
                    {
                        countedNDs.Add(NDScript.rightND);
                        totalGrpLibs += NDScript.rightNDScript.libertyVal;
                    }
                }
                if(NDScript.bottomND != null){
                    if(countedNDs.Contains(NDScript.bottomND) == false)
                    {
                        countedNDs.Add(NDScript.bottomND);
                        totalGrpLibs += NDScript.bottomNDScript.libertyVal;
                    }
                }
                if(NDScript.topND != null){
                    if(countedNDs.Contains(NDScript.topND) == false)
                    {
                        countedNDs.Add(NDScript.topND);
                        totalGrpLibs += NDScript.topNDScript.libertyVal;
                    }
                }
            }
            
            Grp.GrpLibs = totalGrpLibs;

            totalGrpLibs = 0;
            countedNDs.Clear();
        }

    }




    //* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
    public void DeleteZeroLibertyGroupMethods()   //? Called GameManagerScript      // Sets Nodes in ZeroGrps to Empty, Deletes Group
    {
        List<int?> zeroGrpIDList = GetZeroLibGrpIDList();                       // Update All Group Liberties
        List<Group> zeroGrpList = GetZeroLibGrp(zeroGrpIDList);                 // Delete Groups with 0 Liberties
        DeleteZeroLibGrps(zeroGrpList);
    }

    public List<int?> GetZeroLibGrpIDList()                          // STEP 1  // Returns list from AllGrpList with GrpLiberties = 0
    {
        List<int?> zero_GrpID_List = new List<int?>();

        foreach(Group group in All_Grp_List)
        {
            if(group.GrpLibs == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zero_GrpID_List.Add(group.GrpID);
            }
        }
        return zero_GrpID_List;
    }

    public List<Group> GetZeroLibGrp(List<int?> zero_GrpID_List)       // STEP 2  // Receives the zeroLibertyGrpID list from CalculateGrouLiberties()
    {
        List<Group> zero_Grp_List = new List<Group>();                                    // Create a new list for sorting

        foreach(Group crntGrp in All_Grp_List)                                            // Look through list of All Groups
        {    
            if(zero_GrpID_List.Contains(crntGrp.GrpID))                                  // If the zeroList contains the ID of a Zero'd Node Group
            {
                zero_Grp_List.Add(crntGrp);                                               // Add it to the zeroGrpList for updating
            }
        }
        return zero_Grp_List;
    }

    public void DeleteZeroLibGrps(List<Group> zero_Grp_List)            // STEP 3  // Deletes groups in zeroGrpList
    {
        if(zero_Grp_List.Count > 0)
        {
            foreach(Group zeroGrp in zero_Grp_List)                                   // Loop of new list of Zero liberty Groups
            {    
                if(zeroGrp.Grp_ShpVal != lastND_ShpVal)                            // ! Checks if zeroGroup is the same sheepVal as current player
                {
                    foreach(int zeroND_ID in zeroGrp.ND_ID_List)                // Get the script of each Node and Set Node to Empty
                    {
                        GameObject zeroND = GetNodeWithID(All_ND_List, zeroND_ID);
                        NodeScript zeroScript = zeroND.GetComponent<NodeScript>(); 
                        zeroScript.EmptySheepSetter();
                    }
                }
                DeleteGroup(zeroGrp.GrpID);
            }
        }
        else
        {
            Debug.Log("No zeroLiberty Groups");
        }
    }



    //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
    public void AssignSheepToGroups(int targetND_ID)
    {
        GameObject targetND = GetNodeWithID(All_ND_List, targetND_ID);
        NodeScript NDScript = targetND.GetComponent<NodeScript>();

        if(NDScript.sheepVal != NDScript.sheepValList[0])
        {
            // Create new NodeGroup, assign grpID
            if(NDScript.NDgrpID == null)
            {
                NDScript.NDgrpID = CreateNewGroup(NDScript.nodeID);
            }
            int? targetNDGrpID = NDScript.NDgrpID;
            
            // Adjacent Node Scripts
            NodeScript leftNDScript = NDScript.leftNDScript;
            NodeScript rightNDScript = NDScript.rightNDScript;
            NodeScript bottomNDScript = NDScript.bottomNDScript;
            NodeScript topNDScript = NDScript.topNDScript;

            //* FIRST CHECK OCCURS ON LEFT NODE
            // Left Node Found
            if(leftNDScript != null && leftNDScript.sheepVal == NDScript.sheepVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int? leftNDGrpID = leftNDScript.NDgrpID;

                if(leftNDGrpID != targetNDGrpID && leftNDGrpID != null){
                    JoinGroups(targetNDGrpID, leftNDGrpID);
                }
            }
            // Right Node Found
            if(rightNDScript != null && rightNDScript.sheepVal == NDScript.sheepVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int? rightNDGrpID = rightNDScript.NDgrpID;

                if(rightNDGrpID != targetNDGrpID && rightNDGrpID != null){
                    JoinGroups(targetNDGrpID, rightNDGrpID);
                }
            }
            // Bottom Node Found
            if(bottomNDScript != null && bottomNDScript.sheepVal == NDScript.sheepVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int? bottomNDGrpID = bottomNDScript.NDgrpID;
                if(bottomNDGrpID != targetNDGrpID && bottomNDGrpID != null){
                    JoinGroups(targetNDGrpID, bottomNDGrpID);
                }
            }
            // Top Node Found
            if(topNDScript != null && topNDScript.sheepVal == NDScript.sheepVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int? topNDGrpID = topNDScript.NDgrpID;

                if(topNDGrpID != targetNDGrpID && topNDGrpID != null){
                    JoinGroups(targetNDGrpID, topNDGrpID);
                }
            }

            NDScript.lastPlaced = true;
            lastND_ID = NDScript.nodeID;
        }
    }


    //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
    public void AssignSheepToGroups_REFACTOR(int newND_ID)
    {
        GameObject newND = GetNodeWithID(All_ND_List, newND_ID);
        NodeScript NDScript = newND.GetComponent<NodeScript>();

        // Create new NodeGroup, assign grpID
        if(NDScript.NDgrpID == null)
        {
            NDScript.NDgrpID = CreateNewGroup(NDScript.nodeID);
        }
        int? newNDGrpID = NDScript.NDgrpID;
        
        // Adjacent Node Scripts
        List<NodeScript> adjNDScriptList = NDScript.adjNDScriptList;

        foreach(NodeScript adjNDScript in adjNDScriptList)
        {
            if(adjNDScript != null && adjNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
            {
                int? adjNDGrpID = adjNDScript.NDgrpID;

                if(adjNDGrpID != newNDGrpID && adjNDGrpID != null){
                    JoinGroups(newNDGrpID, adjNDGrpID);
                }

            }
            Debug.Log("adjNode " + adjNDScript.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScript.NDgrpID);
        }

        NDScript.lastPlaced = true;
        lastND_ID = NDScript.nodeID;
    }





    // public void AssignSheepToGroups_REFACTOR(GameObject node)
    // {
    //     NodeScript newScript = node.GetComponent<NodeScript>();                          // Get NodeScript from newNode
    //     newScript.NDgrpID = CreateNewGroup(node);                                        // Create new NodeGroup, assign NDgrpID

    //     List<NodeScript> adjScriptList = newScript.adjNDScriptList;                       // Get List of AdjacentScripts

    //     foreach(NodeScript adjScript in adjScriptList)                                      // Loop over all AdjScripts
    //     {
    //         if(adjScript != null && adjScript.sheepVal == newScript.sheepVal)           // If script is not null, and matches sheepVal (Same color)
    //         {
    //             int adjGrpID = adjScript.NDgrpID;                                         // Save GrpID reference to Delete at end of Method

    //             if(adjScript.NDgrpID != newScript.NDgrpID)                                  // If they are not in the same group
    //             {
    //                 List<GameObject> newGrp = JoinGroups(newScript.NDgrpID, adjScript.NDgrpID);   // Join (Add) all nodes in adjGroup to newGrp

    //                 foreach(GameObject groupNode in newGrp)
    //                 {
    //                     groupNode.GetComponent<NodeScript>().NDgrpID = newScript.NDgrpID;        // Set all newGrp nodes to new ID
    //                 }
    //             }
    //             DeleteGroup(adjGrpID);                                                 // Delete adjGroup by GrpID
    //         }
    //     }

    // }


}