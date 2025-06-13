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

public class GroupScript : MonoBehaviour
{
	[System.Serializable]
	public class Group
	{
		public int GrpID;
		public int GrpLibs;
		public int Grp_ShpVal;
		public List<int> NDID_List;

		public static int groupCounter = -1;

		// Constructor
		public Group()
		{
			groupCounter++;                                                         // Increment the counter and assign it to GrpID
			GrpID = groupCounter;
			NDID_List = new List<int>();
		}
	}

	[SerializeField] public List<Group> All_Grp_List;
	public int lastNDID;
	public int lastND_ShpVal;
	public int lastGrpID;

	public int testVariable;


	public BoardGenerator BrdGntScr;
	public NodeScript lastND_Script;

	public TargetNode targetNDScript;

	public GameObject debugNodeCheck;
	public List<NodeScript> NDScrList;
	public List<int> All_ND_GrpID_List;

	public DebugMethods debugScript;


	public bool perform_Ko_Check;


	public void Start()
	{
		BrdGntScr = GetComponent<BoardGenerator>();
		All_Grp_List = new List<Group>();

		testVariable = 123;

		//debugScript = GetDebugMethods();
	}

	private void Update()
	{
		if (NDScrList.Count < 1 && BrdGntScr.NDList != null){
			NDScrList = BrdGntScr.NDScrList;
		}
	}


	public void CreateGroup_Method(int crntNDID)          //? Called in GameManagerScript //
	{
		CreateNewGroup(crntNDID);                                                 // Creates newGrp, Associate Properties
		AssignSheepToGroups(crntNDID);                                            // Assign Adj Grps to newGrpID
	}

	public void UpdateGroups_Method()                      //? Called in GameManagerScript //  
	{
		CalculateGrpLiberties();                                                   // Update All Group Liberties
		DeleteZeroLibertyGroup_Methods();                                          // MethodGroup to Delete Grps with 0 Liberties, Set Empty ND
		CalculateGrpLiberties();                                                   // Update All Group Liberties (Post ZeroLibGrp Deletion)
	}



	//* ---------------------------------------- CreateNewGroup  ----------------------------------------
	public int CreateNewGroup(int crntNDID)   //? Called GameManagerScript    // Creates New Group - Called in TargetNode - Returns GrpID
	{
		NodeScript NDScr = GetNodeScriptByID(NDScrList, crntNDID);

		// Group Attributes
		Group newGrp = new Group();
		int newGrpID = newGrp.GrpID;
		newGrp.Grp_ShpVal = NDScr.shpVal;

		// Associate Group, Node, List
		NDScr.GrpID = newGrpID;
		newGrp.NDID_List.Add(NDScr.NDID);

		// Update Global Properties
		All_Grp_List.Add(newGrp);
		lastNDID = crntNDID;                                              // ! Sets last placed NDID for Script Reference
		lastND_ShpVal = NDScr.shpVal;

		lastGrpID = newGrpID;           // TODO Check on change 

		// debugScript.LogCurrentLine();

		return newGrpID;
	}
	//* ---------------------------------------- AssignSheepToGroups ----------------------------------------
	public void AssignSheepToGroups(int targetNDID)
	{
		NodeScript NDScr = GetNodeScriptByID(NDScrList, targetNDID);

		if (NDScr.shpVal != NDScr.shpValList[0]) {
			if (NDScr.GrpID == -1) {
				NDScr.GrpID = CreateNewGroup(NDScr.NDID); 			// Create new NodeGroup, assign grpID
			}
			
			int targetNDGrpID = NDScr.GrpID;

			// Adjacent Node Scripts
			NodeScript LNDScr = NDScr.LNDScr;
			NodeScript RNDScr = NDScr.RNDScr;
			NodeScript BNDScr = NDScr.BNDScr;
			NodeScript TNDScr = NDScr.TNDScr;

			//* FIRST CHECK OCCURS ON LEFT NODE
			// Left Node Found
			if (LNDScr != null && LNDScr.shpVal == NDScr.shpVal) {                   // ! Checks if adjNode = sheepVal as current player                  
				int LNDGrpID = LNDScr.GrpID;
				if (LNDGrpID != targetNDGrpID && LNDGrpID != -1) {
					JoinGroups(targetNDGrpID, LNDGrpID);
				}
			}
			// Right Node Found
			if (RNDScr != null && RNDScr.shpVal == NDScr.shpVal) {                   // ! Checks if adjNode = sheepVal as current player                  
				int RNDGrpID = RNDScr.GrpID;
				if (RNDGrpID != targetNDGrpID && RNDGrpID != -1) {
					JoinGroups(targetNDGrpID, RNDGrpID);
				}
			}
			// Bottom Node Found
			if (BNDScr != null && BNDScr.shpVal == NDScr.shpVal) {                   // ! Checks if adjNode = sheepVal as current player                  
				int BNDGrpID = BNDScr.GrpID;
				if (BNDGrpID != targetNDGrpID && BNDGrpID != -1) {
					JoinGroups(targetNDGrpID, BNDGrpID);
				}
			}
			// Top Node Found
			if (TNDScr != null && TNDScr.shpVal == NDScr.shpVal) {                   // ! Checks if adjNode = sheepVal as current player                  
				int TNDGrpID = TNDScr.GrpID;

				if (TNDGrpID != targetNDGrpID && TNDGrpID != -1) {
					JoinGroups(targetNDGrpID, TNDGrpID);
				}
			}

			NDScr.lastPlaced = true;
			lastNDID = NDScr.NDID;
		}
	}


