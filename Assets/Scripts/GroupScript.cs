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
		public int grpID;
		public int GrpLibs;
		public int Grp_ShpVal;
		public List<int> NDID_List;

		public static int groupCounter = -1;

		// Constructor
		public Group()
		{
			groupCounter++;                                                         // Increment the counter and assign it to grpID
			grpID = groupCounter;
			NDID_List = new List<int>();
		}
	}

	[SerializeField] public List<Group> All_Grp_List;
	public int lastNDID;
	public int lastND_ShpVal;
	public int lastGrpID;

	public int testVariable;


	public BoardScript BrdScr;
	public NodeScript lastND_Script;

	public TargetNode targetNDScr;

	public GameObject debugNodeCheck;
	public List<NodeScript> NDScrList;
	public List<int> All_ND_grpID_List;

	public DebugMethods debugScript;


	public bool perform_Ko_Check;


	public void Start()
	{
		BrdScr = GetComponent<BoardScript>();
		All_Grp_List = new List<Group>();

		//debugScript = GetDebugMethods();
	}

	private void Update()
	{
		if (NDScrList.Count < 1 && BrdScr.NDList != null){
			NDScrList = BrdScr.NDScrList;
		}
	}


	public void CreateGroup_Method(int crntNDID)          //? Called in GameManager //
	{
		CreateNewGroup(crntNDID);                                                 // Creates newGrp, Associate Properties
		AssignSheepToGroups(crntNDID);                                            // Assign Adj Grps to newgrpID
	}

	public void UpdateGroups_Method()                      //? Called in GameManager //  
	{
		CalculateGrpLiberties();                                                   // Update All Group Liberties
		DeleteZeroLibertyGroup_Methods();                                          // MethodGroup to Delete Grps with 0 Liberties, Set Empty ND
		CalculateGrpLiberties();                                                   // Update All Group Liberties (Post ZeroLibGrp Deletion)
	}



	//* ---------------------------------------- CreateNewGroup  ----------------------------------------
	public int CreateNewGroup(int crntNDID)   //? Called GameManager    // Creates New Group - Called in TargetNode - Returns grpID
	{
		NodeScript NDScr = GetNodeScriptByID(NDScrList, crntNDID);

		// Group Attributes
		Group newGrp = new Group();
		int newGrpID = newGrp.grpID;
		newGrp.Grp_ShpVal = NDScr.shpVal;

		// Associate Group, Node, List
		NDScr.grpID = newGrpID;
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
			if (NDScr.grpID == -1) {
				NDScr.grpID = CreateNewGroup(NDScr.NDID); 			// Create new NodeGroup, assign grpID
			}
			
			int targetNDGID = NDScr.grpID;

			// Adjacent Node Scripts
			NodeScript LNDScr = NDScr.LNDScr;
			NodeScript RNDScr = NDScr.RNDScr;
			NodeScript BNDScr = NDScr.BNDScr;
			NodeScript TNDScr = NDScr.TNDScr;

			//* FIRST CHECK OCCURS ON LEFT NODE
			// Left Node Found
			if (LNDScr != null && LNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
				int LNDGID = LNDScr.grpID;
				if (LNDGID != targetNDGID && LNDGID != -1) {
					JoinGroups(targetNDGID, LNDGID);
				}
			}
			// Right Node Found
			if (RNDScr != null && RNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
				int RNDGID = RNDScr.grpID;
				if (RNDGID != targetNDGID && RNDGID != -1) {
					JoinGroups(targetNDGID, RNDGID);
				}
			}
			// Bottom Node Found
			if (BNDScr != null && BNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
				int BNDGID = BNDScr.grpID;
				if (BNDGID != targetNDGID && BNDGID != -1) {
					JoinGroups(targetNDGID, BNDGID);
				}
			}
			// Top Node Found
			if (TNDScr != null && TNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
				int TNDGID = TNDScr.grpID;

				if (TNDGID != targetNDGID && TNDGID != -1) {
					JoinGroups(targetNDGID, TNDGID);
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

		// Debug.Log("GetNodeScriptByID targetID = " + targetID);
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
		Group group = All_Grp_List.FirstOrDefault(g => g.grpID == grpID);

		return group;
	}
	
	//* --------------------------
	// -------------- JoinGroups ----------------------------------------
	public void JoinGroups(int newGID, int prevGID)                             // Adds prevGIDs to newGrp.NDID_List - return newGrp.NDID_List 
	{
		if (prevGID != -1 && prevGID != newGID){                                // if prevGID is not null and doesn't match the newGrp 
			Group newGrp  = GetGroup(newGID);                                      // Get NewGroup 
			Group prevGrp = GetGroup(prevGID);                                    // Get PrevGroup

			foreach (int NDID in prevGrp.NDID_List){                                   // For each ID in prevGrp 
				newGrp.NDID_List.Add(NDID);                                          // Add ID to newGrp 
				NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                   // Get Node with ID 
				NDScr.GetComponent<NodeScript>().grpID = newGrp.grpID;             // Update grpID on NodeScript 
			}

			DeleteGroup(prevGID);                                                 // Delete prevGrp from All_Grp_List 
		}

	}


	//* ---------------------------------------- DeleteGroup -----------------------------------------
	public void DeleteGroup(int delGID)                                           // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
	{
		if (delGID != -1 && delGID != lastGrpID) {

			Group grpToDelete = All_Grp_List.FirstOrDefault(g => g.grpID == delGID);
			All_Grp_List.Remove(grpToDelete);

			Debug.Log("Group #" + grpToDelete.grpID + " cleared and deleted.");
			// debugScript.LogCallerMethod();
		}
		else
		{
			Debug.Log("grpID was -1");
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


	//* ---------------------------------------- CheckPlaceable ----------------------------------------

	public bool CheckPlaceble(int NDID, int crntShpVal)                              // Calculate Group Capture
	{
		bool canPlace = false;                                                     // Set canPlace bool default at false

		NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                        // Get TargetNode
		List<NodeScript> adjNDScrList = NDScr.adjNDScrList;              // Get adjNDScrList to iterate over Group Capture

		List<int> adjND_GIDList = new List<int>();
		List<Group> adjGrpList = new List<Group>();                                   // Get ajd Group and add to list

		//? --------------------------------------------------------------------------// Get AdjGID and AdjGrp Lists
		foreach (NodeScript adjNDScr in adjNDScrList)                                 // Get ajdND Group ID and add to list
		{
			if (adjNDScr != null && adjND_GIDList.Contains(adjNDScr.grpID) == false)   // If adjNDScr not null and not already in List
			{
				adjND_GIDList.Add(adjNDScr.grpID);                                  // Add to list
			}
		}
		if (adjND_GIDList.Count > 0)                                                 // adjND are not all empty
		{
			foreach (int grpID in adjND_GIDList)
			{
				if (grpID == -1)                                                       // If grpID is -1, node is empty
				{
					canPlace = true;
					Debug.Log("canPlace = " + canPlace);
					// debugScript.LogCurrentMethod();
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
			// debugScript.LogCurrentMethod();
			return canPlace;                                                       // Return true
		}

		foreach (Group adjGrp in adjGrpList)
		{
			if (adjGrp.Grp_ShpVal != crntShpVal && adjGrp.GrpLibs == 1)            // If adjGrp is not same ShpVal and liberties = 1 (can be captured)
			{
				canPlace = true;
				Debug.Log("canPlace = " + canPlace);
				// debugScript.LogCurrentMethod();
				return canPlace;                                               // Return true
			}

			if (adjGrp.Grp_ShpVal == crntShpVal && adjGrp.GrpLibs > 1)    // adjGrp is ShpVal and GrpLibs greater than 1
			{
				canPlace = true;
				Debug.Log("canPlace = " + canPlace);
				// debugScript.LogCurrentMethod();
				return canPlace;                                               // Return true
			}
		}

		Debug.Log("canPlace = " + canPlace);
		foreach (Group adjGrp in adjGrpList)
		{
			Debug.Log("Group " + adjGrp.grpID + " libs : " + adjGrp.GrpLibs + " ShpVal: " + adjGrp.Grp_ShpVal);
			Debug.Log("Placed ShpVal: " + crntShpVal);
		}

		return canPlace;                                                           // Return false by Default

	}




	//* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
	public void DeleteZeroLibertyGroup_Methods()   // Called GameManager         // Sets Nodes in ZeroGrps to Empty, Deletes Group
	{
		List<int> zeroGIDList = GetZeroLibGIDList();                                // Update All Group Liberties
		List<Group> zeroGrpList = GetZeroLibGrp(zeroGIDList);                         // Delete Groups with 0 Liberties
		DeleteZeroLibGrps(zeroGrpList);
	}

	public List<int> GetZeroLibGIDList()                                   // STEP 1  // Returns list from AllGrpList with GrpLiberties = 0
	{
		List<int> zero_GID_List = new List<int>();

		foreach (Group group in All_Grp_List) {
			if (group.GrpLibs == 0) {           // Returns a list of Groups with Liberties == 0 for deletion in other method
				if (group.Grp_ShpVal != lastND_ShpVal) {
					zero_GID_List.Add(group.grpID);
				}
			}
		}
		return zero_GID_List;
	}

	public List<Group> GetZeroLibGrp(List<int> zero_GID_List)              // STEP 2  // Receives the zeroLibertyGID list from CalculateGrouLiberties()
	{
		List<Group> zero_Grp_List = new List<Group>();                                  // Create a new list for sorting

		foreach (Group crntGrp in All_Grp_List) {                                          // Look through list of All Groups
			if (zero_GID_List.Contains(crntGrp.grpID)) {                                 // If the zeroList contains the ID of a Zero'd Node Group
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
				DeleteGroup(zeroGrp.grpID);
			}

		}
		else {
			Debug.Log("No zeroLiberty Groups");
		}

	}






	public List<int> GetAll_ND_GID()
	{
		List<int> ND_GrpID_List = new List<int>();

		foreach (NodeScript NDScr in NDScrList) {
			if (NDScr != null) {
				ND_GrpID_List.Add(NDScr.grpID);
			}
		}

		All_ND_grpID_List = ND_GrpID_List;

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
		NodeScript NDScr = newND.GetComponent<NodeScript>();

		// Create new NodeGroup, assign grpID
		if (NDScr.grpID == -1) {
			NDScr.grpID = CreateNewGroup(NDScr.NDID);
		}
		int newNDGID = NDScr.grpID;

		// Adjacent Node Scripts
		List<NodeScript> adjNDScrList = NDScr.adjNDScrList;

		foreach (NodeScript adjNDScr in adjNDScrList)
		{
			if (adjNDScr != null && adjNDScr.shpVal == NDScr.shpVal)                   // ! Checks if zeroGroup is the same sheepVal as current player                  
			{
				int adjNDGID = adjNDScr.grpID;

				if (adjNDGID != newNDGID && adjNDGID != -1)
				{
					JoinGroups(newNDGID, adjNDGID);
				}

			}
			Debug.Log("adjNode " + adjNDScr.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScr.grpID);
		}

		NDScr.lastPlaced = true;
		lastNDID = NDScr.NDID;
	}


	// public void AssignSheepToGroups_REFACTOR(GameObject node)
	// {
	//     NodeScript newScript = node.GetComponent<NodeScript>();                                  // Get NodeScript from newNode
	//     newScript.grpID = CreateNewGroup(node);                                                // Create new NodeGroup, assign grpID

	//     List<NodeScript> adjScriptList = newScript.adjNDScrList;                              // Get List of AdjacentScripts

	//     foreach(NodeScript adjScript in adjScriptList)                                           // Loop over all AdjScripts
	//     {        
	//         if(adjScript != null && adjScript.sheepVal == newScript.sheepVal)                    // If script is not null, and matches sheepVal (Same color)
	//         {        
	//             int adjGID = adjScript.grpID;                                                // Save grpID reference to Delete at end of Method

	//             if(adjScript.grpID != newScript.grpID)                                       // If they are not in the same group
	//             {
	//                 List<GameObject> newGrp = JoinGroups(newScript.grpID, adjScript.grpID);  // Join (Add) all nodes in adjGroup to newGrp

	//                 foreach(GameObject groupNode in newGrp)
	//                 {
	//                     groupNode.GetComponent<NodeScript>().grpID = newScript.grpID;        // Set all newGrp nodes to new ID
	//                 }
	//             }
	//             DeleteGroup(adjGID);                                                 // Delete adjGroup by GID
	//         }
	//     }

	// }


}