using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeGroupManager : MonoBehaviour
{


    [System.Serializable]
    public class NodeGroup
    {
        public static int groupCounter = -1;
        
        public int GroupID {get; private set;}
        public int GroupLiberties;

        public List<GameObject> GroupNodeList = new List<GameObject>();

        // Constructor
        public NodeGroup()
        {
            // Increment the counter and assign it to GroupID
            groupCounter++;
            GroupID = groupCounter;
        }
    }

    public List<NodeGroup> AllGroupList = new List<NodeGroup>();
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSheepToGroup(GameObject node, int groupID)
    {
        NodeGroup nodeGroup = RetrieveNodeGroup(groupID);
        nodeGroup.GroupNodeList.Add(node);

    }

    public void Add_To_AllGroupList(NodeGroup group)
    {
        AllGroupList.Add(group);
    }

    public int CreateNewNodeGroup(GameObject node)
    {
        NodeGroup newNodeGroup = new NodeGroup();
        newNodeGroup.GroupNodeList.Add(node);
        Add_To_AllGroupList(newNodeGroup);

        int groupID = newNodeGroup.GroupID;

        return groupID;
        
    }

    public NodeGroup RetrieveNodeGroup(int groupID)
    {
        NodeGroup nodeGroup = AllGroupList.Find(group => group.GroupID == groupID);

        return nodeGroup;
    }

    public void CalculateGroupLiberties()
    {

        foreach(NodeGroup group in AllGroupList)
        {
            int totalGroupLiberties = 0;
        
            List<GameObject> currentGroup = group.GroupNodeList;

            foreach(GameObject node in currentGroup)
            {
                NodeScript nodeScript = node.GetComponent<NodeScript>();

                if(nodeScript.leftNode != null)
                {
                    totalGroupLiberties += nodeScript.leftNodeScript.libertyValue;
                }
                if(nodeScript.rightNode != null)
                {
                    totalGroupLiberties += nodeScript.rightNodeScript.libertyValue;
                }
                if(nodeScript.bottomNode != null)
                {
                    totalGroupLiberties += nodeScript.bottomNodeScript.libertyValue;
                }
                if(nodeScript.topNode != null)
                {
                    totalGroupLiberties += nodeScript.topNodeScript.libertyValue;
                }

                Debug.Log("node liberties are " + nodeScript.libertyValue);
            }
            
            group.GroupLiberties = totalGroupLiberties;
            Debug.Log("currentGroup Liberties are " + totalGroupLiberties);
            Debug.Log("GroupLiberties property is " + group.GroupLiberties);
            
            totalGroupLiberties = 0;
        }
    }
}
