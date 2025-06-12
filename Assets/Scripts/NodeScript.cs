using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

    //* ---------------------------------------- (this) PROPERTIES ----------------------------------------
    public int  NDID;   // Set on Initialization in BoardGenerator
    public int  NDVal;
    public int  shpVal;
    public int  libVal;
    public bool hasGrp;
    public int  NDGrpID = -1;
    public bool canPlace;
    public bool lastPlaced;                                                            //? the most recently placed Node 
    
    public List<int> NDValList  = new List<int> { 0, 1, 2, 3, 4 };                    // Node Values when not occupied
    public List<int> shpValList = new List<int> {0, 1, 2};                           // { emptySpace, shpBlack, shpWhite }
    public List<int> libValList = new List<int> { 0, 1 };                          //  LibertyValue{ 1 , 0 }
    public List<bool> canPlaceList = new List<bool> { false, true };                  // is node canPlace for current player
    public List<GameObject> shpTileList = new List<GameObject> {};                   // { emptySpace, shpBlack, shpWhite }
    public List<GameObject> GrassTileList = new List<GameObject> {};


    //* ---------------------------------------- NODE ARRAY PROPERTIES ----------------------------------------
    public GameObject NDArray;
    public int[] arrPos = new int[2];


    //* ---------------------------------------- SHEEP GROUP PARAMETERS ----------------------------------------
    // Adjacent Nodes
    public GameObject LND;                                                           //? left
    public GameObject RND;                                                          //? right
    public GameObject TND;                                                            //? top
    public GameObject BND;                                                         //? bottom
    public NodeScript LNDScr;
    public NodeScript RNDScr;
    public NodeScript TNDScr;
    public NodeScript BNDScr;
    
    // Adjacent Node Scripts
    public List<NodeScript> adjNDScrList;
	public List<GameObject> adjNDList;

	public int LNDLibVal;
    public int RNDLibVal;
    public int BNDLibVal;
    public int TNDLibVal;


    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator BrdGntScr;
    public GroupManagerScript ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;


    //* ---------------------------------------- NODE COLOR VARIABLES ----------------------------------------
    [Header("Node Color Variables")]
    public GameObject grassContainer;
    public Material selectionMaterial;
    public List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;


    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                    //* Sets Initial Node Values to Default on Creation 
    // Start is called before the first frame update
    void Start()
    {
        NDVal = NDValList[4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
        libVal = 1;
        canPlace = true;
        shpVal = shpValList[0];
        adjNDList    = new List<GameObject>() {LND, RND, BND, TND };
        adjNDScrList = new List<NodeScript>() {LNDScr, RNDScr, BNDScr, TNDScr};

        // Get reference to Node Array and scripts
        NDArray = transform.parent.gameObject;
        BrdGntScr = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<GroupManagerScript>();

        // NodeColor Variables
        grassContainer = gameObject.transform.Find("GrassContainer").gameObject;
        BuildTileRendererList();
    
    }

    // Update is called once per frame
    void Update()
    {

    }



    //* ---------------------------------------- NODE DISPLAY METHODS ----------------------------------------

    public void UpdateNodeDisplay()                                                 // Updates Node Display 
    {
        SetSheepDisplay();
        SetTileDisplay();
    }

    public void SetSheepDisplay()
    {
        bool shpActive = true;
        
        // Sets all SheepTiles to inactive
        for (int i = shpTileList.Count-1; i >= 0; i--)    
        {
            shpTileList[i].SetActive(!shpActive);                               // Sets all SheepTiles to inactive
        }
        shpTileList[shpVal].SetActive(shpActive);                             // Set Current SheepTile active
    }

    public void SetTileDisplay()
    {
        bool tileActive = true;                                                     // Sets initialize bool
        for (int i = NDVal; i >= 0; i--)                                          // Sets all tiles from ND_Val and lower true
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = tileActive;
            GrassTileList[i].SetActive(tileActive);
        }
        for (int i = NDValList.Count - 1; i > NDVal; i--)                       // Sets all tiles above ND_Val false
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = !tileActive;
            GrassTileList[i].SetActive(!tileActive);
        }

        // SetNodeColor_Not_Selected();
    }




    //* ---------------------------------------- NODE COLOR SELECT METHODS ----------------------------------------
                            // Highlights/Resets selected Nodes Color by changing GrassTiles materials 

    public void SetNodeColor_Selected()
    {
        foreach(Renderer renderer in tileRendererList) {
            renderer.material = selectionMaterial;
        }
    }

    public void SetNodeColor_Not_Selected()
    {
        int colorCounter = 0;
        foreach(Renderer renderer in tileRendererList) {
            renderer.material = tileMaterialList[colorCounter];
            colorCounter++;
        }
    }

    public void BuildTileRendererList()
    {
        tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();
        
        foreach(Renderer renderer in tileRendererList) {
            tileMaterialList.Add(renderer.material);
            // Debug.Log("TileRenderList Added: " + renderer.material);
        }
    }



    // *---------------------------------------- SHEEP SETTER  METHODS ----------------------------------------
                                          // Called in TargetNode Script 

    public void BlackSheepSetter()                                          // Sets node to Black Sheep Object
    {
        shpVal = shpValList[1];                                         // Set shp value to blackSheep index
        libVal = libValList[0];                                     // Set libVal to 0
        NDVal  = NDValList [0];                                           // ND_Val is 0
        canPlace = false;
        lastPlaced = true;
        hasGrp = true;

    }


    public void WhiteSheepSetter()                                          // Sets node to Black Sheep Object
    {
        shpVal = shpValList[2];                                         // Set shp value to whiteSheep index
        libVal = libValList[0];                                     // Set libVal to 0
        NDVal  = NDValList [0];                                           // ND_Val is 0
        canPlace = false; 
        lastPlaced = true;
        hasGrp = true;

    }


    public void EmptySheepSetter()                                          // Sets node to Empty Sheep Object
    {
        shpVal = shpValList[0];                                         // Set shp value to emptySheep index
        libVal = libValList[1];                                     // Set libVal to 1
        NDVal  = NDValList [4];                                           // ND_Val is reset to 4
        canPlace = true;
        NDGrpID = -1;
        lastPlaced = false;
        hasGrp = false;

    }




//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    // Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                // Calls BoardGeneratorScript and NodeScript 
                                
    public void PlaceBlackSheepMethod()
    {
        // Check if the left mouse button was clicked
        BlackSheepSetter();                                                                  // Set Node to BlackshpVal
        BrdGntScr.UpdateBoardNodeValues();
        BrdGntScr.UpdateBoardDisplay();

        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }
    

    // 09/05/2024 - Method Commented out to user Mouse1 for testing
    public void PlaceWhiteSheepMethod()
    {
        WhiteSheepSetter();                                                                  // Set Node to BlackshpVal
        BrdGntScr.UpdateBoardNodeValues();
        BrdGntScr.UpdateBoardDisplay();
        
        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }


    public void PlaceEmptySheepMethod()
    {
        EmptySheepSetter();
    }




}