	//? ---------------------------------------- STATIC METHODS  ----------------------------------------
	//* ---------------------------------------- GetNodeScriptByID ----------------------------------------
	// ------ Get NODE from nodeID in gNodeArray -------
	public NodeScript GetNodeScriptByID(List<NodeScript> NDScrList, int targetID) {
		foreach (NodeScript NDScr in NDScrList) {
			if (NDScr.NDID == targetID) {
				return NDScr; 																														// Found NodeScript with the targetID
			}
		}
		return null; 																																	// Return null NodeScript not found
	}
	//* ---------------------------------------- GetGroup ----------------------------------------
	public Group GetGroup(int grpID)                                                // Returns Group from AllGrpList by grpID
	{
		Group group = All_Grp_List.FirstOrDefault(g => g.GrpID == grpID);

		return group;
	}
	//* ---------------------------------------- JoinGroups ----------------------------------------
	public void JoinGroups(int newGrpID, int prevGrpID)                             // Adds prevGrpIDs to newGrp.NDID_List - return newGrp.NDID_List 
	{
		if (prevGrpID != -1 && prevGrpID != newGrpID)                                // if prevGrpID is not null and doesn't match the newGrp 
		{
			Group newGrp = GetGroup(newGrpID);                                      // Get NewGroup 
			Group prevGrp = GetGroup(prevGrpID);                                    // Get PrevGroup

			foreach (int ID in prevGrp.NDID_List)                                   // For each ID in prevGrp 
			{
				newGrp.NDID_List.Add(ID);                                          // Add ID to newGrp 
				NodeScript NDScr = GetNodeScriptByID(NDScrList, ID);                   // Get Node with ID 
				NDScr.GetComponent<NodeScript>().GrpID = newGrp.GrpID;             // Update GrpID on NodeScript 
			}

			DeleteGroup(prevGrpID);                                                 // Delete prevGrp from All_Grp_List 
		}

		// DeleteGroup(prevGrpID);
		// Debug.Log("GroupManager: JoinGroups [ " + prevGrpID + " Nodes added to newGrpID " + newGrpID + " ]");

	}
	//* ---------------------------------------- DeleteGroup -----------------------------------------
	public void DeleteGroup(int delGrpID)                                           // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
	{
		if (delGrpID != -1 && delGrpID != lastGrpID)
		{
			Group grpToDelete = All_Grp_List.FirstOrDefault(g => g.GrpID == delGrpID);

			All_Grp_List.Remove(grpToDelete);

			Debug.Log("Group #" + grpToDelete.GrpID + " cleared and deleted.");
			// debugScript.LogCallerMethod();
		}
		else
		{
			Debug.Log("GrpID was -1");
		}
	}
	//* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
	public void CalculateGrpLiberties()                                               // Updates Liberties of all Groups in AllGrpList
	{
		int totalGrpLibs = 0;

		foreach (Group Grp in All_Grp_List){    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
			List<NodeScript> NDScrList = new List<NodeScript>();

			foreach (int NDID in Grp.NDID_List){
				NodeScript crntNDScr = GetNodeScriptByID(NDScrList, NDID);
				NDScrList.Add(crntNDScr);
			}

			List<NodeScript> countedNDs = new List<NodeScript>();                     //! holds adjNDScr to prevent double references for libVals 

			foreach (NodeScript NDScr in NDScrList){                                   // Adds value of node to total liberties
				
				if (NDScr.LNDScr != null){																				// If the adj node is not empty 
					if (countedNDs.Contains(NDScr.LNDScr) == false) {                 // If the node is not already in the script (has been counted)
						countedNDs.Add(NDScr.LNDScr);                              // Add it to the liberty node list
						totalGrpLibs += NDScr.LNDScr.libVal;             // Add it's liberty value to the group ( 1 or 0 )
					}
				}
				if (NDScr.RNDScr != null) {
					if (countedNDs.Contains(NDScr.RNDScr) == false) {
						countedNDs.Add(NDScr.RNDScr);
						totalGrpLibs += NDScr.RNDScr.libVal;
					}
				}
				if (NDScr.BNDScr != null) {
					if (countedNDs.Contains(NDScr.BNDScr) == false) {
						countedNDs.Add(NDScr.BNDScr);
						totalGrpLibs += NDScr.BNDScr.libVal;
					}
				}
				if (NDScr.TNDScr != null) {
					if (countedNDs.Contains(NDScr.TNDScr) == false)
					{
						countedNDs.Add(NDScr.TNDScr);
						totalGrpLibs += NDScr.TNDScr.libVal;
					}
				}
			}

			Grp.GrpLibs = totalGrpLibs;

			totalGrpLibs = 0;
			countedNDs.Clear();
		}

	}



