using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using Diag = System.Diagnostics;
using System.Linq;
using System.Globalization;
using System;
using UnityEditor.Build;
using UnityEngine.XR;

public class GroupManagerScript : MonoBehaviour
{
    [System.Serializable]
    public class Group
    {
        public int GrpID;
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

    [SerializeField] public List<Group> All_Grp_List;
    public int lastND_ID;
    public int lastND_ShpVal;
    public int lastGrpID;

    public int testVariable;


    public BoardGenerator Brd_Gnt_Scr;
    public NodeScript lastND_Script;

    public TargetNode targetNDScript;

    public GameObject debugNodeCheck;
    public List<GameObject> All_ND_List;
    public List<int> All_ND_GrpID_List;

    public DebugMethods debugScript;


    public bool perform_Ko_Check;


    public void Start()
    {
        Brd_Gnt_Scr = GetComponent<BoardGenerator>();
        All_Grp_List = new List<Group>();

        testVariable = 123;

        debugScript = GetDebugMethods();
    }

    private void Update()
    {
        if (All_ND_List.Count < 1 && Brd_Gnt_Scr.ND_List != null)
        {
            All_ND_List = Brd_Gnt_Scr.ND_List;
        }
    }


    public void CreateGroup_Method(int crntND_ID)          //? Called in GameManagerScript //
    {
        CreateNewGroup(crntND_ID);                                                 // Creates newGrp, Associate Properties
        AssignSheepToGroups(crntND_ID);                                            // Assign Adj Grps to newGrpID
    }

    public void UpdateGroups_Method()                      //? Called in GameManagerScript //  
    {
        CalculateGrpLiberties();                                                   // Update All Group Liberties
        DeleteZeroLibertyGroup_Methods();                                          // MethodGroup to Delete Grps with 0 Liberties, Set Empty ND
        CalculateGrpLiberties();                                                   // Update All Group Liberties (Post ZeroLibGrp Deletion)
    }



    //* ---------------------------------------- CreateNewGroup  ----------------------------------------
    public int CreateNewGroup(int crntND_ID)   //? Called GameManagerScript    // Creates New Group - Called in TargetNode - Returns GrpID
    {
        GameObject crntND = GetNodeWithID(All_ND_List, crntND_ID);
        NodeScript ND_Scr = crntND.GetComponent<NodeScript>();
        
        //~ Group Attributes
        Group newGrp = new Group();
        int newGrpID = newGrp.GrpID;
        newGrp.Grp_ShpVal = crntND.GetComponent<NodeScript>().shpVal;
        
        //~ Associate Group, Node, List
        ND_Scr.NDGrpID = newGrpID;
        newGrp.ND_ID_List.Add(ND_Scr.NDID);

        //~ Update Global Properties
        All_Grp_List.Add(newGrp);        
        lastND_ID = crntND_ID;                                              // ! Sets last placed ND_ID for Script Reference
        lastND_ShpVal = ND_Scr.shpVal;
        
        debugNodeCheck = crntND;
        lastGrpID = newGrpID;           // TODO Check on change 

        // debugScript.LogCurrentLine();

        return newGrpID;        
    }
    //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
    public void AssignSheepToGroups(int targetND_ID)
    {
        GameObject targetND = GetNodeWithID(All_ND_List, targetND_ID);
        NodeScript ND_Scr = targetND.GetComponent<NodeScript>();

        if(ND_Scr.shpVal != ND_Scr.shpValList[0])
        {
            // Create new NodeGroup, assign grpID
            if(ND_Scr.NDGrpID == -1)
            {
                ND_Scr.NDGrpID = CreateNewGroup(ND_Scr.NDID);
            }
            int targetNDGrpID = ND_Scr.NDGrpID;
            
            // Adjacent Node Scripts
            NodeScript LND_Scr = ND_Scr.LNDScr;
            NodeScript RND_Scr = ND_Scr.RNDScr;
            NodeScript BND_Scr = ND_Scr.BNDScr;
            NodeScript TND_Scr = ND_Scr.TNDScr;

            //* FIRST CHECK OCCURS ON LEFT NODE
            // Left Node Found
            if(LND_Scr != null && LND_Scr.shpVal == ND_Scr.shpVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int LNDGrpID = LND_Scr.NDGrpID;

                if(LNDGrpID != targetNDGrpID && LNDGrpID != -1){
                    JoinGroups(targetNDGrpID, LNDGrpID);
                }
            }
            // Right Node Found
            if(RND_Scr != null && RND_Scr.shpVal == ND_Scr.shpVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int RNDGrpID = RND_Scr.NDGrpID;

                if(RNDGrpID != targetNDGrpID && RNDGrpID != -1){
                    JoinGroups(targetNDGrpID, RNDGrpID);
                }
            }
            // Bottom Node Found
            if(BND_Scr != null && BND_Scr.shpVal == ND_Scr.shpVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int BNDGrpID = BND_Scr.NDGrpID;
                if(BNDGrpID != targetNDGrpID && BNDGrpID != -1){
                    JoinGroups(targetNDGrpID, BNDGrpID);
                }
            }
            // Top Node Found
            if(TND_Scr != null && TND_Scr.shpVal == ND_Scr.shpVal){                   // ! Checks if adjNode is the same sheepVal as current player                  
                int TNDGrpID = TND_Scr.NDGrpID;

                if(TNDGrpID != targetNDGrpID && TNDGrpID != -1){
                    JoinGroups(targetNDGrpID, TNDGrpID);
                }
            }

            ND_Scr.lastPlaced = true;
            lastND_ID = ND_Scr.NDID;
        }
    }


