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
    public BoardGenerator brd_Gntr_Script;
    public NodeScript NDScript;
    public TargetNode targetNDScript;

    [System.Serializable]
    public class Group
    {
        public static int groupCounter = -1;
        
        public int GrpID {get; private set;}
        public int GrpLiberties;
        public List<GameObject> NodeList = new List<GameObject>();
        public int Grp_sheepVal;

        // Constructor
        public Group() // Increment the counter and assign it to GrpID
        {
            groupCounter++;
            GrpID = groupCounter;
        }
    }

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



    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int CreateNewGroup(GameObject node)      //  Creates New Group - Called in TargetNode - Returns GrpID
    {
        Group newGrp = new Group();
        newGrp.Grp_sheepVal = node.GetComponent<NodeScript>().sheepVal;
        newGrp.NodeList.Add(node);
        
        Add_To_AllGrpList(newGrp);
        
        int grpID = newGrp.GrpID;
        Debug.Log("NodeGM : CreateNewGroup: Group ID: [ " + grpID + " ]");

        return grpID;        
    }


    //* ---------------------------------------- Add_To_AllGrpList ----------------------------------------
    public void Add_To_AllGrpList(Group group)                                    // Updates All Group List
    {
        AllGrpList.Add(group);
    }

    public void Clear_AllGrpList()                                                // Clears the All Group List if all groups Die
    {                                                                               // This should only occur in testing
        if(AllGrpList.Count == 1 && AllGrpList[0].GrpLiberties == 0)
        {
            AllGrpList.Clear();
        }
    }


    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int grpID)                              // Returns Group by NodeScript.grpID 
    {
        Group group = AllGrpList.Find(g => g.GrpID == grpID);
        return group;
    }


    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int prevGrpID)                            // Clears previous group List and removes for AllGrpList 
    {
        Group grpToDelete = AllGrpList.Find(g => g.GrpID == prevGrpID);
        
        grpToDelete.NodeList.Clear();
        AllGrpList.Remove(grpToDelete);
        
        Debug.Log("Group " + grpToDelete.GrpID + " cleared and deleted.");
    }


    //* ---------------------------------------- JoinGroups ----------------------------------------
    public List<GameObject> JoinGroups(int newGrpID, int prevGrpID)     // Adds all nodes from Previous Group to New Group by ID - Takes new/prev GrpID
    {
        Group prevGrp = GetGroup(prevGrpID);
        Group newGrp = GetGroup(newGrpID);

        foreach(GameObject node in prevGrp.NodeList) {
            newGrp.NodeList.Add(node);
        }
        Debug.Log("GroupManager:JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");
        return newGrp.NodeList;
    }


    //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
    public void CalculateGrpLiberties()                                    //  - Updates Liberties of all Groups in AllGrpList
    {
        List<int> zeroGrpIDList = new List<int>();

        foreach(Group group in AllGrpList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            int totalGrpLiberties = 0;
            List<GameObject> nodeList = group.NodeList;
            List<GameObject> libertyNodes = new List<GameObject>();  //! This holds adjacent nodes to prevent double references for liberty values 
            
            foreach(GameObject node in nodeList)    // Adds value of node to total liberties, adds node to list so it is not counted twice
            {
                NodeScript NDScript = node.GetComponent<NodeScript>();                // Node script reference to get all information

                if(NDScript.leftND != null){                                        // If the adj node is not empty
                    if(libertyNodes.Contains(NDScript.leftND) == false)             // If the node is not already in the script (has been counted)
                    {
                        libertyNodes.Add(NDScript.leftND);                          // Add it to the liberty node list
                        totalGrpLiberties += NDScript.leftNDScript.libertyVal;  // Add it's liberty value to the group ( 1 or 0 )
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
                // Debug.Log("node liberties are " + NDScript.libertyValue);
            }
            
            group.GrpLiberties = totalGrpLiberties;
            Debug.Log("currentGroup Liberties are " + totalGrpLiberties);
            Debug.Log("GrpLiberties property is " + group.GrpLiberties);

            totalGrpLiberties = 0;
            libertyNodes.Clear();
        }

    }


    public List<int> GetZeroLibertyGrpID()     // Goes over AllGroupsList and returns a list of all Group Liberties
    {
        List<int> zeroLibertyGrpList = new List<int>();

        foreach(Group group in AllGrpList){
            if(group.GrpLiberties == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroLibertyGrpList.Add(group.GrpID);
            }
        }
        return zeroLibertyGrpList;
    }


    public void UpdateZeroLibertyGroups(List<int> zeroGrpIDList)   // Receives the zeroLibertyGrpID list from CalculateGrouLiberties()
    {
        List<Group> zeroGrpList = new List<Group>();                                  // Create a new list for sorting

        foreach(Group group in AllGrpList)                                            // Look through list of All Groups
        {    
            if(zeroGrpIDList.Contains(group.GrpID)){                                // If the zeroList contains the ID of a Zero'd Node Group
                zeroGrpList.Add(group);                                               // Add it to the zeroGrpList for updating
            }

            foreach(Group zeroGrp in zeroGrpList)                                   // ? Loop of new list of Zero liberty Groups
            {   
                if(zeroGrp.NodeList.Count > 1)                                        //! If the group has more than 1 stone. Allows for captures in KO.
                {
                    List<GameObject> zeroList = zeroGrp.NodeList;                     // Get a list of the Nodes in the group

                    foreach(GameObject zeroNode in zeroList)                            // For each Node
                    {
                        NodeScript zeroScript = zeroNode.GetComponent<NodeScript>();    // Get the script of the node
                        zeroScript.PlaceEmptySheepMethod();

                        Debug.Log("Setting Node " + zeroNode.name + " to empty");
                    }
                }

                // TODO LOGIC HERE FOR KO AND OTHER SINGLE NODE SITUATIONS                // Update this space for KO an other empty single space logic
            
            }
        }

        // Placed in method for debug 10/02/24
        Clear_AllGrpList();
        

    }


    //* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
                            //* Highlights/Resets selected Nodes Color by changing GrassTiles materials 

    public void AssignSheepToGroups(GameObject targetNode)
    {
        NodeScript NDScript = targetNode.GetComponent<NodeScript>();

        // Create new NodeGroup, assign grpID
        NDScript.grpID = CreateNewGroup(targetNode);
        int targetNodeGrpID = NDScript.grpID;
        
        // Adjacent Node Scripts
        NodeScript leftNDScript = NDScript.leftNDScript;
        NodeScript rightNDScript = NDScript.rightNDScript;
        NodeScript bottomNDScript = NDScript.bottomNDScript;
        NodeScript topNDScript = NDScript.topNDScript;


        //* FIRST CHECK OCCURS ON LEFT NODE
        // Left Node Found
        if(leftNDScript != null && leftNDScript.sheepVal == NDScript.sheepVal)
        {
            int leftNDGrpID = leftNDScript.grpID;

            List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, leftNDGrpID);

            foreach(GameObject crntND in nodeGroup)
            {
                crntND.GetComponent<NodeScript>().grpID = targetNodeGrpID;
            }

            DeleteGroup(leftNDGrpID);

        }

        // Right Node Found
        if(rightNDScript != null && rightNDScript.sheepVal == NDScript.sheepVal)
        {
            int rightNDGrpID = rightNDScript.grpID;

            if(rightNDGrpID != targetNodeGrpID)
            {
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, rightNDGrpID);

                foreach(GameObject crntND in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + targetNodeGrpID);
                    Debug.Log("prevNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                    crntND.GetComponent<NodeScript>().grpID = targetNodeGrpID;
                    Debug.Log("newNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                }

                DeleteGroup(rightNDGrpID);

                Debug.Log("New Node added to GrpID: " + rightNDScript.grpID);
            }
        }

        // Bottom Node Found
        if(bottomNDScript != null && bottomNDScript.sheepVal == NDScript.sheepVal)
        {
            int bottomNDGrpID = bottomNDScript.grpID;

            if(bottomNDGrpID != targetNodeGrpID)
            {
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, bottomNDGrpID);

                foreach(GameObject crntND in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + targetNodeGrpID);
                    Debug.Log("prevNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                    crntND.GetComponent<NodeScript>().grpID = targetNodeGrpID;
                    Debug.Log("newNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                }

                DeleteGroup(bottomNDGrpID);

                Debug.Log("New Node added to GrpID: " + bottomNDScript.grpID);
            }
        }

        // Top Node Found
        if(topNDScript != null && topNDScript.sheepVal == NDScript.sheepVal)
        {
            int topNDGrpID = topNDScript.grpID;
            
            if(topNDGrpID != targetNodeGrpID)
            {
                List<GameObject> nodeGroup = JoinGroups(targetNodeGrpID, topNDGrpID);

                foreach(GameObject crntND in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + targetNodeGrpID);
                    Debug.Log("prevNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                    crntND.GetComponent<NodeScript>().grpID = targetNodeGrpID;
                    Debug.Log("newNodeID's are " + crntND.GetComponent<NodeScript>().grpID);
                }

                DeleteGroup(topNDGrpID);

                Debug.Log("New Node added to GrpID: " + topNDScript.grpID);
            }
        }
    }





    // public void AssignSheepToGroups_REFACTOR(GameObject node)
    // {
    //     NodeScript newScript = node.GetComponent<NodeScript>();                          // Get NodeScript from newNode
    //     newScript.grpID = CreateNewGroup(node);                                        // Create new NodeGroup, assign grpID

    //     List<NodeScript> adjScriptList = newScript.adjNDScriptList;                       // Get List of AdjacentScripts

    //     foreach(NodeScript adjScript in adjScriptList)                                      // Loop over all AdjScripts
    //     {
    //         if(adjScript != null && adjScript.sheepVal == newScript.sheepVal)           // If script is not null, and matches sheepVal (Same color)
    //         {
    //             int adjGrpID = adjScript.grpID;                                         // Save GrpID reference to Delete at end of Method

    //             if(adjScript.grpID != newScript.grpID)                                  // If they are not in the same group
    //             {
    //                 List<GameObject> newGrp = JoinGroups(newScript.grpID, adjScript.grpID);   // Join (Add) all nodes in adjGroup to newGrp

    //                 foreach(GameObject groupNode in newGrp)
    //                 {
    //                     groupNode.GetComponent<NodeScript>().grpID = newScript.grpID;        // Set all newGrp nodes to new ID
    //                 }
    //             }
    //             DeleteGroup(adjGrpID);                                                 // Delete adjGroup by GrpID
    //         }
    //     }

    // }


}