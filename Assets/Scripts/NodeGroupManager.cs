using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeGroupManager : MonoBehaviour
{
    public BoardGenerator boardGeneratorScript;
    public NodeScript nodeScript;

    [System.Serializable]
    public class NodeGroup
    {
        public static int groupCounter = -1;
        
        public int GroupID {get; private set;}
        public int GroupLiberties;
        public List<GameObject> GroupNodeList = new List<GameObject>();

        // Constructor
        public NodeGroup() // Increment the counter and assign it to GroupID
        {
            groupCounter++;
            GroupID = groupCounter;
        }
    }

    public List<NodeGroup> AllGroupList = new List<NodeGroup>();
    
    // Start is called before the first frame update
    void Start()
    {
        boardGeneratorScript = gameObject.GetComponent<BoardGenerator>();

    }
    // Update is called once per frame
    void Update()
    {
        
    }



    // *---------------------------------------- CRUD GROUP METHODS ----------------------------------------
                        //* Add_To_AllGroupList - Updates All Group List 
                        //* CreateNewNodeGroup - Creates New NodeGroup - Called in TargetNode - Returns GroupID
                        //* JoineNodeGroups - Adds all nodes from Previous Group to New Group by ID - Takes new/prev GroupID
                        //* RetrieveNodeGroup - Returns NodeGroup by NodeScript.groupID 
                        //* DeleteNodeGroup - Clears previous group List and removes for AllGroupList 
                        //* CalculateGroupLiberties - Calculates Liberties of all NodeGroups


    public int CreateNewNodeGroup(GameObject node)
    {
        NodeGroup newNodeGroup = new NodeGroup();
        newNodeGroup.GroupNodeList.Add(node);
        Add_To_AllGroupList(newNodeGroup);

        int groupID = newNodeGroup.GroupID;

        Debug.Log("NodeGM : CreateNewNodeGroup: Group ID: [ " + groupID + " ]");

        return groupID;
        
    }


    public List<GameObject> JoinNodeGroups(int newGroupID, int prevGroupID)
    {

        NodeGroup prevNodeGroup = RetrieveNodeGroup(prevGroupID);
        NodeGroup newNodeGroup = RetrieveNodeGroup(newGroupID);

        foreach(GameObject node in prevNodeGroup.GroupNodeList)
        {
            newNodeGroup.GroupNodeList.Add(node);
        }

        Debug.Log("NodeGroupManager:JoinNodeGroups [ " + prevGroupID + " Nodes added to newGroupID " + newGroupID + " ]");

        return newNodeGroup.GroupNodeList;
    }


    public void Add_To_AllGroupList(NodeGroup group)
    {
        AllGroupList.Add(group);
    }


    public NodeGroup RetrieveNodeGroup(int groupID)
    {
        NodeGroup nodeGroup = AllGroupList.Find(g => g.GroupID == groupID);
        return nodeGroup;
    }


    public void DeleteNodeGroup(int prevGroupID)
    {
        NodeGroup groupToDelete = AllGroupList.Find(g => g.GroupID == prevGroupID);
        
        groupToDelete.GroupNodeList.Clear();
        AllGroupList.Remove(groupToDelete);
        
        Debug.Log("NodeGroup " + groupToDelete.GroupID + " cleared and deleted.");
    }

    // Calculates and Updates Liberties for All Groups and stones on the board
    public void CalculateGroupLiberties() 
    {
        foreach(NodeGroup nodeGroup in AllGroupList)
        {
            int totalGroupLiberties = 0;
            List<GameObject> currentGroup = nodeGroup.GroupNodeList;
            List<GameObject> libertyNodes = new List<GameObject>();

            foreach(GameObject node in currentGroup)
            {
                NodeScript nodeScript = node.GetComponent<NodeScript>();

                if(nodeScript.leftNode != null){
                    if(!libertyNodes.Contains(nodeScript.leftNode))
                    {
                        libertyNodes.Add(nodeScript.leftNode);
                        totalGroupLiberties += nodeScript.leftNodeScript.libertyValue;
                    }
                }
                if(nodeScript.rightNode != null){
                    if(!libertyNodes.Contains(nodeScript.rightNode))
                    {
                        libertyNodes.Add(nodeScript.rightNode);
                        totalGroupLiberties += nodeScript.rightNodeScript.libertyValue;
                    }
                }
                if(nodeScript.bottomNode != null){
                    if(!libertyNodes.Contains(nodeScript.bottomNode))
                    {
                        libertyNodes.Add(nodeScript.bottomNode);
                        totalGroupLiberties += nodeScript.bottomNodeScript.libertyValue;
                    }
                }
                if(nodeScript.topNode != null){
                    if(!libertyNodes.Contains(nodeScript.topNode))
                    {
                        libertyNodes.Add(nodeScript.topNode);
                        totalGroupLiberties += nodeScript.topNodeScript.libertyValue;
                    }
                }
                // Debug.Log("node liberties are " + nodeScript.libertyValue);
            }
            
            nodeGroup.GroupLiberties = totalGroupLiberties;
            Debug.Log("currentGroup Liberties are " + totalGroupLiberties);
            Debug.Log("GroupLiberties property is " + nodeGroup.GroupLiberties);
            
            totalGroupLiberties = 0;
            libertyNodes.Clear();

        }
    }

    public void UpdateAllNodeGroups()
    {
        foreach(NodeGroup nodeGroup in AllGroupList)
        {
            List<GameObject> currentGroup = nodeGroup.GroupNodeList;

            if(nodeGroup.GroupLiberties == 0)
            {
                foreach(GameObject node in currentGroup)
                {
                    NodeScript nodeScript = node.GetComponent<NodeScript>();

                    nodeScript.EmptySheepSetter();
                    nodeScript.SetGrassTileDisplayLoop();
                }

            }
        }
    }
}