    //? ---------------------------------------- STATIC METHODS  ----------------------------------------
    //* ---------------------------------------- GetNodeWithID ----------------------------------------
    // ------ Get NODE from nodeID in gNodeArray -------
    public GameObject GetNodeWithID(List<GameObject> gameObjectList, int targetID)
    {
        foreach (GameObject obj in gameObjectList)
        {
            NodeScript nodeScript = obj.GetComponent<NodeScript>();
            if (nodeScript != null && nodeScript.NDID == targetID)
            {
                return obj; // Found the GameObject with the target ID
            }
        }
        return null; // Return null if no GameObject with the target ID is found
    }
    //* ---------------------------------------- GetGroup ----------------------------------------
    public Group GetGroup(int grpID)                                                // Returns Group from AllGrpList by grpID
    {
        Group group = All_Grp_List.FirstOrDefault(g => g.GrpID == grpID);
        
        return group;
    }
    //* ---------------------------------------- JoinGroups ----------------------------------------
    public void JoinGroups(int newGrpID, int prevGrpID)                             // Adds prevGrpIDs to newGrp.ND_ID_List - return newGrp.ND_ID_List 
    {
        if(prevGrpID != -1 && prevGrpID != newGrpID)                                // if prevGrpID is not null and doesn't match the newGrp 
        {
            Group newGrp = GetGroup(newGrpID);                                      // Get NewGroup 
            Group prevGrp = GetGroup(prevGrpID);                                    // Get PrevGroup

            foreach(int ID in prevGrp.ND_ID_List)                                   // For each ID in prevGrp 
            {
                newGrp.ND_ID_List.Add(ID);                                          // Add ID to newGrp 
                GameObject node = GetNodeWithID(All_ND_List, ID);                   // Get Node with ID 
                node.GetComponent<NodeScript>().NDGrpID = newGrp.GrpID;             // Update GrpID on NodeScript 
            }

            DeleteGroup(prevGrpID);                                                 // Delete prevGrp from All_Grp_List 
        }

        // DeleteGroup(prevGrpID);
        // Debug.Log("GroupManager: JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");
        
    }
    //* ---------------------------------------- DeleteGroup -----------------------------------------
    public void DeleteGroup(int delGrpID)                                           // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
    {
        if(delGrpID != -1 && delGrpID != lastGrpID) 
        {
            Group grpToDelete = All_Grp_List.FirstOrDefault(g => g.GrpID == delGrpID);
            
            All_Grp_List.Remove(grpToDelete);
            
            Debug.Log("Group #" + grpToDelete.GrpID + " cleared and deleted.");
            // debugScript.LogCallerMethod();
        }
        else {
            Debug.Log("GrpID was -1");
        }
    }
    //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
    public void CalculateGrpLiberties()                                               // Updates Liberties of all Groups in AllGrpList
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
            
