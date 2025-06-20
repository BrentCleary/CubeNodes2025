using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{

	//* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
	[Header("Object References")]
	public GameObject NDArr;
	public GameObject pauseMenu;

	[Header("Current Board State")]
	public List<int> nowBrdState;
	public int nowShpVal;
	public bool isKo = false;

	[Header("Last Board State")]
	public List<int> prvBrdState;
	public int prvShpVal;

	[Header("Game Recorder")]
	public List<List<int>> Brd_State_Record;


  //* ---------------------------------------- RAYCAST PROPS ----------------------------------------

	[Header("RayCast Props")]
	public Camera mainCamera;
	public LayerMask nodeLayerMask;
	public bool rayCastMouseSelect;

	private GameObject _hitObject ;
	public  GameObject  hitObject	{
		get { return _hitObject; }
		set	{	if (_hitObject != value) { 
							_hitObject  = value;
	}}}
	private NodeScript _hitScr;
	public  NodeScript  hitScr	{
		get { return _hitScr; }
		set	{ if(_hitScr != value) { 
						_hitScr 	= value; 
						OnValueChanged(); 
	}}}

	//* ---------------------------------------- BOARD PROPS ----------------------------------------
  [Header("Board Array Settings")] 																					
	[SerializeField] public List<GameObject> NDList;
	[SerializeField] public List<NodeScript> NDScrList;
	[SerializeField] public GameObject[,] ND_Arr;
  public Transform ND_ArrTransform;

  public int arrColLen = 3;                                              // ! Array Column Size
  public int arrRowLen = 3;                                              // ! Array Row Size
  public int arrSize => arrColLen * arrRowLen;                           // arrColLen * arrRowLen
  public float ND_Spacing = 2f;                                          // Space Between Nodes

  public GameObject ND_Prefab;
  public List<int> startNDValMap;
  public List<int> crntNDValMap;


  //* ---------------------------------------- GROUP MANAGER PROPS ----------------------------------------
	[System.Serializable]
	public class Group {
		public int grpID;
		public int GrpLibs;
		public int Grp_ShpVal;
		public List<int> NDID_List;
		public static int groupCount = -1;	

		// Constructor
		public Group() { 
			groupCount++;                                                         // Increment the counter and assign it to grpID
			grpID = groupCount;
			NDID_List = new List<int>();
		}
	}
	[Header("Group Properties")]
	public int  lastNDID;
	public int  lastShpVal;
	public int  lastGrpID;
	public bool checkForKo;
	public List<int> All_ND_grpID_List;
	public List<Group> allGrpList;




  //* ----------------------------------------  MANAGER METHODS ----------------------------------------

	void Awake() {
		NDArr 		 = GameObject.Find("gNodeArray");
		pauseMenu  = GameObject.Find("PauseMenu");
		mainCamera = Camera.main;
	}

	void Start() {
		
		// BOARD 
		CreateBoard();
		UpdateBoardNodeValues();
		UpdateBoardDisplay();

		nowBrdState = Create_ShpValMap();
		prvBrdState = nowBrdState.ToList();
		allGrpList = new List<Group>();

	}

	void Update() {

		DrawRay();
		PlaceSheep_OnClick();

	}


  //* ----------------------------------------  PLACEMENT METHODS ----------------------------------------

	public void PlaceSheepMethod(int ND_ID, int shpVal) {
		NodeScript crntNDScr = GetNodeScriptByID(NDScrList, ND_ID);                                                            // Set blackSheepVal

		Debug.Log("shpVal = " + shpVal);

		// bool isPlaceAble = CheckPlaceble(ND_ID, shpVal);
		// Update_Ko_Status(ND_ID, shpVal);

		// if (isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
		if(1 == 1)
		{
			if (shpVal == 1) { crntNDScr.EmptySheepSetter(); }
			if (shpVal == 1) { crntNDScr.BlackSheepSetter(); }
		  if (shpVal == 2) { crntNDScr.WhiteSheepSetter(); }                                                               // Set Node to BlacksheepVal
		

			UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes
			
			// CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
			// UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

			// UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
			// UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

			UpdateBoardDisplay();                                                             // Update Board Display

			prvBrdState = nowBrdState.ToList();
			LogListValues<int>(prvBrdState, "prvBrdState GM");

			nowBrdState = Create_ShpValMap();
			LogListValues<int>(nowBrdState, "nowBrdState GM");

			prvShpVal = prvBrdState[ND_ID];
			nowShpVal = nowBrdState[ND_ID];

		}
	}


	public void Update_Ko_Status(int ND_ID, int shpVal)
	{
		if (checkForKo == true) {
			isKo = Check_Map_For_Ko(prvBrdState, ND_ID, shpVal);
		}
		else {
			isKo = false;
		}
	}




	//* ---------------------------------------- RAYCAST METHODS ----------------------------------------
	#region RAYCAST METHODS

	public GameObject GetRaycastHitObject() 
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		nodeLayerMask = LayerMask.GetMask("Node");

		if (Physics.Raycast(ray, out hit, 1000, nodeLayerMask))
		{
			Debug.Log("Raycast hit: " + hit.transform.gameObject.name);
			return hit.transform.gameObject; // Return the GameObject that the ray hits
		}
		else{	return null; }  // Return null if no object is hit

	}

	// DrawRay
	public void DrawRay()	
	{
		if (mainCamera == null)	return;

		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);					// Get a ray from the mouse position
		Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);				// Draw the ray from the camera toward the mouse direction, 1000 units long
	}

	public void OnValueChanged()
	{
    if (_hitObject != null) { Debug.Log("Raycast hit: " + _hitObject.name); }
		else{	Debug.Log("Raycast hit: null"); }
	}


	#endregion


	//* ---------------------------------------- BOARD METHODS ----------------------------------------
	#region BOARD METHODS
		


  public void CreateBoard()  																								// Instantiates Variables, Calls Methods Below
  {
    ND_Arr = new GameObject[arrColLen, arrRowLen];
    ND_ArrTransform = gameObject.transform;
    NDList = new List<GameObject>();
		NDScrList = new List<NodeScript>();

		InstantiateNodes();					// Creates a list of Node GameObjects
    SetNodeTransformPosition(); // Sets Transfrom of All Nodes
    BuildNodeArray();						// Creates Array. Places nodes.
    AdjNodeMapper();						// Associates Nodes to Neighbors
  }
  
	
	// *---------------------------------------- CreateBoard Methods ----------------------------------------
  
	public void InstantiateNodes()                                                  // Instantiates Nodes, Assigns names and values, Adds them to NDList
  {
    for (int i = 0; i < arrSize; i++)
    {
      GameObject node = Instantiate(ND_Prefab, ND_ArrTransform);
      node.name = $"Node ({i})";
      node.GetComponent<NodeScript>().NDID = i;                                   //! Sets NDID in NodeScript
      NDList.Add(node);
			NDScrList.Add(node.GetComponent<NodeScript>());
		}
  }
	
	public void SetNodeTransformPosition()                                          // Set Node transform.position
  {
    int count = 0;                                                              // Increments Node reference in NDList 
    for (int i = 0; i < arrColLen; i++){                                          // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++){
        NDList[count].transform.position = new Vector3(i * ND_Spacing, 0, j * ND_Spacing);
        count++;
      }
    }
  }


  public void BuildNodeArray()                                                    // Generate Array using Length x Row using nodes in NDList
  {
    int count = 0;                                                            // Increments Node reference in NDList 
    for (int i = 0; i < arrColLen; i++){                                          // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++){
        ND_Arr[i, j] = NDList[count];                                         // Set curent NDList object to current array position

        // Maps Board Array position for reference
        NodeScript NDScr = NDScrList[count];
        NDScr.arrPos[0] = i;
        NDScr.arrPos[1] = j;

        // Add Array Position to Node Name
        ND_Arr[i, j].name = $"{ND_Arr[i, j].name} [{i},{j}]";
        
        count++;
      }
    }
  }

  public void AdjNodeMapper()                                           // Loop over NDScrList, assign adjNodes
  {
    foreach (NodeScript NDScr in NDScrList){
      
      int[] arrPos = NDScr.arrPos;

      if (arrPos[0] == 0) { NDScr.LNDScr = null; }                                   // left index out of range 
      else {                                                                      // left index is node
        NDScr.LNDScr = ND_Arr[arrPos[0] - 1, arrPos[1]].GetComponent<NodeScript>();
      }
      
      if (arrPos[0] == arrColLen - 1){ NDScr.RNDScr = null; }                        // right index out of range 
      else {                                                                      // right index is node 
        NDScr.RNDScr = ND_Arr[arrPos[0] + 1, arrPos[1]].GetComponent<NodeScript>();
      }

      if (arrPos[1] == 0) { NDScr.BNDScr = null; }                                   // bottom index out of range
      else {                                                                      // bottom index is  node
        NDScr.BNDScr = ND_Arr[arrPos[0], arrPos[1] - 1].GetComponent<NodeScript>();
      }

      if (arrPos[1] == arrRowLen - 1) { NDScr.TNDScr = null; }                       // top index out of range
      else {                                                                      // top index is node
        NDScr.TNDScr = ND_Arr[arrPos[0], arrPos[1] + 1].GetComponent<NodeScript>();
      }

    }
  }




  public void UpdateBoardNodeValues()  //? Called GameManager               // Calls Functions Step1, Step2, Step3 
  {
    List<int> NDValMap = Create_NDValueMap_Step1();
    List<int> NDValMapUpdate = Set_NDValueMap_Step2(NDValMap);                 // Update NDValMap based on position and board state
    Update_NDValues_Step3(NDValMapUpdate);
  }
  
  
  // *---------------------------------------- Node Value Update Methods ----------------------------------
  public List<int> Create_NDValueMap_Step1() {                                      // Displays Array based on nodeValues
    
    List<int> NDValMap = new List<int>();                                           // List to hold update values for arrayNodes
    
    foreach (NodeScript NDScr in NDScrList) {                                        // Loops over all nodes in NDList    
      if (NDScr.shpVal == NDScr.shpValList[0]) {                                     // No Sheep
        NDValMap.Add(NDScr.NDValList[4]);                                           // Assigns max NodeValue to each position
        NDScr.libVal = NDScr.libValList[1];
      }
      else{                                                                         // Sheep placed
        NDValMap.Add(NDScr.NDValList[0]);                                           // Assigns min NodeValue to each position
        NDScr.libVal = NDScr.libValList[0];
      }
    }

    return NDValMap;
  }


  public List<int> Set_NDValueMap_Step2(List<int> NDValMap) {                    // Maps nodeValues for updating in Step 3

    List<int> newValMap = NDValMap;

    int crntNDVal = 0;

    // Map node values to NDValMap based on current board state
    for (int i = 0; i < arrColLen; i++) {         // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++) {
        
        if (j == 0) {      // Check left index is not out of range 
          newValMap[crntNDVal] -= 1;
        }
        else if (j != 0) {      // Check left position
          if (ND_Arr[i, j - 1].GetComponent<NodeScript>().libVal == 0) {     // Check left crntNDVal is not null
            newValMap[crntNDVal] -= 1;
          }
        }

        if (j == arrRowLen - 1) {      // Check right crntNDVal is not out of range 
          newValMap[crntNDVal] -= 1;
        }
        else if (j != arrRowLen - 1) {       // Check right position
          if (ND_Arr[i, j + 1].GetComponent<NodeScript>().libVal == 0) {     // Check right crntNDVal is not null
            newValMap[crntNDVal] -= 1;
          }
        }

        if (i == 0) {      // Check top crntNDVal is not out of range 
          newValMap[crntNDVal] -= 1;
        }
        else if (i != 0) {      //  Check top position
          if (ND_Arr[i - 1, j].GetComponent<NodeScript>().libVal == 0) {
            newValMap[crntNDVal] -= 1;
          }
        }

        if (i == arrColLen - 1) {      // Check bottom crntNDVal is not out of range 
          newValMap[crntNDVal] -= 1;
        }
        else if (i != arrColLen - 1) {      // Check bottom position 
          if (ND_Arr[i + 1, j].GetComponent<NodeScript>().libVal == 0) {
            newValMap[crntNDVal] -= 1;
          }
        }
        crntNDVal += 1;
      }
    }

    return newValMap;
  }


  public void Update_NDValues_Step3(List<int> NDValMap)                         // Sets Node Display values to Map Values
  {
    int arrayIndex = 0;

    // Map NDValMap values to NDArray
    for (int i = 0; i < arrColLen; i++) {        // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++) {
        GameObject crntND = ND_Arr[i, j];
        NodeScript crntNDScr = crntND.GetComponent<NodeScript>();

        if (NDValMap[arrayIndex] < 0) {
          crntNDScr.NDVal = 0;
        }
        else { crntNDScr.NDVal = NDValMap[arrayIndex]; }

        arrayIndex += 1;
      }
    }
  }


  // *---------------------------------------- Node Display Update Method ---------------------------------
  public void UpdateBoardDisplay()  																					  														// Updated Display of Nodes
  {
    foreach (GameObject crntNode in NDList) {
      NodeScript crntNDScrpt = crntNode.GetComponent<NodeScript>();
      crntNDScrpt.UpdateNodeDisplay();
    }
  }


  // *---------------------------------------- Ko Check Methods ---------------------------------
  public List<int> Create_ShpValMap()  																														// Displays Array based on nodeValues
  {
    List<int> ShpValMap = new List<int>();                                         // List to hold update values for arrayNodes

    foreach (GameObject crntND in NDList) {                                         // Loops over all nodes in NDList    
      NodeScript NDScr = crntND.GetComponent<NodeScript>();                    // Set liberty value based on masterNode
      ShpValMap.Add(NDScr.shpVal);
    }

    return ShpValMap;
  }


  public bool Check_Map_For_Ko(List<int> prevShpValMap, int ND_ID, int ShpVal)        // Map of board state before last move, ND_ID and Value
  {
    bool isKo = false;                                                              // Ko is initially set false

    List<int> newShpValMap = prevShpValMap.ToList();                                // Create a new copy of ShpValMap for updating and comparing
    newShpValMap[ND_ID] = ShpVal;                                                   // Change List val to ShpVal at ND_ID index

    bool sequenceCheck = newShpValMap.SequenceEqual(prevShpValMap);
    Debug.Log("sequenceCheck: " + sequenceCheck);

    FindDifferences(newShpValMap, prevShpValMap);

    if (sequenceCheck) {                                              // Compare to prev ShpValMap for Board state
      isKo = true;                                                                // If they match, the move will violate KO rules
      Debug.Log("** KO is True **");
      LogListValues<int>(prevShpValMap, "prevShpValMap");
      LogListValues<int>(newShpValMap, "newShpValMap");
    }
    else {
      isKo = false;
    }

    return isKo;                                                                    // Return the Ko bool Value
  }

  // DEBUG METHOD FOR KO CHECK METHODS
  void FindDifferences(List<int> newShpValMap, List<int> prevShpValMap) {
    List<int> differences = new List<int>();

    for (int i = 0; i < newShpValMap.Count; i++) {
      if (newShpValMap[i] != prevShpValMap[i]) {
        differences.Add(i); // Record the index where they differ
      }
    }

		if (differences.Count > 0) {
      Debug.Log("Sequences differ at indices: " + string.Join(", ", differences));
    }
    else {
      Debug.Log("Sequences are identical.");
    }

  }


	#endregion


	//* ---------------------------------------- GROUP METHODS  ----------------------------------------
	#region GROUP METHODS


	// public void CreateGroup_Method(int crntNDID)          //? Called in GameManager //
	// {
	// 	CreateNewGroup(crntNDID);                                                 // Creates newGrp, Associate Properties
	// 	AssignSheepToGroups(crntNDID);                                            // Assign Adj Grps to newgrpID
	// }

	// public void UpdateGroups_Method()                      //? Called in GameManager //  
	// {
	// 	CalculateGrpLiberties();                                                   // Update All Group Liberties
	// 	DeleteZeroLibertyGroup_Methods();                                          // MethodGroup to Delete Grps with 0 Liberties, Set Empty ND
	// 	CalculateGrpLiberties();                                                   // Update All Group Liberties (Post ZeroLibGrp Deletion)
	// }



	// //* ---------------------------------------- CreateNewGroup  ----------------------------------------
	// public int CreateNewGroup(int crntNDID)   //? Called GameManager    // Creates New Group - Called in TargetNode - Returns grpID
	// {
	// 	NodeScript NDScr = GetNodeScriptByID(NDScrList, crntNDID);

	// 	// Group Attributes
	// 	Group newGrp = new Group();
	// 	int newGrpID = newGrp.grpID;
	// 	newGrp.Grp_ShpVal = NDScr.shpVal;

	// 	// Associate Group, Node, List
	// 	NDScr.grpID = newGrpID;
	// 	newGrp.NDID_List.Add(NDScr.NDID);

	// 	// Update Global Properties
	// 	allGrpList.Add(newGrp);
	// 	lastNDID = crntNDID;                                              // ! Sets last placed NDID for Script Reference
	// 	lastShpVal = NDScr.shpVal;

	// 	lastGrpID = newGrpID;           																	// TODO Check on change 

	// 	return newGrpID;
	// }
	// //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
	// public void AssignSheepToGroups(int targetNDID)
	// {
	// 	NodeScript NDScr = GetNodeScriptByID(NDScrList, targetNDID);

	// 	if (NDScr.shpVal != NDScr.shpValList[0]) {
	// 		if (NDScr.grpID == -1) {
	// 			NDScr.grpID = CreateNewGroup(NDScr.NDID); 			// Create new NodeGroup, assign grpID
	// 		}
			
	// 		int targetNDGID = NDScr.grpID;

	// 		// Adjacent Node Scripts
	// 		NodeScript LNDScr = NDScr.LNDScr;
	// 		NodeScript RNDScr = NDScr.RNDScr;
	// 		NodeScript BNDScr = NDScr.BNDScr;
	// 		NodeScript TNDScr = NDScr.TNDScr;

	// 		//* FIRST CHECK OCCURS ON LEFT NODE
	// 		// Left Node Found
	// 		if (LNDScr != null && LNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
	// 			int LNDGID = LNDScr.grpID;
	// 			if (LNDGID != targetNDGID && LNDGID != -1) {
	// 				JoinGroups(targetNDGID, LNDGID);
	// 			}
	// 		}
	// 		// Right Node Found
	// 		if (RNDScr != null && RNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
	// 			int RNDGID = RNDScr.grpID;
	// 			if (RNDGID != targetNDGID && RNDGID != -1) {
	// 				JoinGroups(targetNDGID, RNDGID);
	// 			}
	// 		}
	// 		// Bottom Node Found
	// 		if (BNDScr != null && BNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
	// 			int BNDGID = BNDScr.grpID;
	// 			if (BNDGID != targetNDGID && BNDGID != -1) {
	// 				JoinGroups(targetNDGID, BNDGID);
	// 			}
	// 		}
	// 		// Top Node Found
	// 		if (TNDScr != null && TNDScr.shpVal == NDScr.shpVal) {                   // ! Check adjNode = sheepVal as current player                  
	// 			int TNDGID = TNDScr.grpID;

	// 			if (TNDGID != targetNDGID && TNDGID != -1) {
	// 				JoinGroups(targetNDGID, TNDGID);
	// 			}
	// 		}

	// 		NDScr.lastPlaced = true;
	// 		lastNDID = NDScr.NDID;
	// 	}
	// }


	// //? ---------------------------------------- STATIC METHODS  ----------------------------------------



	// //* ---------------------------------------- GetGroup ----------------------------------------
	// public Group GetGroup(int grpID) {                                               // Returns Group from AllGrpList by grpID
	// 	Group group = allGrpList.FirstOrDefault(g => g.grpID == grpID);

	// 	return group;
	// }
	

	// //* -------------- JoinGroups ----------------------------------------
	// public void JoinGroups(int newGID, int prevGID)                             // Adds prevGIDs to newGrp.NDID_List - return newGrp.NDID_List 
	// {
	// 	if (prevGID != -1 && prevGID != newGID){                                // if prevGID is not null and doesn't match the newGrp 
	// 		Group newGrp  = GetGroup(newGID);                                      // Get NewGroup 
	// 		Group prevGrp = GetGroup(prevGID);                                    // Get PrevGroup

	// 		foreach (int NDID in prevGrp.NDID_List){                                   // For each ID in prevGrp 
	// 			newGrp.NDID_List.Add(NDID);                                          // Add ID to newGrp 
	// 			NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                   // Get Node with ID 
	// 			NDScr.GetComponent<NodeScript>().grpID = newGrp.grpID;             // Update grpID on NodeScript 
	// 		}

	// 		DeleteGroup(prevGID);                                                 // Delete prevGrp from allGrpList 
	// 	}

	// }


	// //* ---------------------------------------- DeleteGroup -----------------------------------------
	// public void DeleteGroup(int delGID)                                           // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
	// {
	// 	if (delGID != -1 && delGID != lastGrpID) {
	// 		Group grpToDelete = allGrpList.FirstOrDefault(g => g.grpID == delGID);
	// 		allGrpList.Remove(grpToDelete);

	// 		Debug.Log("Group #" + grpToDelete.grpID + " cleared and deleted.");
	// 	}
	// 	else
	// 	{
	// 		Debug.Log("grpID was -1");
	// 	}
	// }


	// //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
	// public void CalculateGrpLiberties()                                               // Updates Liberties of all Groups in AllGrpList
	// {
	// 	int totalGrpLibs = 0;

	// 	foreach (Group Grp in allGrpList){    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
	// 		List<NodeScript> NDScrList = new List<NodeScript>();

	// 		foreach (int NDID in Grp.NDID_List){
	// 			NodeScript crntNDScr = GetNodeScriptByID(NDScrList, NDID);
	// 			NDScrList.Add(crntNDScr);
	// 		}

	// 		List<NodeScript> countedNDs = new List<NodeScript>();                     //! holds adjNDScr to prevent double references for libVals 

	// 		foreach (NodeScript NDScr in NDScrList){                                   // Adds value of node to total liberties
				
	// 			if (NDScr.LNDScr != null){																				// If the adj node is not empty 
	// 				if (countedNDs.Contains(NDScr.LNDScr) == false) {                 // If the node is not already in the script (has been counted)
	// 					countedNDs.Add(NDScr.LNDScr);                              // Add it to the liberty node list
	// 					totalGrpLibs += NDScr.LNDScr.libVal;             // Add it's liberty value to the group ( 1 or 0 )
	// 				}
	// 			}
	// 			if (NDScr.RNDScr != null) {
	// 				if (countedNDs.Contains(NDScr.RNDScr) == false) {
	// 					countedNDs.Add(NDScr.RNDScr);
	// 					totalGrpLibs += NDScr.RNDScr.libVal;
	// 				}
	// 			}
	// 			if (NDScr.BNDScr != null) {
	// 				if (countedNDs.Contains(NDScr.BNDScr) == false) {
	// 					countedNDs.Add(NDScr.BNDScr);
	// 					totalGrpLibs += NDScr.BNDScr.libVal;
	// 				}
	// 			}
	// 			if (NDScr.TNDScr != null) {
	// 				if (countedNDs.Contains(NDScr.TNDScr) == false)
	// 				{
	// 					countedNDs.Add(NDScr.TNDScr);
	// 					totalGrpLibs += NDScr.TNDScr.libVal;
	// 				}
	// 			}
	// 		}

	// 		Grp.GrpLibs = totalGrpLibs;

	// 		totalGrpLibs = 0;
	// 		countedNDs.Clear();
	// 	}

	// }


	//* ---------------------------------------- CheckPlaceable ----------------------------------------

	// public bool CheckPlaceble(int NDID, int crntShpVal)                              // Calculate Group Capture
	// {
	// 	bool canPlace = false;                                                     // Set canPlace bool default at false

	// 	//? --------------------------------------------------------------------------// Get AdjGID and AdjGrp Lists
	// 	List<int> adjND_GIDList = new List<int>();
	// 	List<Group> adjGrpList  = new List<Group>();                                   // Get ajd Group and add to list

	// 	NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                        // Get TargetNode
	// 	List<NodeScript> adjNDScrList = NDScr.adjNDScrList;              							// Get adjNDScrList to iterate over Group Capture
	// 	foreach (NodeScript adjNDScr in adjNDScrList) {                                // Get ajdND Group ID and add to list
	// 		if (adjNDScr != null && adjND_GIDList.Contains(adjNDScr.grpID) == false) {  // If adjNDScr not null and not already in List
	// 			adjND_GIDList.Add(adjNDScr.grpID);                                  // Add to list
	// 		}
	// 	}
	// 	if (adjND_GIDList.Count > 0) {                                                // adjND are not all empty
	// 		foreach (int grpID in adjND_GIDList) {
	// 			if (grpID == -1) {                                                      // If grpID is -1, node is empty
	// 				canPlace = true;
	// 				Debug.Log("canPlace = " + canPlace);
	// 				return canPlace;                                               // Return true
	// 			}
	// 			if (grpID >= 0) {                                                       // if grpID is not default -1 (has been assigned a group)
	// 				adjGrpList.Add(GetGroup(grpID));
	// 			}
	// 		}
	// 	}

	// 	// --------------------------------------------------------------------------// Check isPlaceable Status
	// 	if (adjGrpList.Count == 0)                                                     // If no Grps in List (All Nodes are empty)
	// 	{
	// 		canPlace = true;
	// 		Debug.Log("canPlace = " + canPlace);
	// 		return canPlace;                                                       // Return true
	// 	}

	// 	foreach (Group adjGrp in adjGrpList)
	// 	{
	// 		if (adjGrp.Grp_ShpVal != crntShpVal && adjGrp.GrpLibs == 1)            // If adjGrp is not same ShpVal and liberties = 1 (can be captured)
	// 		{
	// 			canPlace = true;
	// 			Debug.Log("canPlace = " + canPlace);
	// 			return canPlace;                                               // Return true
	// 		}

	// 		if (adjGrp.Grp_ShpVal == crntShpVal && adjGrp.GrpLibs > 1)    // adjGrp is ShpVal and GrpLibs greater than 1
	// 		{
	// 			canPlace = true;
	// 			Debug.Log("canPlace = " + canPlace);
	// 			return canPlace;                                               // Return true
	// 		}
	// 	}

	// 	Debug.Log("canPlace = " + canPlace);
	// 	foreach (Group adjGrp in adjGrpList)
	// 	{
	// 		Debug.Log("Group " + adjGrp.grpID + " libs : " + adjGrp.GrpLibs + " ShpVal: " + adjGrp.Grp_ShpVal);
	// 		Debug.Log("Placed ShpVal: " + crntShpVal);
	// 	}

	// 	return canPlace;                                                           // Return false by Default

	// }


	// //* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------
	// public void DeleteZeroLibertyGroup_Methods()   // Called GameManager         // Sets Nodes in ZeroGrps to Empty, Deletes Group
	// {
	// 	List<int> zeroGIDList = GetZeroLibGIDList();                                // Update All Group Liberties
	// 	List<Group> zeroGrpList = GetZeroLibGrp(zeroGIDList);                         // Delete Groups with 0 Liberties
	// 	DeleteZeroLibGrps(zeroGrpList);
	// }


	// public List<int> GetZeroLibGIDList()                                   	// STEP 1  // Returns list from AllGrpList with GrpLiberties = 0
	// {
	// 	List<int> zero_GID_List = new List<int>();

	// 	foreach (Group group in allGrpList) {
	// 		if (group.GrpLibs == 0) {           																					// Returns a list of Groups with Liberties == 0 for deletion in other method
	// 			if (group.Grp_ShpVal != lastShpVal) {
	// 				zero_GID_List.Add(group.grpID);
	// 			}
	// 		}
	// 	}
	// 	return zero_GID_List;
	// }


	// public List<Group> GetZeroLibGrp(List<int> zero_GID_List)              // STEP 2  // Receives the zeroLibertyGID list from CalculateGrouLiberties()
	// {
	// 	List<Group> zero_Grp_List = new List<Group>();                                  // Create a new list for sorting

	// 	foreach (Group crntGrp in allGrpList) {                                          // Look through list of All Groups
	// 		if (zero_GID_List.Contains(crntGrp.grpID)) {                                 // If the zeroList contains the ID of a Zero'd Node Group
	// 			zero_Grp_List.Add(crntGrp);                                             // Add it to the zeroGrpList for updating
	// 		}
	// 	}
	// 	return zero_Grp_List;
	// }

	// public void DeleteZeroLibGrps(List<Group> zero_Grp_List)                 // STEP 3  // Deletes groups in zeroGrpList
	// {
	// 	if (zero_Grp_List.Count > 0) {
	// 		foreach (Group zeroGrp in zero_Grp_List) {                                    // Loop of new list of Zero liberty Groups
	// 			if (zeroGrp.NDID_List.Count == 1) {                                       // If there is a Single Node in the groups to Delete, check for Ko
	// 				checkForKo = true;
	// 				Debug.Log("checkForKo is True");
	// 			}
	// 			else {
	// 				checkForKo = false;
	// 			}

	// 			if (zeroGrp.Grp_ShpVal != lastShpVal) {                                // ! Checks if zeroGroup is the same sheepVal as current player
	// 				foreach (int zeroNDID in zeroGrp.NDID_List) {                        // Get the script of each Node and Set Node to Empty
	// 					NodeScript zeroND = GetNodeScriptByID(NDScrList, zeroNDID);
	// 					NodeScript zeroScript = zeroND.GetComponent<NodeScript>();
	// 					zeroScript.EmptySheepSetter();
	// 					zeroScript.SetNodeColor_Not_Selected();
	// 				}
	// 			}
	// 			DeleteGroup(zeroGrp.grpID);
	// 		}

	// 	}
	// 	else {
	// 		Debug.Log("No zeroLiberty Groups");
	// 	}

	// }


	// //* ---------------------------------------- GetAll_ND_GID ----------------------------------------

	// public List<int> GetAll_ND_GID() {
	// 	List<int> ND_GrpID_List = new List<int>();

	// 	foreach (NodeScript NDScr in NDScrList) {
	// 		if (NDScr != null) {
	// 			ND_GrpID_List.Add(NDScr.grpID);
	// 		}
	// 	}

	// 	All_ND_grpID_List = ND_GrpID_List;

	// 	return ND_GrpID_List;
	// }


	// //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
	
	// public void AssignSheepToGroups_REFACTOR(int newNDID) {
	// 	NodeScript newND = GetNodeScriptByID(NDScrList, newNDID);
	// 	NodeScript NDScr = newND.GetComponent<NodeScript>();

	// 	if (NDScr.grpID == -1) {																																								// Create new NodeGroup, assign grpID
	// 		NDScr.grpID = CreateNewGroup(NDScr.NDID);
	// 	}
	// 	int newNDGID = NDScr.grpID;

	// 	// Adjacent Node Scripts
	// 	List<NodeScript> adjNDScrList = NDScr.adjNDScrList;

	// 	foreach (NodeScript adjNDScr in adjNDScrList) {
	// 		if (adjNDScr != null && adjNDScr.shpVal == NDScr.shpVal) {                   // ! Checks if zeroGroup is the same sheepVal as current player                  
	// 			int adjNDGID = adjNDScr.grpID;
	// 			if(adjNDGID != newNDGID && adjNDGID != -1) {
	// 				JoinGroups(newNDGID, adjNDGID);
	// 			}
	// 		}
	// 		Debug.Log("adjNode " + adjNDScr.GetComponentInParent<Transform>().name + " added to Grp " + adjNDScr.grpID);
	// 	}

	// 	NDScr.lastPlaced = true;
	// 	lastNDID = NDScr.NDID;
	// }


	#endregion




	//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------

	public void PlaceSheep_OnClick() {
		int shpVal;
		if (Input.GetKeyDown(KeyCode.Mouse0)){ 
			shpVal = 1;
			NodeScript node = GetNDScr_OnClick();
			if(node != null){
				PlaceSheepMethod(node.NDID, shpVal);
			}
		}
	}


	public NodeScript GetNDScr_OnClick() {
		hitObject = GetRaycastHitObject();
		if(hitObject.layer == 8) {
			NodeScript node = hitObject.GetComponentInParent<NodeScript>();
			return node;
		}
		return null;
	}


	// // 09/05/2024 - Method Commented out to user Mouse1 for testing
	// public void PlaceWhiteSheep_OnClick_GM() {
	// 	// Check if the left mouse button was clicked
	// 	if (nodeSelected && canPlace == true && Input.GetKeyDown(KeyCode.Mouse1)) {
	// 		GM.PlaceWhiteSheepMethod_GM(NDID);
	// 		nodeSelected = false;
	// 		// Debug.Log("PlaceSheep_OnClick");
	// 	}
	// }


	// // Debug method for removing stones
	// public void PlaceEmptySheep_OnClick() {
	// 	// Check if the middle mouse button was clicked
	// 	if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse2))
	// 	{
	// 		PlaceEmptySheepMethod();
	// 	}
	// }




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






	//* ---------------------------------------- DEBUG METHODS  ----------------------------------------
	#region DEBUG METHODS

	void LogListValues<T>(List<T> list, string listName)
	{
		string values = string.Join(", ", list);
		Debug.Log($"{listName} values: [{values}]");
	}

	#endregion

}
