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

  //* ---------------------------------------- GROUP MANAGER PROPS ----------------------------------------
  [Header("Group Properties")]
  public int  lastNDID;
  public int  lastShpVal;
  public int  lastGrpID;
  public bool checkForKo;
  public List<Group> allGrpList;

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
            // OnValueChanged(); 
  }}}

  //* ---------------------------------------- BOARD PROPS ----------------------------------------
  [Header("Board Array Settings")] 																					
  [SerializeField] public List<GameObject> NDList;
  [SerializeField] public List<NodeScript> NDScrList;
  [SerializeField] public GameObject[,] ND_Arr;
  public GameObject NDArr;
  public Transform NDArrTransform;

  public int arrColLen = 3;                                              // ! Array Column Size
  public int arrRowLen = 3;                                              // ! Array Row Size
  public int arrSize => arrColLen * arrRowLen;                           // arrColLen * arrRowLen
  public float ND_Spacing = 1f;                                          // Space Between Nodes

  public GameObject ND_Prefab;
  public List<int> startNDValMap;
  public List<int> crntNDValMap;



  //* ----------------------------------------  MANAGER METHODS ----------------------------------------

  void Awake() 
  {
    NDArr 		 = GameObject.Find("nodeArray");
    pauseMenu  = GameObject.Find("PauseMenu");
    mainCamera = Camera.main;
  }

  void Start() 
  {
    // Board
    CreateBoard();
    NodeValueUpdate();
    UpdateBoardDisplay();

    nowBrdState = CreateShpValMap();
    prvBrdState = nowBrdState.ToList();
    allGrpList = new List<Group>();
  }

  void Update() 
  {
    DrawRay();
    PlaceSheep_OnClick();
  }




  //* ----------------------------------------  PLACE SHEEP METHODS ----------------------------------------

  public void PlaceSheepMethod(int ND_ID, int shpVal) {
    NodeScript crntNDScr = GetNodeScriptByID(NDScrList, ND_ID);                                                            // Set blackSheepVal

    Debug.Log("shpVal = " + shpVal);

    bool isPlaceAble = CheckPlaceble(ND_ID, shpVal);
    Update_Ko_Status(ND_ID, shpVal);

    if (isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
    {
      if (shpVal == 0) { crntNDScr.EmptySheepSetter(); }
      if (shpVal == 1) { crntNDScr.BlackSheepSetter(); }
      if (shpVal == 2) { crntNDScr.WhiteSheepSetter(); }                                                               // Set Node to BlacksheepVal

      NodeValueUpdate();                                                          // Update Value of All BoardNodes
        CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
        UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 
      NodeValueUpdate();                                                          // Update All NodeValues after Group Deletions
        UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

      UpdateBoardDisplay();                                                             // Update Board Display

      prvBrdState = nowBrdState.ToList();
      // LogListValues<int>(prvBrdState, "prvBrdState GM");

      nowBrdState = CreateShpValMap();
      //LogListValues<int>(nowBrdState, "nowBrdState GM");

      prvShpVal = prvBrdState[ND_ID];
      nowShpVal = nowBrdState[ND_ID];

    }
  }


  public void Update_Ko_Status(int ND_ID, int shpVal)
  {
    if (checkForKo == true) {
      isKo = CheckMapForKo(prvBrdState, ND_ID, shpVal);
    }
    else { isKo = false; }
  }


  public void PlaceSheep_OnClick()
  {
    int shpVal;
    if (Input.GetKeyDown(KeyCode.Mouse0)){ 
      shpVal = 1;
      NodeScript node = GetNDScr_OnClick();	
      if(node != null) { PlaceSheepMethod(node.NDID, shpVal); }
    }
    if (Input.GetKeyDown(KeyCode.Mouse1)){ 
      shpVal = 2;
      NodeScript node = GetNDScr_OnClick();
      if(node != null) { PlaceSheepMethod(node.NDID, shpVal); }
    }
  }


  public NodeScript GetNDScr_OnClick() {
    hitObject = GetRaycastHitObject();
    if(hitObject.layer == 8) {
      NodeScript node = hitObject.GetComponentInParent<NodeScript>();
      return node;
    }
    else{ return null; }
  }



  //* ---------------------------------------- RAYCAST METHODS ----------------------------------------
  #region RAYCAST METHODS

  public GameObject GetRaycastHitObject() {
    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    nodeLayerMask = LayerMask.GetMask("Node");

    if (Physics.Raycast(ray, out hit, 1000, nodeLayerMask)) { return hit.transform.gameObject; }   // Return the GameObject that the ray hits
    else{	return null; }                                                                           // Return null if no object is hit
  }

  public void DrawRay()	{
    if (mainCamera == null)	return;

    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);																		 // Get a ray from the mouse position
    Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);																	 // Draw ray from camera toward mouse direction 1000 long
  }

  public void OnValueChanged() {
    if (_hitObject != null) 
      { Debug.Log("Raycast hit: " + _hitObject.name); }
    else 
      { Debug.Log("Raycast hit: null"); }
  }


  #endregion


  //* ---------------------------------------- START BOARD METHODS ----------------------------------------
  #region BOARD METHODS

  public void CreateBoard() {                                                   // Instantiates Variables, Calls Methods Below
  
    ND_Arr = new GameObject[arrColLen, arrRowLen];
    NDArrTransform = NDArr.transform;
    NDList = new List<GameObject>();
    NDScrList = new List<NodeScript>();

    InstantiateNodes();					// Creates a list of Node GameObjects
    SetNodeTransformPosition(); // Sets Transfrom of All Nodes
    BuildNodeArray();						// Creates Array. Places nodes.
    AdjNodeScrMapper();						// Associates Nodes to Neighbors
  }
  
  public void InstantiateNodes(){                                               // Instantiates Nodes, Assigns names and values, Adds them to NDList
    for (int i = 0; i < arrSize; i++) {
      GameObject node = Instantiate(ND_Prefab, NDArrTransform);
      node.name = $"Node ({i})";
      node.GetComponent<NodeScript>().NDID = i;                                 //! Sets NDID in NodeScript
      NDList.Add(node);															                            //! Adds node to NDlist
      NDScrList.Add(node.GetComponent<NodeScript>());                           //! Adds NDScr to NDScrList
    }
  }

  public void SetNodeTransformPosition() {                                      // Set Node transform.position
    int count = 0;                                                              // Increments Node reference in NDList 
    for (int i = 0; i < arrColLen; i++){                                        // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++){
        NDList[count].transform.position = new Vector3(i * ND_Spacing, 0, j * ND_Spacing);
        count++;
      }
    }
  }

  public void BuildNodeArray() {                                                // Generate Array using Length x Row using nodes in NDList
    int count = 0;                                                              // Increments Node reference in NDList 
    for (int i = 0; i < arrColLen; i++){                                        // Assigns positions to each gNode in ND_Arr
      for (int j = 0; j < arrRowLen; j++){
        ND_Arr[i, j] = NDList[count];                                           // Set curent NDList object to current array position
        ND_Arr[i, j].name = $"{ND_Arr[i, j].name} [{i},{j}]";                   // Add Array Position to Node Name
        
        NodeScript NDScr = NDScrList[count];                                    // Maps Board Array position for reference
        NDScr.arrPos[0] = i;
        NDScr.arrPos[1] = j;
        
        count++;
      }
    }
  }

  public void AdjNodeScrMapper() {                                          // Loop over NDScrList, assign adjNodes
    foreach (NodeScript NDScr in NDScrList){
      int[] arrPos = NDScr.arrPos;
      NDScr.LNDScr = (arrPos[0] == 0) 						? null : ND_Arr[arrPos[0]-1, arrPos[1]	].GetComponent<NodeScript>();
      NDScr.RNDScr = (arrPos[0] == arrColLen - 1) ? null : ND_Arr[arrPos[0]+1, arrPos[1]	].GetComponent<NodeScript>();
      NDScr.BNDScr = (arrPos[1] == 0) 						? null : ND_Arr[arrPos[0]	 , arrPos[1]-1].GetComponent<NodeScript>();
      NDScr.TNDScr = (arrPos[1] == arrRowLen - 1) ? null : ND_Arr[arrPos[0]	 , arrPos[1]+1].GetComponent<NodeScript>();
    }
  }

  //* ---------------------------------------- BOARD UPDATE METHODS ----------------------------------
  public void NodeValueUpdate() {                                      							// Displays Array based on nodeValues
    
    List<int> NDValMap = new List<int>();                                           // 
    
    // Set all node values to 4 (empty) or 0 (sheep)
    foreach (NodeScript scr in NDScrList) {
      if (scr.shpVal == scr.shpValList[0]) {                                     		// If No Sheep
        NDValMap.Add(scr.NDValList[4]);                                           	// Assigns 4 (max value) at map position
        scr.libVal = scr.libValList[1];
      }
      else{                                                                         // If Sheep present
        NDValMap.Add(scr.NDValList[0]);                                           	// Assigns 0 (min value) at map position
        scr.libVal = scr.libValList[0];
      }
    }

    // Subtract 1 from all node values where adjNode libVal == 0 (has sheep)
    List<int> newValMap = NDValMap;                                           			// Copy of the NDValMap to update, to preserve  
    int crntNDVal = 0;
    foreach(NodeScript scr in NDScrList) {
      if(scr.LNDScr == null || scr.LNDScr.libVal == 0 ) { newValMap[crntNDVal] -= 1; }
      if(scr.RNDScr == null || scr.RNDScr.libVal == 0 ) { newValMap[crntNDVal] -= 1; }
      if(scr.BNDScr == null || scr.BNDScr.libVal == 0 ) { newValMap[crntNDVal] -= 1; }
      if(scr.TNDScr == null || scr.TNDScr.libVal == 0 ) { newValMap[crntNDVal] -= 1; }
      crntNDVal += 1;
    }

    // Set NDVal of each NDScr to the newValMap value (updated value)
    int count = 0;
    foreach(NodeScript scr in NDScrList) {
      if(newValMap[count] < 0) { scr.NDVal = 0; }
      else{ scr.NDVal = newValMap[count]; }
      count += 1;
    }
  }

  public void UpdateBoardDisplay() {                                                                         // Updated Display of Nodes
    foreach (NodeScript scr in NDScrList)	{ 
      scr.UpdateNodeDisplay(); 
    }
  }


  // *---------------------------------------- Ko Check Methods ---------------------------------
  
  // Creates List of Board values to compare between turns to check for Ko Status
  public List<int> CreateShpValMap() { 																														// Displays Array based on nodeValues
    List<int> ShpValMap = new List<int>();                                         // List to hold update values for arrayNodes
    
    foreach(NodeScript scr in NDScrList) { 
      ShpValMap.Add(scr.shpVal);	
    }

    return ShpValMap;
  }


  public bool CheckMapForKo(List<int> prevShpValMap, int ND_ID, int ShpVal)        // Map of board state before last move, ND_ID and Value
  {
    bool isKo = false;                                                              // Ko is initially set false

    List<int> newShpValMap = prevShpValMap.ToList();                                // Create a new copy of ShpValMap for updating and comparing
    newShpValMap[ND_ID] = ShpVal;                                                   // Change List val to ShpVal at ND_ID index

    bool sequenceCheck = newShpValMap.SequenceEqual(prevShpValMap);
    Debug.Log("sequenceCheck: " + sequenceCheck);

    if (sequenceCheck) {                                              // Compare to prev ShpValMap for Board state
      FindDifferences(newShpValMap, prevShpValMap);
      isKo = true;                                                                // If they match, the move will violate KO rules
      Debug.Log("** KO is True **");
      // LogListValues<int>(prevShpValMap, "prevShpValMap");
      // LogListValues<int>(newShpValMap, "newShpValMap");
    }
    else { isKo = false; }

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

  public void CreateGroup_Method(int crntNDID) {
    CreateNewGroup(crntNDID);                                                   // Creates newGrp, Associate Properties
    AssignSheepToGroups(crntNDID);                                              // Assign Adj Grps to newgrpID
  }

  public void UpdateGroups_Method() {
    CalculateGrpLiberties();                                                    // Update All Group Liberties
    DeleteZeroLibGrp();                                                         // Delete Grps with 0 Liberties, Set Empty Nodes
    CalculateGrpLiberties();                                                    // Update All Group Liberties (Post ZeroGrp Deletion)
  }


  //* ---------------------------------------- CreateNewGroup  ----------------------------------------
  public int CreateNewGroup(int crntNDID)                                       //? creates new Group, returns grpID
  {
    Group newGrp = Group.Create();
    NodeScript NDScr = GetNodeScriptByID(NDScrList, crntNDID);

    // Group Attributes
    int newGrpID = newGrp.grpID;
    newGrp.Grp_ShpVal = NDScr.shpVal;
    newGrp.NDIDList.Add(NDScr.NDID);

    // Set Node Properties
    NDScr.grpID = newGrpID;
    NDScr.lastPlaced = true;

    // Update Global Properties
    allGrpList.Add(newGrp);
    lastNDID   = crntNDID;                                                      // ! Sets last placed NDID for Script Reference
    lastShpVal = NDScr.shpVal;
    lastGrpID  = newGrpID;

    return newGrpID;

  }

  //* ---------------------------------------- AssignSheepToGroups ----------------------------------------
  public void AssignSheepToGroups(int targetNDID) {

    NodeScript NDScr = GetNodeScriptByID(NDScrList, targetNDID);
    int targetGID = NDScr.grpID;

    foreach(NodeScript adjScr in NDScr.adjNDScrList) {                        // First check occurs on left node
      if (adjScr != null && adjScr.shpVal == NDScr.shpVal) {                  // ! Check adjNode = sheepVal as current player                  
        int adjGID  = adjScr.grpID;
				if (adjGID != targetGID && adjGID != -1) { 
          JoinGroups(targetGID, adjGID); 
    }}}

  }


  //* ---------------------------------------- GetGroup ----------------------------------------
  public Group GetGroup(int grpID) {                                          //? Returns Group from AllGrpList by grpID
    Group  group = allGrpList.FirstOrDefault(g => g.grpID == grpID);
    return group;
  }
  

  //* ---------------------------------------- JoinGroups ----------------------------------------
  public void JoinGroups(int newGID, int prevGID)                             //? Adds prevGIDs to newGrp.NDID_List
  {
    if (prevGID != -1 && prevGID != newGID){                                  // if prevGID is not null and doesn't match the newGrp 
      Group newGrp  = GetGroup(newGID);                                       // Get NewGroup 
      Group prevGrp = GetGroup(prevGID);                                      // Get PrevGroup

      foreach (int NDID in prevGrp.NDIDList){                                 // For each ID in prevGrp 
        newGrp.NDIDList.Add(NDID);                                            // Add ID to newGrp 
        NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                // Get Node with ID 
        NDScr.grpID = newGrp.grpID;                                           // Update grpID on NodeScript 
      }

      DeleteGroup(prevGID);                                                   // Delete prevGrp from allGrpList 
    }

  }


  //* ---------------------------------------- DeleteGroup -----------------------------------------
  public void DeleteGroup(int delGID)                                           // Clears prevGrp Nodelist - Removes prevGrp from AllGrpList 
  {
    if (delGID != -1 && delGID != lastGrpID) {
      Group grpToDelete = allGrpList.FirstOrDefault(g => g.grpID == delGID);
      allGrpList.Remove(grpToDelete);
      Debug.Log("Group #" + grpToDelete.grpID + " cleared and deleted.");
    }
    else { Debug.Log("grpID was -1");	}
  }


  //* ---------------------------------------- CalculateGrpLiberties ----------------------------------------
  public void CalculateGrpLiberties() {                                          //? Updates Liberties of all Groups in AllGrpList
    
    int totalGrpLibs = 0;
    foreach (Group grp in allGrpList) {                                         // Loops over List of All Groups

      List<NodeScript> grpScrList = new List<NodeScript>();
      foreach (int NDID in grp.NDIDList) {
        NodeScript scr = GetNodeScriptByID(NDScrList, NDID);              // Check NDID In grp.NDIDList against NDScrList (all scripts)
        grpScrList.Add(scr);
      }
      
      List<NodeScript> countedNDs = new List<NodeScript>();                     //! holds adjNDScr to prevent double references for libVals 
      foreach (NodeScript scr in grpScrList){                                   // loops over nodeScript listed in the group
        foreach(NodeScript adjScr in scr.adjNDScrList) {                        // loops over adjacent nodeScripts in node
          if (adjScr != null && countedNDs.Contains(adjScr) == false) {         // If the adj node is not empty && not in counted scr list
              totalGrpLibs += adjScr.libVal;                                    // Add it's liberty value to the group ( 1 or 0 )
              countedNDs.Add(adjScr);                                           // Add the script to the counted nodeScript list
      }}}

      grp.GrpLibs = totalGrpLibs;                                               // Set current grp libs to totalGrpLibs
      totalGrpLibs = 0;                                                         // Reset totalGrpLibs to 0
      countedNDs.Clear();                                                       // Clear list of counted Nodes
    }
  }


  //* ---------------------------------------- CheckPlaceable ----------------------------------------

  public bool CheckPlaceble(int NDID, int crntShpVal) {                         //? Calculate Group Capture
    
    NodeScript NDScr = GetNodeScriptByID(NDScrList, NDID);                      // Get TargetNode
    List<int>        adjGIDList   = new List<int>();                            // List to identify the grpID of adjacent nodes 
    List<Group>      adjGrpList   = new List<Group>();                          // Get ajd Group and add to list
    List<NodeScript> adjNDScrList = NDScr.adjNDScrList;                         // Get adjNDScrList to iterate over Group Capture
    
    foreach (NodeScript scr in adjNDScrList) {                                  //? Get ajdND Group ID and add to list
      if (scr != null && adjGIDList.Contains(scr.grpID) == false) {             // If scr not null and not already in List
        adjGIDList.Add(scr.grpID);                                              // Add to list
    }}

		if (adjGIDList.Count == 0) { return true; }                                 // no Grps in List (All Nodes are empty)
    if (adjGIDList.Count >  0) {                                                // adjND not all empty
      foreach (int grpID in adjGIDList) {
				if (grpID == -1) { return true; }                                       // grpID is  -1, no  group, node is empty
				if (grpID >=  0) { adjGrpList.Add(GetGroup(grpID)); }                   // grpID not -1, has group, node occupied
		}}
    
    foreach (Group adjGrp in adjGrpList) {
			if (adjGrp.Grp_ShpVal != crntShpVal && adjGrp.GrpLibs == 1) { return true; }  // adjGrp not ShpVal and grpLibs = 1 (can capture)
			if (adjGrp.Grp_ShpVal == crntShpVal && adjGrp.GrpLibs >  1) { return true; }  // adjGrp is  ShpVal and grpLibs > 1 (won't reduce to 0)
    }

    return false;                                                               // Return false by Default

  }


  //* ---------------------------------------- ZeroLibertyGroup Methods ----------------------------------------

  public void DeleteZeroLibGrp()                                     	          // Returns list from AllGrpList with GrpLiberties = 0
  {
    List<int> zeroGIDList = new List<int>();
    foreach (Group grp in allGrpList) {
      if (grp.GrpLibs == 0) {           																				// Returns a list of Groups with Liberties == 0 for deletion in other method
        if (grp.Grp_ShpVal != lastShpVal) {                                     // ! IMPT - Prevents current player from killing own groups
          zeroGIDList.Add(grp.grpID);
    }}}
    
    List<Group> zGrpList = new List<Group>();                                   // Create a new list for sorting
    foreach (Group grp in allGrpList) {                                         // Look through list of All Groups
      if (zeroGIDList.Contains(grp.grpID)) {                                    // If the zeroList contains the ID of a Zero'd Node Group
        zGrpList.Add(grp);                                                      // Add it to the zGrpList for updating
    }}

    if (zGrpList.Count > 0) {
      foreach (Group zeroGrp in zGrpList) {                                     // Loop of new list of Zero liberty Groups
				checkForKo = false;
				if (zeroGrp.NDIDList.Count == 1) { checkForKo = true; }                 // If there is a Single Node in the groups to Delete, check for Ko
				if (zeroGrp.Grp_ShpVal != lastShpVal) {                                 // ! Checks if zeroGroup is the same sheepVal as current player
          foreach (int zeroNDID in zeroGrp.NDIDList) {                          // Get the script of each Node and Set Node to Empty
            NodeScript zeroND  = GetNodeScriptByID(NDScrList, zeroNDID);
            NodeScript zeroScr = zeroND.GetComponent<NodeScript>();
            zeroScr.EmptySheepSetter();
        }}
        
        DeleteGroup(zeroGrp.grpID);
      }
    }

  }


  //* ---------------------------------------- GetAll_ND_GID ----------------------------------------

  public List<int> GetAll_ND_GID() {
    
    List<int> grpIDList = new List<int>();
    foreach (NodeScript NDScr in NDScrList) {
      if (NDScr != null) { grpIDList.Add(NDScr.grpID); }
    }
    return grpIDList;
  }


  #endregion





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
