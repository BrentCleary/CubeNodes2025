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

	}

	// Update is called once per frame
	void Update(){ }


	//* ---------------------------------------- NODE DISPLAY METHODS ----------------------------------------

	public void UpdateNodeDisplay() {                                                // Updates Node Display 
		SetSheepDisplay();
		SetTileDisplay();
	}

	public void SetSheepDisplay()	{
		for (int i = shpTileList.Count - 1; i >= 0; i--) {
			shpTileList[i].SetActive(false);                               // Sets all SheepTiles to inactive
		}
		shpTileList[shpVal].SetActive(true);                             // Set Current SheepTile active
	}

	public void SetTileDisplay() {
    for (int i = 0; i < tileList.Count; i++) { 
			tileList[i].SetActive(false);	
		}                                                                                  // Sets all tiles inactive
    tileList[NDVal].SetActive(true);                                                   // Set current NDVal tile active
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




}