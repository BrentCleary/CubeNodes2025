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
        public static int groupCounter = -1;
        
        public int? GrpID {get; set;}
        public int GrpLiberties;
        public List<GameObject> NodeList = new List<GameObject>();
        public int Grp_sheepVal;

        // Constructor
        public Group()
        {
            groupCounter++;                     // Increment the counter and assign it to GrpID
            GrpID = groupCounter;
        }
    }

    public BoardGenerator brd_Gntr_Script;
    public NodeScript NDScript;
    public TargetNode targetNDScript;

    public GameObject lastPlacedND;
    public NodeScript lastNDScript;
    public int lastNDSheepVal;
    
    public List<Group> AllGrpList = new List<Group>();


    // Start is called before the first frame update
    void Start()
    {
        brd_Gntr_Script = gameObject.GetComponent<BoardGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGroupsMethod(GameObject activeNode)
    {
        AssignSheepToGroups(activeNode);                              // Assign All Groups
        CalculateGrpLiberties();                                    // Update All Group Liberties

        List<int?> zeroGrpIDList;
        zeroGrpIDList = GetZeroLibertyGrpID();                       // Update All Group Liberties
        List<Group> zeroGrpList = GetZeroLibertyGrp(zeroGrpIDList);                        // Delete Groups with 0 Liberties
        DeleteZeroLibertyGrp(zeroGrpList);
    }





    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int? CreateNewGroup(GameObject node)                                   // Creates New Group - Called in TargetNode - Returns GrpID
    {
        lastPlacedND = node;                                              // ! Sets updates the last placed node for Script Reference
        lastNDScript = lastPlacedND.GetComponent<NodeScript>();
        lastNDSheepVal = lastNDScript.sheepVal;

        Group newGrp = new Group();
        newGrp.Grp_sheepVal = node.GetComponent<NodeScript>().sheepVal;
        newGrp.NodeList.Add(node);

        Add_To_AllGrpList(newGrp);
        
        int? newGrpID = newGrp.GrpID;
        Debug.Log("NodeGM : CreateNewGroup: Group ID: [ " + newGrpID + " ]");

        return newGrpID;        
    }
    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int? grpID)                                             // Returns Group from AllGrpList by grpID
    {
        Group group = AllGrpList.Find(g => g.GrpID == grpID);
        return group;
    }
    //* ---------------------------------------- JoinGroups ----------------------------------------
    public List<GameObject> JoinGroups(int? newGrpID, int? prevGrpID)             // Adds prevGrp nodes to newGrp by ID - return newGrp.Nodelist 
    {
        Group newGrp = GetGroup(newGrpID);
        Group prevGrp = GetGroup(prevGrpID);

        foreach(GameObject node in prevGrp.NodeList) {
            newGrp.NodeList.Add(node);
        }
        Debug.Log("GroupManager: JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");
        return newGrp.NodeList;
    }
    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int? prevGrpID)                                       // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
    {
        Group grpToDelete = AllGrpList.Find(g => g.GrpID == prevGrpID);
        
        grpToDelete.NodeList.Clear();
        AllGrpList.Remove(grpToDelete);
        
        Debug.Log("Group " + grpToDelete.GrpID + " cleared and deleted.");
    }


    //* ---------------------------------------- Add To AllGrpList ----------------------------------------
    public void Add_To_AllGrpList(Group group)                                    // Adds group to AllGrpList
    {
        AllGrpList.Add(group);
    }


    //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
    public void CalculateGrpLiberties()                                           // Updates Liberties of all Groups in AllGrpList
    {
        int totalGrpLiberties = 0;
        
        foreach(Group crntGrp in AllGrpList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            List<GameObject> nodeList = crntGrp.NodeList;
            List<GameObject> libertyNodes = new List<GameObject>();                     //! This holds adjacent nodes to prevent double references for lib values 
            
            foreach(GameObject crntNode in nodeList)                                        // Adds value of node to total liberties
            {
                NodeScript NDScript = crntNode.GetComponent<NodeScript>();                  // Node script reference to get all information

                if(NDScript.leftND != null){                                            // If the adj node is not empty
                    if(libertyNodes.Contains(NDScript.leftND) == false)                 // If the node is not already in the script (has been counted)
                    {
                        libertyNodes.Add(NDScript.leftND);                              // Add it to the liberty node list
                        totalGrpLiberties += NDScript.leftNDScript.libertyVal;          // Add it's liberty value to the group ( 1 or 0 )
                    }
                }
                if(NDScript.rightND != null){
                    if(libertyNodes.Contains(NDScript.rightND) == false)
                    {
                        libertyNodes.Add(NDScript.rightND);
                        totalGrpLiberties += NDScript.rightNDScript.libertyVal;
                    }
                }
                if(NDScript.bottomND != null){
                    if(libertyNodes.Contains(NDScript.bottomND) == false)
                    {
                        libertyNodes.Add(NDScript.bottomND);
                        totalGrpLiberties += NDScript.bottomNDScript.libertyVal;
                    }
                }
                if(NDScript.topND != null){
                    if(libertyNodes.Contains(NDScript.topND) == false)
                    {
                        libertyNodes.Add(NDScript.topND);
                        totalGrpLiberties += NDScript.topNDScript.libertyVal;
                    }
                }
            }
            
            crntGrp.GrpLiberties = totalGrpLiberties;

            totalGrpLiberties = 0;
            libertyNodes.Clear();
        }

    }


    //* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
    public List<int?> GetZeroLibertyGrpID()                                       // Returns list from AllGrpList with GrpLiberties = 0
    {
        List<int?> zeroLibertyGrpList = new List<int?>();

        foreach(Group group in AllGrpList)
        {
            if(group.GrpLiberties == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroLibertyGrpList.Add(group.GrpID);
            }
        }
        return zeroLibertyGrpList;
    }

    public List<Group> GetZeroLibertyGrp(List<int?> zeroGrpIDList)                // Receives the zeroLibertyGrpID list from CalculateGrouLiberties()
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

    public void DeleteZeroLibertyGrp(List<Group> zeroGrpList)                     // Deletes groups in zeroGrpList
    {
        if(zeroGrpList.Count > 0)
        {
            foreach(Group zeroGrp in zeroGrpList)                                           // ? Loop of new list of Zero liberty Groups
            {    
                if(zeroGrp.Grp_sheepVal != lastNDSheepVal)                                  // ! Checks if zeroGroup is the same sheepVal as current player
                {
                    foreach(GameObject zeroNode in zeroGrp.NodeList)                        // For each Node
                    {
                        NodeScript zeroScript = zeroNode.GetComponent<NodeScript>();        // Get the script of the node
                        zeroScript.PlaceEmptySheepMethod();
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
    public void AssignSheepToGroups(GameObject targetNode)
    {
        NodeScript NDScript = targetNode.GetComponent<NodeScript>();

        // Create new NodeGroup, assign grpID
        NDScript.NDgrpID = CreateNewGroup(targetNode);
        int? targetNodeGrpID = NDScript.NDgrpID;
        
        // Adjacent Node Scripts
        NodeScript leftNDScript = NDScript.leftNDScript;
        NodeScript rightNDScript = NDScript.rightNDScript;
        NodeScript bottomNDScript = NDScript.bottomNDScript;
        NodeScript topNDScript = NDScript.topNDScript;

        //* FIRST CHECK OCCURS ON LEFT NODE
        // Left Node Found
        if(leftNDScript != null && leftNDScript.sheepVal == NDScript.sheepVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
        {
            int? leftNDGrpID = leftNDScript.NDgrpID;

            if(leftNDGrpID != targetNodeGrpID){
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, leftNDGrpID);

                foreach(GameObject crntND in nodeGroup) {
                    NodeScript crntNDScript = crntND.GetComponent<NodeScript>();                // Gets nodeScript
                    crntNDScript.NDgrpID = targetNodeGrpID;                                       // Sets node to new GrpID
                    crntNDScript.lastPlaced = false;                                           // Sets lastClicked status to false
                }

                DeleteGroup(leftNDGrpID);
            }
        }

        // Right Node Found
        if(rightNDScript != null && rightNDScript.sheepVal == NDScript.sheepVal)
        {
            int? rightNDGrpID = rightNDScript.NDgrpID;

            if(rightNDGrpID != targetNodeGrpID){
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, rightNDGrpID);

                foreach(GameObject crntND in nodeGroup) {
                    NodeScript crntNDScript = crntND.GetComponent<NodeScript>();                // Gets nodeScript
                    crntNDScript.NDgrpID = targetNodeGrpID;                                       // Sets node to new GrpID
                    crntNDScript.lastPlaced = false;                                           // Sets lastClicked status to false
                }

                DeleteGroup(rightNDGrpID);
            }
        }

        // Bottom Node Found
        if(bottomNDScript != null && bottomNDScript.sheepVal == NDScript.sheepVal)
        {
            int? bottomNDGrpID = bottomNDScript.NDgrpID;

            if(bottomNDGrpID != targetNodeGrpID) {
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, bottomNDGrpID);

                foreach(GameObject crntND in nodeGroup) {
                    NodeScript crntNDScript = crntND.GetComponent<NodeScript>();                // Gets nodeScript
                    crntNDScript.NDgrpID = targetNodeGrpID;                                       // Sets node to new GrpID
                    crntNDScript.lastPlaced = false;                                           // Sets lastClicked status to false
                }

                DeleteGroup(bottomNDGrpID);
            }
        }

        // Top Node Found
        if(topNDScript != null && topNDScript.sheepVal == NDScript.sheepVal)
        {
            int? topNDGrpID = topNDScript.NDgrpID;
            
            if(topNDGrpID != targetNodeGrpID) {
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, topNDGrpID);

                foreach(GameObject crntND in nodeGroup) {
                    NodeScript crntNDScript = crntND.GetComponent<NodeScript>();                // Gets nodeScript
                    crntNDScript.NDgrpID = targetNodeGrpID;                                       // Sets node to new GrpID
                    crntNDScript.lastPlaced = false;                                           // Sets lastClicked status to false
                }

                DeleteGroup(topNDGrpID);
            }
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