            foreach(GameObject crntND in GrpNDList)                                   // Adds value of node to total liberties
            {
                NodeScript NDScr = crntND.GetComponent<NodeScript>();              // Node script reference to get all information

                if(NDScr.LND != null){                                          // If the adj node is not empty
                    if(countedNDs.Contains(NDScr.LND) == false)                 // If the node is not already in the script (has been counted)
                    {
                        countedNDs.Add(NDScr.LND);                              // Add it to the liberty node list
                        totalGrpLibs += NDScr.LNDScr.libVal;             // Add it's liberty value to the group ( 1 or 0 )
                    }
                }
                if(NDScr.RND != null){
                    if(countedNDs.Contains(NDScr.RND) == false)
                    {
                        countedNDs.Add(NDScr.RND);
                        totalGrpLibs += NDScr.RNDScr.libVal;
                    }
                }
                if(NDScr.BND != null){
                    if(countedNDs.Contains(NDScr.BND) == false)
                    {
                        countedNDs.Add(NDScr.BND);
                        totalGrpLibs += NDScr.BNDScr.libVal;
                    }
                }
                if(NDScr.TND != null){
                    if(countedNDs.Contains(NDScr.TND) == false)
                    {
                        countedNDs.Add(NDScr.TND);
                        totalGrpLibs += NDScr.TNDScr.libVal;
                    }
                }
            }
            
            Grp.GrpLibs = totalGrpLibs;