	public bool CheckPlaceble(int NDID, int crntShpVal)                              // Calculate Group Capture
	{
		bool canPlace = false;                                                     // Set canPlace bool default at false

		NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                        // Get TargetNode
		List<NodeScript> adjNDScrList = NDScr.adjNDScrList;              // Get adjNDScrList to iterate over Group Capture

		List<int> adjND_GrpIDList = new List<int>();
		List<Group> adjGrpList = new List<Group>();                                   // Get ajd Group and add to list

		//? --------------------------------------------------------------------------// Get AdjGrpID and AdjGrp Lists
		foreach (NodeScript AdjNDScr in adjNDScrList)                                 // Get ajdND Group ID and add to list
		{
			if (AdjNDScr != null && adjND_GrpIDList.Contains(AdjNDScr.GrpID) == false)   // If AdjNDScr not null and not already in List
			{
				adjND_GrpIDList.Add(AdjNDScr.GrpID);                                  // Add to list
			}
		}
		if (adjND_GrpIDList.Count > 0)                                                 // adjND are not all empty
		{
			foreach (int grpID in adjND_GrpIDList)
			{
				if (grpID == -1)                                                       // If grpID is -1, node is empty
				{
					canPlace = true;
					Debug.Log("canPlace = " + canPlace);
					debugScript.LogCurrentMethod();
					return canPlace;                                               // Return true
				}
				if (grpID >= 0)                                                        // if grpID is not default -1 (has been assigned a group)
				{
					adjGrpList.Add(GetGroup(grpID));
				}
			}
		}

		// --------------------------------------------------------------------------// Check isPlaceable Status
		if (adjGrpList.Count == 0)                                                     // If no Grps in List (All Nodes are empty)
		{
			canPlace = true;
			Debug.Log("canPlace = " + canPlace);
			debugScript.LogCurrentMethod();
			return canPlace;                                                       // Return true
		}

		foreach (Group adjGrp in adjGrpList)
		{
			if (adjGrp.Grp_ShpVal != crntShpVal && adjGrp.GrpLibs == 1)            // If adjGrp is not same ShpVal and liberties = 1 (can be captured)
			{
				canPlace = true;
				Debug.Log("canPlace = " + canPlace);
				debugScript.LogCurrentMethod();
				return canPlace;                                               // Return true
			}

			if (adjGrp.Grp_ShpVal == crntShpVal && adjGrp.GrpLibs > 1)    // adjGrp is ShpVal and GrpLibs greater than 1
			{
				canPlace = true;
				Debug.Log("canPlace = " + canPlace);
				debugScript.LogCurrentMethod();
				return canPlace;                                               // Return true
			}
		}

		Debug.Log("canPlace = " + canPlace);
		foreach (Group adjGrp in adjGrpList)
		{
			Debug.Log("Group " + adjGrp.GrpID + " libs : " + adjGrp.GrpLibs + " ShpVal: " + adjGrp.Grp_ShpVal);
			Debug.Log("Placed ShpVal: " + crntShpVal);
		}

		debugScript.LogCurrentMethod();
		return canPlace;                                                           // Return false by Default

	}




	//* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
	public void DeleteZeroLibertyGroup_Methods()   // Called GameManagerScript         // Sets Nodes in ZeroGrps to Empty, Deletes Group
	{
		List<int> zeroGrpIDList = GetZeroLibGrpIDList();                                // Update All Group Liberties
		List<Group> zeroGrpList = GetZeroLibGrp(zeroGrpIDList);                         // Delete Groups with 0 Liberties
		DeleteZeroLibGrps(zeroGrpList);
	}

