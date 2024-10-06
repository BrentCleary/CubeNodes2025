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
    public BoardGenerator boardGeneratorScript;
    public NodeScript nodeScript;
    public TargetNode targetNodeScript;

    [System.Serializable]
    public class Group
    {
        public static int groupCounter = -1;
        
        public int GroupID {get; private set;}
        public int GroupLiberties;
        public List<GameObject> NodeList = new List<GameObject>();

        // Constructor
        public Group() // Increment the counter and assign it to GroupID
        {
            groupCounter++;
            GroupID = groupCounter;
        }
    }

    public List<Group> AllGroupList = new List<Group>();
    


    // Start is called before the first frame update
    void Start()
    {
        boardGeneratorScript = gameObject.GetComponent<BoardGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }





    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int CreateNewGroup(GameObject node)      //  Creates New Group - Called in TargetNode - Returns GroupID
    {
        Group newGroup = new Group();
        newGroup.NodeList.Add(node);
        
        Add_To_AllGroupList(newGroup);
        
        int groupID = newGroup.GroupID;
        Debug.Log("NodeGM : CreateNewGroup: Group ID: [ " + groupID + " ]");

        return groupID;        
    }


    //* ---------------------------------------- Add_To_AllGroupList ----------------------------------------
    public void Add_To_AllGroupList(Group group)                                    // Updates All Group List
    {
        AllGroupList.Add(group);
    }

    public void Clear_AllGroupList()                                                // Clears the All Group List if all groups Die
    {                                                                               // This should only occur in testing
        if(AllGroupList.Count == 1 && AllGroupList[0].GroupLiberties == 0)
        {
            AllGroupList.Clear();
            AllGroupList = new List<Group>();  // Creates a new list. Was not working correctly using Clear()   10/02/24
        }
    }


    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int groupID)                              // Returns Group by NodeScript.groupID 
    {
        Group group = AllGroupList.Find(g => g.GroupID == groupID);
        return group;
    }


    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int prevGroupID)                            // Clears previous group List and removes for AllGroupList 
    {
        Group groupToDelete = AllGroupList.Find(g => g.GroupID == prevGroupID);
        
        groupToDelete.NodeList.Clear();
        AllGroupList.Remove(groupToDelete);
        
        Debug.Log("Group " + groupToDelete.GroupID + " cleared and deleted.");
    }


    //* ---------------------------------------- JoinGroups ----------------------------------------
    public List<GameObject> JoinGroups(int newGroupID, int prevGroupID)     // Adds all nodes from Previous Group to New Group by ID - Takes new/prev GroupID
    {
        Group prevGroup = GetGroup(prevGroupID);
        Group newGroup = GetGroup(newGroupID);

        foreach(GameObject node in prevGroup.NodeList) {
            newGroup.NodeList.Add(node);
        }
        Debug.Log("GroupManager:JoinGroups [ " + prevGroupID + " Nodes added to newGroupID " + newGroupID + " ]");
        return newGroup.NodeList;
    }



    //* ---------------------------------------- CalculateGroupLiberties ----------------------------------------
    public void CalculateGroupLiberties()                                    //  - Updates Liberties of all Groups in AllGroupList
    {
        List<int> zeroGroupIDList = new List<int>();

        foreach(Group group in AllGroupList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            int totalGroupLiberties = 0;
            List<GameObject> nodeList = group.NodeList;
            List<GameObject> libertyNodes = new List<GameObject>();  //! This holds adjacent nodes to prevent double references for liberty values 
            
            foreach(GameObject node in nodeList)    // Adds value of node to total liberties, adds node to list so it is not counted twice
            {
                NodeScript nodeScript = node.GetComponent<NodeScript>();                // Node script reference to get all information

                if(nodeScript.leftNode != null){                                        // If the adj node is not empty
                    if(libertyNodes.Contains(nodeScript.leftNode) == false)             // If the node is not already in the script (has been counted)
                    {
                        libertyNodes.Add(nodeScript.leftNode);                          // Add it to the liberty node list
                        totalGroupLiberties += nodeScript.leftNodeScript.libertyValue;  // Add it's liberty value to the group ( 1 or 0 )
                    }
                }
                if(nodeScript.rightNode != null){
                    if(libertyNodes.Contains(nodeScript.rightNode) == false)
                    {
                        libertyNodes.Add(nodeScript.rightNode);
                        totalGroupLiberties += nodeScript.rightNodeScript.libertyValue;
                    }
                }
                if(nodeScript.bottomNode != null){
                    if(libertyNodes.Contains(nodeScript.bottomNode) == false)
                    {
                        libertyNodes.Add(nodeScript.bottomNode);
                        totalGroupLiberties += nodeScript.bottomNodeScript.libertyValue;
                    }
                }
                if(nodeScript.topNode != null){
                    if(libertyNodes.Contains(nodeScript.topNode) == false)
                    {
                        libertyNodes.Add(nodeScript.topNode);
                        totalGroupLiberties += nodeScript.topNodeScript.libertyValue;
                    }
                }
                // Debug.Log("node liberties are " + nodeScript.libertyValue);
            }
            
            group.GroupLiberties = totalGroupLiberties;
            Debug.Log("currentGroup Liberties are " + totalGroupLiberties);
            Debug.Log("GroupLiberties property is " + group.GroupLiberties);

            totalGroupLiberties = 0;
            libertyNodes.Clear();
        }

    }


    public List<int> GetZeroLibertyGroupID()     // Goes over AllGroupsList and returns a list of all Group Liberties
    {
        List<int> zeroLibertyGroupList = new List<int>();

        foreach(Group group in AllGroupList){
            if(group.GroupLiberties == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroLibertyGroupList.Add(group.GroupID);
            }
        }

        return zeroLibertyGroupList;
    }


    public void UpdateZeroLibertyGroups(List<int> zeroGroupIDList)   // Receives the zeroLibertyGroupID list from CalculateGrouLiberties()
    {
        List<Group> zeroGroupList = new List<Group>();                                  // Create a new list for sorting

        foreach(Group group in AllGroupList)                                           // Look through list of All Groups
        {    
            if(zeroGroupIDList.Contains(group.GroupID)){                                // If the zeroList contains the ID of a Zero'd Node Group
                zeroGroupList.Add(group);                                               // Add it to the zeroGroupList for updating
            }

            foreach(Group zeroGroup in zeroGroupList)                                   // Loop of new list of Zero liberty Groups
            {   
                if(zeroGroup.NodeList.Count > 1)                                        // If the group has more than 1 stone. Allows for captures. 
                {
                    List<GameObject> zeroList = zeroGroup.NodeList;                     // Get a list of the Nodes in the group

                    foreach(GameObject zeroNode in zeroList)                            // For each Node
                    {
                        NodeScript zeroScript = zeroNode.GetComponent<NodeScript>();    // Get the script of the node
                        zeroScript.EmptySheepSetter();

                        Debug.Log("Setting Node " + zeroNode.name + " to empty");
                    }
                }

                // TODO LOGIC HERE FOR KO AND OTHER SINGLE NODE SITUATIONS          // Update this space for KO an other empty single space logic
            
            }
        }

        // Placed in method for debug 10/02/24
        Clear_AllGroupList();
        

    }


    public void AssignSheepToGroups(GameObject node)
    {
        NodeScript newScript = node.GetComponent<NodeScript>();                          // Get NodeScript from newNode
        newScript.groupID = CreateNewGroup(node);                                        // Create new NodeGroup, assign groupID

        List<NodeScript> adjScriptList = newScript.adjNodeScriptList;                       // Get List of AdjacentScripts

        foreach(NodeScript adjScript in adjScriptList)                                      // Loop over all AdjScripts
        {
            if(adjScript != null && adjScript.sheepValue == newScript.sheepValue)           // If script is not null, and matches sheepValue (Same color)
            {
                int adjGroupID = adjScript.groupID;                                         // Save GroupID reference to Delete at end of Method

                if(adjScript.groupID != newScript.groupID)                                  // If they are not in the same group
                {
                    List<GameObject> newGroup = JoinGroups(newScript.groupID, adjScript.groupID);   // Join (Add) all nodes in adjGroup to newGroup

                    foreach(GameObject groupNode in newGroup)
                    {
                        groupNode.GetComponent<NodeScript>().groupID = newScript.groupID;        // Set all newGroup nodes to new ID
                    }
                }
                DeleteGroup(adjGroupID);                                                 // Delete adjGroup by GroupID
            }
        }

    }


}