            totalGrpLibs = 0;
            countedNDs.Clear();
        }

    }



    public bool Check_IsPlaceAble(int ND_ID, int crntShpVal)                              // Calculate Group Capture
    {
        bool isPlaceAble = false;                                                     // Set isPlaceAble bool default at false

        GameObject crntND = GetNodeWithID(All_ND_List, ND_ID);                        // Get TargetNode
        NodeScript crntNDScript = crntND.GetComponent<NodeScript>();                  // Get TargetNode Script
        List<NodeScript> adjNDScriptList = crntNDScript.adjNDScrList;              // Get adjNDScriptList to iterate over Group Capture
        
        List<int> adjND_GrpIDList = new List<int>();
        List<Group> adjGrpList = new List<Group>();                                   // Get ajd Group and add to list

        //? --------------------------------------------------------------------------// Get AdjGrpID and AdjGrp Lists
        foreach(NodeScript ND_SCR in adjNDScriptList)                                 // Get ajdND Group ID and add to list
        {
            if(ND_SCR != null && adjND_GrpIDList.Contains(ND_SCR.NDGrpID) == false)   // If ND_SCR not null and not already in List
            {
                adjND_GrpIDList.Add(ND_SCR.NDGrpID);                                  // Add to list
            }
        }
        if(adjND_GrpIDList.Count > 0)                                                 // adjND are not all empty
        {
            foreach(int grpID in adjND_GrpIDList)
            {
                if(grpID == -1)                                                       // If grpID is -1, node is empty
                {
                    isPlaceAble = true;
                    Debug.Log("isPlaceAble = " + isPlaceAble);
                    debugScript.LogCurrentMethod();
                    return isPlaceAble;                                               //~ Return true
                }
                if(grpID >= 0)                                                        // if grpID is not default -1 (has been assigned a group)
                {
                    adjGrpList.Add(GetGroup(grpID));
                }
            }
        }

        //? --------------------------------------------------------------------------// Check isPlaceable Status
        if(adjGrpList.Count == 0)                                                     // If no Grps in List (All Nodes are empty)
        {
            isPlaceAble = true;
            Debug.Log("isPlaceAble = " + isPlaceAble);
            debugScript.LogCurrentMethod();
            return isPlaceAble;                                                       //~ Return true
        }

        foreach(Group adjGrp in adjGrpList)
        {
            if(adjGrp.Grp_ShpVal != crntShpVal && adjGrp.GrpLibs == 1)            // If adjGrp is not same ShpVal and liberties = 1 (can be captured)
            {
                isPlaceAble = true;
                Debug.Log("isPlaceAble = " + isPlaceAble);
                debugScript.LogCurrentMethod();
                return isPlaceAble;                                               //~ Return true
            }
            
            if(adjGrp.Grp_ShpVal == crntShpVal && adjGrp.GrpLibs > 1)    // adjGrp is ShpVal and GrpLibs greater than 1
            {
                isPlaceAble = true;
                Debug.Log("isPlaceAble = " + isPlaceAble);
                debugScript.LogCurrentMethod();
                return isPlaceAble;                                               //~ Return true
            }
        }
        
        Debug.Log("isPlaceAble = " + isPlaceAble);
        foreach(Group adjGrp in adjGrpList)
        {
            Debug.Log("Group " +  adjGrp.GrpID +  " libs : " + adjGrp.GrpLibs + " ShpVal: " + adjGrp.Grp_ShpVal);
            Debug.Log("Placed ShpVal: " + crntShpVal);
        }
        
        debugScript.LogCurrentMethod();
        return isPlaceAble;                                                           //~ Return false by Default

    }




    //* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
    public void DeleteZeroLibertyGroup_Methods()   //? Called GameManagerScript         // Sets Nodes in ZeroGrps to Empty, Deletes Group
    {
        List<int> zeroGrpIDList = GetZeroLibGrpIDList();                                // Update All Group Liberties
        List<Group> zeroGrpList = GetZeroLibGrp(zeroGrpIDList);                         // Delete Groups with 0 Liberties
        DeleteZeroLibGrps(zeroGrpList);
    }

    public List<int> GetZeroLibGrpIDList()                                   // STEP 1  // Returns list from AllGrpList with GrpLiberties = 0
    {
        List<int> zero_GrpID_List = new List<int>();

        foreach(Group group in All_Grp_List)
        {
            if(group.GrpLibs == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                if(group.Grp_ShpVal != lastND_ShpVal)
                {
                    zero_GrpID_List.Add(group.GrpID);
                }
            }
        }
        return zero_GrpID_List;
    }

    public List<Group> GetZeroLibGrp(List<int> zero_GrpID_List)              // STEP 2  // Receives the zeroLibertyGrpID list from CalculateGrouLiberties()
    {        
        List<Group> zero_Grp_List = new List<Group>();                                  // Create a new list for sorting
        
        foreach(Group crntGrp in All_Grp_List)                                          // Look through list of All Groups
        {            
            if(zero_GrpID_List.Contains(crntGrp.GrpID))                                 // If the zeroList contains the ID of a Zero'd Node Group
            {        
                zero_Grp_List.Add(crntGrp);                                             // Add it to the zeroGrpList for updating
            }
        }
        return zero_Grp_List;
    }

    public void DeleteZeroLibGrps(List<Group> zero_Grp_List)                 // STEP 3  // Deletes groups in zeroGrpList
    {
        if(zero_Grp_List.Count > 0)
        {
            foreach(Group zeroGrp in zero_Grp_List)                                     // Loop of new list of Zero liberty Groups
            {   
                if(zeroGrp.ND_ID_List.Count == 1)                                       // If there is a Single Node in the groups to Delete, check for Ko
                {
                    perform_Ko_Check = true;
                    Debug.Log("perform_Ko_Check is True");
                }
                else
                {
                    perform_Ko_Check = false;
                }

                if(zeroGrp.Grp_ShpVal != lastND_ShpVal)                                 // ! Checks if zeroGroup is the same sheepVal as current player
                {
                    foreach(int zeroND_ID in zeroGrp.ND_ID_List)                        // Get the script of each Node and Set Node to Empty
                    {
                        GameObject zeroND = GetNodeWithID(All_ND_List, zeroND_ID);
                        NodeScript zeroScript = zeroND.GetComponent<NodeScript>(); 
                        zeroScript.EmptySheepSetter();
                        zeroScript.SetNodeColor_Not_Selected();
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






    public List<int> GetAll_ND_GrpID()
    {
        List<int> ND_GrpID_List = new List<int>();

        foreach(GameObject ND in All_ND_List)
        {
            NodeScript ND_Scrp = ND.GetComponent<NodeScript>();
            if (ND_Scrp != null)
            {
                ND_GrpID_List.Add(ND_Scrp.NDGrpID);
            }
        }

        All_ND_GrpID_List = ND_GrpID_List;

        return ND_GrpID_List;
    }



    public DebugMethods GetDebugMethods()
    {
        GameObject gameManagerObj = GameObject.Find("GameManagerObj");
        debugScript = gameManagerObj.GetComponent<DebugMethods>();

        return debugScript;
    }



    //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
    public void AssignSheepToGroups_REFACTOR(int newND_ID)
    {
        GameObject newND = GetNodeWithID(All_ND_List, newND_ID);
        NodeScript NDScript = newND.GetComponent<NodeScript>();

        // Create new NodeGroup, assign grpID
        if(NDScript.NDGrpID == -1)
        {
            NDScript.NDGrpID = CreateNewGroup(NDScript.NDID);
        }
        int newNDGrpID = NDScript.NDGrpID;
        
        // Adjacent Node Scripts
        List<NodeScript> adjNDScriptList = NDScript.adjNDScrList;

        foreach(NodeScript adjNDScript in adjNDScriptList)
        {
            if(adjNDScript != null && adjNDScript.shpVal == NDScript.shpVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
            {
                int adjNDGrpID = adjNDScript.NDGrpID;

                if(adjNDGrpID != newNDGrpID && adjNDGrpID != -1){
                    JoinGroups(newNDGrpID, adjNDGrpID);
                }

            }
            Debug.Log("adjNode " + adjNDScript.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScript.NDGrpID);
        }

        NDScript.lastPlaced = true;
        lastND_ID = NDScript.NDID;
    }


    // public void AssignSheepToGroups_REFACTOR(GameObject node)
    // {
    //     NodeScript newScript = node.GetComponent<NodeScript>();                                  // Get NodeScript from newNode
    //     newScript.NDGrpID = CreateNewGroup(node);                                                // Create new NodeGroup, assign NDGrpID

    //     List<NodeScript> adjScriptList = newScript.adjNDScriptList;                              // Get List of AdjacentScripts

    //     foreach(NodeScript adjScript in adjScriptList)                                           // Loop over all AdjScripts
    //     {        
    //         if(adjScript != null && adjScript.sheepVal == newScript.sheepVal)                    // If script is not null, and matches sheepVal (Same color)
    //         {        
    //             int adjGrpID = adjScript.NDGrpID;                                                // Save GrpID reference to Delete at end of Method

    //             if(adjScript.NDGrpID != newScript.NDGrpID)                                       // If they are not in the same group
    //             {
    //                 List<GameObject> newGrp = JoinGroups(newScript.NDGrpID, adjScript.NDGrpID);  // Join (Add) all nodes in adjGroup to newGrp

    //                 foreach(GameObject groupNode in newGrp)
    //                 {
    //                     groupNode.GetComponent<NodeScript>().NDGrpID = newScript.NDGrpID;        // Set all newGrp nodes to new ID
    //                 }
    //             }
    //             DeleteGroup(adjGrpID);                                                 // Delete adjGroup by GrpID
    //         }
    //     }

    // }


}