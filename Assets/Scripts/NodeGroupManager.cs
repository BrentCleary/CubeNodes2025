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
        public static int groupCounter = -1;
        
        public List<GameObject> NodeList = new List<GameObject>();
        public int Grp_ShpVal;

        // Constructor
        public Group()
        {
            groupCounter++;                                                         // Increment the counter and assign it to GrpID
            GrpID = groupCounter;
        }
    }

    public BoardGenerator brd_Gntr_Script;
    public TargetNode targetNDScript;

    public GameObject lastPlacedND;
    public NodeScript lastNDScript;
    public int lastNDSheepVal;
    
    public List<Group> AllGrpList = new List<Group>();



    public void UpdateGroups(GameObject activeNode) //? Called in GameManagerScript //  
    {
        CreateNewGroup(activeNode);
        AssignSheepToGroups(activeNode);                                            // Assign All Groups
        CalculateGrpLiberties();                                                    // Update All Group Liberties
        DeleteZeroLibertyGroupMethods();
    }



    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int? CreateNewGroup(GameObject crntND)   //? Called GameManagerScript    // Creates New Group - Called in TargetNode - Returns GrpID
    {
        NodeScript NDScript = crntND.GetComponent<NodeScript>();
        
        Group newGrp = new Group();
        newGrp.Grp_ShpVal = crntND.GetComponent<NodeScript>().sheepVal;
        newGrp.NodeList.Add(crntND);

        Add_To_AllGrpList(newGrp);
        
        int? newGrpID = newGrp.GrpID;
        NDScript.NDgrpID = newGrp.GrpID;
        
        Debug.Log("NodeGM : CreateNewGroup: Group ID: [ " + newGrpID + " ]");
        
        lastPlacedND = crntND;                                              // ! Sets updates the last placed node for Script Reference
        lastNDSheepVal = NDScript.sheepVal;

        return newGrpID;        
    }
    


    //? ---------------------------------------- STATIC METHODS  ----------------------------------------
    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int? grpID)                                               // Returns Group from AllGrpList by grpID
    {
        Group group = AllGrpList.FirstOrDefault(g => g.GrpID == grpID);
        
        return group;
    }
    //* ---------------------------------------- JoinGroups ----------------------------------------
    public void JoinGroups(int? newGrpID, int? prevGrpID)                           // Adds prevGrp nodes to newGrp by ID - return newGrp.Nodelist 
    {
        Group newGrp = GetGroup(newGrpID);
        Group prevGrp = GetGroup(prevGrpID);

        foreach(GameObject node in prevGrp.NodeList) 
        {
            newGrp.NodeList.Add(node);
            node.GetComponent<NodeScript>().NDgrpID = newGrp.GrpID;
        }

        // DeleteGroup(prevGrpID);
        // Debug.Log("GroupManager: JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");
        
    }
    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int? prevGrpID)                                         // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
    {
        if(prevGrpID != null)
        {
            Group grpToDelete = AllGrpList.FirstOrDefault(g => g.GrpID == prevGrpID);

            Debug.Log(grpToDelete + " is Group #" + grpToDelete.GrpID);

            AllGrpList.Remove(grpToDelete);
            
            Debug.Log("Group " + grpToDelete.GrpID + " cleared and deleted.");
        }
        else
        {
            Debug.Log("GrpID was null");
        }
    }
    //* ---------------------------------------- Add To AllGrpList ----------------------------------------
    public void Add_To_AllGrpList(Group group)                                      // Adds group to AllGrpList
    {
        AllGrpList.Add(group);
    }
    //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
    public void CalculateGrpLiberties()                                             // Updates Liberties of all Groups in AllGrpList
    {
        int totalGrpLibs = 0;
        
        foreach(Group Grp in AllGrpList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            List<GameObject> NDList = Grp.NodeList;
            List<GameObject> countedNDs = new List<GameObject>();                     //! This holds adjacent nodes to prevent double references for lib values 
            
            foreach(GameObject crntND in NDList)                                        // Adds value of node to total liberties
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
        List<int?> zeroGrpIDList = new List<int?>();

        foreach(Group group in AllGrpList)
        {
            if(group.GrpLibs == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroGrpIDList.Add(group.GrpID);
            }
        }
        return zeroGrpIDList;
    }
    public List<Group> GetZeroLibGrp(List<int?> zeroGrpIDList)       // STEP 2  // Receives the zeroLibertyGrpID list from CalculateGrouLiberties()
    {
        List<Group> zeroGrpList = new List<Group>();                                    // Create a new list for sorting

        foreach(Group crntGrp in AllGrpList)                                            // Look through list of All Groups
        {    
            if(zeroGrpIDList.Contains(crntGrp.GrpID))                                  // If the zeroList contains the ID of a Zero'd Node Group
            {
                zeroGrpList.Add(crntGrp);                                               // Add it to the zeroGrpList for updating
            }
        }
        return zeroGrpList;
    }
    public void DeleteZeroLibGrps(List<Group> zeroGrpList)            // STEP 3  // Deletes groups in zeroGrpList
    {
        if(zeroGrpList.Count > 0)
        {
            foreach(Group zeroGrp in zeroGrpList)                                   // Loop of new list of Zero liberty Groups
            {    
                if(zeroGrp.Grp_ShpVal != lastNDSheepVal)                            // ! Checks if zeroGroup is the same sheepVal as current player
                {
                    foreach(GameObject zeroNode in zeroGrp.NodeList)                // Get the script of each Node and Set Node to Empty
                    {
                        NodeScript zeroScript = zeroNode.GetComponent<NodeScript>(); 
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
    public void AssignSheepToGroups(GameObject targetND)
    {
        NodeScript NDScript = targetND.GetComponent<NodeScript>();

        if(NDScript.sheepVal != NDScript.sheepValList[0])
        {
            // Create new NodeGroup, assign grpID
            if(NDScript.NDgrpID == null)
            {
                NDScript.NDgrpID = CreateNewGroup(targetND);
            }
            int? targetNDGrpID = NDScript.NDgrpID;
            
            // Adjacent Node Scripts
            NodeScript leftNDScript = NDScript.leftNDScript;
            NodeScript rightNDScript = NDScript.rightNDScript;
            NodeScript bottomNDScript = NDScript.bottomNDScript;
            NodeScript topNDScript = NDScript.topNDScript;

            //* FIRST CHECK OCCURS ON LEFT NODE
            // Left Node Found
            if(leftNDScript != null && leftNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if adjNode is the same sheepVal as current player                  
            {
                int? leftNDGrpID = leftNDScript.NDgrpID;

                if(leftNDGrpID != targetNDGrpID){
                    JoinGroups(targetNDGrpID, leftNDGrpID);
                }
                
                DeleteGroup(leftNDGrpID);
            }

            // Right Node Found
            if(rightNDScript != null && rightNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if adjNode is the same sheepVal as current player                  
            {
                int? rightNDGrpID = rightNDScript.NDgrpID;

                if(rightNDGrpID != targetNDGrpID){
                    JoinGroups(targetNDGrpID, rightNDGrpID);
                }
                
                DeleteGroup(rightNDGrpID);
            }

            // Bottom Node Found
            if(bottomNDScript != null && bottomNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if adjNode is the same sheepVal as current player                  
            {
                int? bottomNDGrpID = bottomNDScript.NDgrpID;

                if(bottomNDGrpID != targetNDGrpID){
                    JoinGroups(targetNDGrpID, bottomNDGrpID);
                }
                
                DeleteGroup(bottomNDGrpID);
            }

            // Top Node Found
            if(topNDScript != null && topNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if adjNode is the same sheepVal as current player                  
            {
                int? topNDGrpID = topNDScript.NDgrpID;

                if(topNDGrpID != targetNDGrpID){
                    JoinGroups(targetNDGrpID, topNDGrpID);
                }
                
                DeleteGroup(topNDGrpID);
            }

            NDScript.lastPlaced = true;
        }
    }


    //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
    public void AssignSheepToGroups_REFACTOR(GameObject newND)
    {
        NodeScript NDScript = newND.GetComponent<NodeScript>();

        // Create new NodeGroup, assign grpID
        if(NDScript.NDgrpID == null)
        {
            NDScript.NDgrpID = CreateNewGroup(newND);
        }
        int? newNDGrpID = NDScript.NDgrpID;
        
        // Adjacent Node Scripts
        List<NodeScript> adjNDScriptList = NDScript.adjNDScriptList;

        foreach(NodeScript adjNDScript in adjNDScriptList)
        {
            if(adjNDScript != null && adjNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
            {
                int? adjNDGrpID = adjNDScript.NDgrpID;

                if(adjNDGrpID != newNDGrpID){
                    JoinGroups(newNDGrpID, adjNDGrpID);
                }

                DeleteGroup(adjNDGrpID);
            }
            Debug.Log("adjNode " + adjNDScript.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScript.NDgrpID);
        }

        NDScript.lastPlaced = true;
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