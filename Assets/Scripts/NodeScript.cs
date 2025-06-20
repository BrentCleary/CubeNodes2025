using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

	//* ---------------------------------------- (this) PROPERTIES ----------------------------------------
	public int NDID;   // Set on Initialization in BoardGenerator
	public int NDVal;
	public int shpVal;
	public int libVal;
	public int grpID = -1;
	public bool hasGrp;
	public bool canPlace;
	public bool lastPlaced;                                                            //? the most recently placed Node 

	public List<int> NDValList  = new List<int> { 0, 1, 2, 3, 4 };                    // Node Values when not occupied
	public List<int> shpValList = new List<int> { 0, 1, 2 };                          // { emptySpace, shpBlack, shpWhite }
	public List<int> libValList = new List<int> { 0, 1 };                             //  LibertyValue{ 1 , 0 }
	public List<bool> canPlaceList = new List<bool> { false, true };                  // is node canPlace for current player
	public List<GameObject> shpTileList = new List<GameObject> { };                   // { emptySpace, shpBlack, shpWhite }
	public List<GameObject> tileList = new List<GameObject> { };


	//* ---------------------------------------- NODE ARRAY PROPERTIES ----------------------------------------
	public GameObject NDArray;
	public int[] arrPos = new int[2];


	//* ---------------------------------------- GROUP PARAMETERS ----------------------------------------
	// Adjacent NodeScripts
	public NodeScript LNDScr;                                                         // left
	public NodeScript RNDScr;                                                         // right
	public NodeScript TNDScr;                                                         // top
	public NodeScript BNDScr;                                                         // bottom
	public List<NodeScript> adjNDScrList;


	//* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
	[SerializeField] GameManager GM;


	//* ---------------------------------------- SHEEP OBJ REFERENCES ----------------------------------------
	private GameObject whtShp;
	private GameObject blkShp;


	//* ---------------------------------------- NODE COLOR VARIABLES ----------------------------------------
	[Header("Node Color Variables")]
	public GameObject grassContainer;
	public Material   selectionMaterial;
	public List<Renderer> tileRendererList;
	public List<Material> tileMatList;


	//* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
	//* Sets Initial Node Values to Default on Creation 
	// Start is called before the first frame update
	void Start()
  {
		NDVal  = NDValList [4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
		shpVal = shpValList[0];
		libVal = 1;
		canPlace = true;
		adjNDScrList = new List<NodeScript>() { LNDScr, RNDScr, BNDScr, TNDScr };

		// Get reference to Node Array and scripts
		NDArray = GameObject.Find("nodeArray");
		GM = GameObject.Find("GameManagerObj").GetComponent<GameManager>();

		blkShp = transform.Find("BlackSheep").gameObject;
		whtShp = transform.Find("WhiteSheep").gameObject;


		// NodeColor Variables
		grassContainer = gameObject.transform.Find("TileContainer").gameObject;
		BuildTileRendererList();
	}

	// Update is called once per frame
	void Update(){

    // PlaceBlackSheep_OnClick_GM();
		// PlaceWhiteSheep_OnClick_GM();

  }




	//* ---------------------------------------- NODE DISPLAY METHODS ----------------------------------------

	public void UpdateNodeDisplay() {                                                // Updates Node Display 
		SetSheepDisplay();
		SetTileDisplay();
	}

	public void SetSheepDisplay()	{
		bool shpActive = true;

		for (int i = shpTileList.Count - 1; i >= 0; i--) {
			shpTileList[i].SetActive(!shpActive);                               // Sets all SheepTiles to inactive
		}
		shpTileList[shpVal].SetActive(shpActive);                             // Set Current SheepTile active
	}


	public void SetTileDisplay() {
		bool tileActive = true;                                                     // Sets initialize bool
		
    for (int i = 0; i < tileList.Count; i++) {                                         // Sets all tiles from ND_Val and lower true
			tileList[i].SetActive(!tileActive);
		}
    
    tileList[NDVal].SetActive(tileActive);

	}




	//* ---------------------------------------- NODE COLOR SELECT METHODS ----------------------------------------
	// Highlights/Resets selected Nodes Color by changing GrassTiles materials 

	public void SetNodeColor_Selected() {
		foreach (Renderer renderer in tileRendererList) {
			renderer.material = selectionMaterial;
		}
	}

	public void SetNodeColor_Not_Selected()	{
		int colorCounter = 0;
		foreach (Renderer renderer in tileRendererList) {
			renderer.material = tileMatList[colorCounter];
			colorCounter++;
		}
	}

	public void BuildTileRendererList()	{
		tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();
		foreach (Renderer renderer in tileRendererList) {
			tileMatList.Add(renderer.material);
		}
	}



	// *---------------------------------------- SHEEP SETTER  METHODS ----------------------------------------
	// Called in TargetNode Script 

	public void BlackSheepSetter()                                          // Sets node to Black Sheep Object
	{
		shpVal = shpValList[1];                                         // Set shp value to blackSheep index
		libVal = libValList[0];                                     // Set libVal to 0
		NDVal  = NDValList [0];                                           // ND_Val is 0
		blkShp.SetActive(true);
		whtShp.SetActive(false);
		hasGrp     = true;
		canPlace   = false;
		lastPlaced = true;
	}

	public void WhiteSheepSetter()                                          // Sets node to Black Sheep Object
	{
		shpVal = shpValList[2];                                         // Set shp value to whiteSheep index
		libVal = libValList[0];                                     // Set libVal to 0
		NDVal  = NDValList [0];                                           // ND_Val is 0
		blkShp.SetActive(false);
		whtShp.SetActive(true);
		hasGrp     = true;
		canPlace   = false;
		lastPlaced = true;
	}

	public void EmptySheepSetter()                                          // Sets node to Empty Sheep Object
	{
		shpVal = shpValList[0];                                         // Set shp value to emptySheep index
		libVal = libValList[1];                                     // Set libVal to 1
		NDVal  = NDValList [4];                                           // ND_Val is reset to 4
		blkShp.SetActive(false);
		whtShp.SetActive(false);
		grpID  = -1;
		hasGrp     = false;
		canPlace   = true;
		lastPlaced = false;
	}




	//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
	// Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
	// Calls BoardGeneratorScript and NodeScript 

	public void PlaceBlackSheepMethod()
	{
		// Check if the left mouse button was clicked
		BlackSheepSetter();                                                                  // Set Node to BlackshpVal
		GM.UpdateBoardNodeValues();
		GM.UpdateBoardDisplay();

		// GrpMngScr.UpdateGroupsMethod(gameObject);

	}

	// 09/05/2024 - Method Commented out to user Mouse1 for testing
	public void PlaceWhiteSheepMethod()
	{
		WhiteSheepSetter();                                                                  // Set Node to BlackshpVal
		GM.UpdateBoardNodeValues();
		GM.UpdateBoardDisplay();

		// GrpMngScr.UpdateGroupsMethod(gameObject);

	}


	public void PlaceEmptySheepMethod()
	{
		EmptySheepSetter();
	}


	//* ---------------------------------------- PROPERTIES ----------------------------------------

	public List<Material> tileMaterialList;
	public bool nodeSelected = false;


	// ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
	// Highlights/Resets selected Nodes Color by changing GrassTiles materials 


	private void OnMouseEnter()
	{
		nodeSelected = true;
		SetNodeColor_Selected();
	}

	private void OnMouseExit()
	{
		nodeSelected = false;
		SetNodeColor_Not_Selected();
	}


	// //* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
	// //* Sets Sheep on selected Node and calls BoardGeneratorScript to reset display

	// public void PlaceBlackSheep_OnClick_GM()
	// {
	// 	if (nodeSelected && canPlace == true && Input.GetKeyDown(KeyCode.Mouse0)) {
	// 		GM.PlaceBlackSheepMethod(NDID);
	// 		nodeSelected = false;
	// 		// Debug.Log("PlaceBlackSheep_OnClick");
	// 	}
	// }

	// // 09/05/2024 - Method Commented out to user Mouse1 for testing
	// public void PlaceWhiteSheep_OnClick_GM() {
	// 	// Check if the left mouse button was clicked
	// 	if (nodeSelected && canPlace == true && Input.GetKeyDown(KeyCode.Mouse1)) {
	// 		GM.PlaceWhiteSheepMethod_GM(NDID);
	// 		nodeSelected = false;
	// 		// Debug.Log("PlaceBlackSheep_OnClick");
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








}