	public List<int> GetZeroLibGrpIDList()                                   // STEP 1  // Returns list from AllGrpList with GrpLiberties = 0
	{
		List<int> zero_GrpID_List = new List<int>();

		foreach (Group group in All_Grp_List)
		{
			if (group.GrpLibs == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
			{
				if (group.Grp_ShpVal != lastND_ShpVal)
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

		foreach (Group crntGrp in All_Grp_List)                                          // Look through list of All Groups
		{
			if (zero_GrpID_List.Contains(crntGrp.GrpID))                                 // If the zeroList contains the ID of a Zero'd Node Group
			{
				zero_Grp_List.Add(crntGrp);                                             // Add it to the zeroGrpList for updating
			}
		}
		return zero_Grp_List;
	}

	public void DeleteZeroLibGrps(List<Group> zero_Grp_List)                 // STEP 3  // Deletes groups in zeroGrpList
	{
		if (zero_Grp_List.Count > 0) {
			foreach (Group zeroGrp in zero_Grp_List) {                                    // Loop of new list of Zero liberty Groups
				if (zeroGrp.NDID_List.Count == 1) {                                       // If there is a Single Node in the groups to Delete, check for Ko
					perform_Ko_Check = true;
					Debug.Log("perform_Ko_Check is True");
				}
				else {
					perform_Ko_Check = false;
				}

				if (zeroGrp.Grp_ShpVal != lastND_ShpVal) {                                // ! Checks if zeroGroup is the same sheepVal as current player
					foreach (int zeroNDID in zeroGrp.NDID_List) {                        // Get the script of each Node and Set Node to Empty
						NodeScript zeroND = GetNodeScriptByID(NDScrList, zeroNDID);
						NodeScript zeroScript = zeroND.GetComponent<NodeScript>();
						zeroScript.EmptySheepSetter();
						zeroScript.SetNodeColor_Not_Selected();
					}
				}
				DeleteGroup(zeroGrp.GrpID);
			}

		}
		else {
			Debug.Log("No zeroLiberty Groups");
		}

	}






	public List<int> GetAll_ND_GrpID()
	{
		List<int> ND_GrpID_List = new List<int>();

		foreach (NodeScript NDScr in NDScrList) {
			if (NDScr != null) {
				ND_GrpID_List.Add(NDScr.GrpID);
			}
		}

		All_ND_GrpID_List = ND_GrpID_List;

		return ND_GrpID_List;
	}



	// public DebugMethods GetDebugMethods()
	// {
	// 	GameObject gameManagerObj = GameObject.Find("GameManagerObj");
	// 	debugScript = gameManagerObj.GetComponent<DebugMethods>();

	// 	return debugScript;
	// }



	//* ---------------------------------------- AssignSheepToGroups ----------------------------------------
	public void AssignSheepToGroups_REFACTOR(int newNDID)
	{
		NodeScript newND = GetNodeScriptByID(NDScrList, newNDID);
		NodeScript NDScript = newND.GetComponent<NodeScript>();

		// Create new NodeGroup, assign grpID
		if (NDScript.GrpID == -1) {
			NDScript.GrpID = CreateNewGroup(NDScript.NDID);
		}
		int newNDGrpID = NDScript.GrpID;

		// Adjacent Node Scripts
		List<NodeScript> adjNDScrList = NDScript.adjNDScrList;

		foreach (NodeScript adjNDScript in adjNDScrList)
		{
			if (adjNDScript != null && adjNDScript.shpVal == NDScript.shpVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
			{
				int adjNDGrpID = adjNDScript.GrpID;

				if (adjNDGrpID != newNDGrpID && adjNDGrpID != -1)
				{
					JoinGroups(newNDGrpID, adjNDGrpID);
				}

			}
			Debug.Log("adjNode " + adjNDScript.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScript.GrpID);
		}

		NDScript.lastPlaced = true;
		lastNDID = NDScript.NDID;
	}


	// public void AssignSheepToGroups_REFACTOR(GameObject node)
	// {
	//     NodeScript newScript = node.GetComponent<NodeScript>();                                  // Get NodeScript from newNode
	//     newScript.GrpID = CreateNewGroup(node);                                                // Create new NodeGroup, assign GrpID

	//     List<NodeScript> adjScriptList = newScript.adjNDScrList;                              // Get List of AdjacentScripts

	//     foreach(NodeScript adjScript in adjScriptList)                                           // Loop over all AdjScripts
	//     {        
	//         if(adjScript != null && adjScript.sheepVal == newScript.sheepVal)                    // If script is not null, and matches sheepVal (Same color)
	//         {        
	//             int adjGrpID = adjScript.GrpID;                                                // Save GrpID reference to Delete at end of Method

	//             if(adjScript.GrpID != newScript.GrpID)                                       // If they are not in the same group
	//             {
	//                 List<GameObject> newGrp = JoinGroups(newScript.GrpID, adjScript.GrpID);  // Join (Add) all nodes in adjGroup to newGrp

	//                 foreach(GameObject groupNode in newGrp)
	//                 {
	//                     groupNode.GetComponent<NodeScript>().GrpID = newScript.GrpID;        // Set all newGrp nodes to new ID
	//                 }
	//             }
	//             DeleteGroup(adjGrpID);                                                 // Delete adjGroup by GrpID
	//         }
	//     }

	// }


}