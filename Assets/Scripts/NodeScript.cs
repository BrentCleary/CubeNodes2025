using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

    //* ---------------------------------------- (this) PROPERTIES ----------------------------------------
    public int nodeID;   // Set on Initialization in BoardGenerator

    public int NDValue;
    public List<int> NDValueList = new List<int> { 0, 1, 2, 3, 4 };                    // Node Values when not occupied
    public List<GameObject> GrassTileList = new List<GameObject> {};
    
    public int sheepVal;
    public List<int> sheepValList = new List<int> {0, 1, 2};                           // { emptySpace, sheepBlack, sheepWhite }
    public List<GameObject> sheepTileList = new List<GameObject> {};                   // { emptySpace, sheepBlack, sheepWhite }

    public int libertyVal;
    public List<int> libertyValList = new List<int> { 0, 1 };                          //  LibertyValue{ 1 , 0 }
    
    public bool placeAble;
    public List<bool> placeAbleList = new List<bool> { false, true };                  // is node placeable for current player

    public bool lastPlaced;                                                            //? the most recently placed Node 

    //* ---------------------------------------- NODE ARRAY PROPERTIES ----------------------------------------
    public GameObject NDArray;
    public int[] arrayPosition = new int[2];


    //* ---------------------------------------- SHEEP GROUP PARAMETERS ----------------------------------------
    // Adjacent Nodes
    public GameObject leftND;                                                           //? left
    public NodeScript leftNDScript;
    public GameObject rightND;                                                          //? right
    public NodeScript rightNDScript;
    public GameObject topND;                                                            //? top
    public NodeScript topNDScript;
    public GameObject bottomND;                                                         //? bottom
    public NodeScript bottomNDScript;
    // Adjacent Node Scripts
    public List<NodeScript> adjNDScriptList;
    public List<GameObject> adjNDList;

    public int leftNDLibertyVal;
    public int rightNDLibertyVal;
    public int bottomNDLibertyVal;
    public int topNDLibertyVal;
    public int? NDgrpID = null;


    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator Brd_Gntr_Script;
    public NodeGroupManager ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;



    //* ---------------------------------------- NODE COLOR VARIABLES ----------------------------------------
    // Node Color Variables
    public GameObject grassContainer;
    public Material selectionMaterial;
    private List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;


    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                    //* Sets Initial Node Values to Default on Creation 
    // Start is called before the first frame update
    void Start()
    {
        NDValue = NDValueList[4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
        libertyVal = 1;
        placeAble = true;
        sheepVal = sheepValList[0];
        adjNDScriptList = new List<NodeScript>() {leftNDScript, rightNDScript, bottomNDScript, topNDScript};
        adjNDList = new List<GameObject>() {leftND, rightND, bottomND, topND };

        // Get reference to Node Array and scripts
        NDArray = transform.parent.gameObject;
        Brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<NodeGroupManager>();

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
        bool sheepActive = true;
        
        // Sets all SheepTiles to inactive
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].SetActive(!sheepActive);                               // Sets all SheepTiles to inactive
        }
        sheepTileList[sheepVal].SetActive(sheepActive);                             // Set Current SheepTile active
    }

    public void SetTileDisplay()
    {
        bool tileActive = true;                                                     // Sets initialize bool
        for (int i = NDValue; i >= 0; i--)                                          // Sets all tiles from NDValue and lower true
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = tileActive;
            GrassTileList[i].SetActive(tileActive);
        }
        for (int i = NDValueList.Count - 1; i > NDValue; i--)                       // Sets all tiles above NDValue false
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
            Debug.Log("TileRenderList Added: " + renderer.material);
        }
    }



    // *---------------------------------------- SHEEP SETTER  METHODS ----------------------------------------
                                          // Called in TargetNode Script 

    public void BlackSheepSetter()                                          // Sets node to Black Sheep Object
    {
        sheepVal = sheepValList[1];                                         // Set sheep value to blackSheep index
        libertyVal = libertyValList[0];                                     // Set libertyVal to 0
        NDValue = NDValueList[0];                                           // NDValue is 0
        placeAble = false;
        lastPlaced = true;

    }


    public void WhiteSheepSetter()                                          // Sets node to Black Sheep Object
    {
        sheepVal = sheepValList[2];                                         // Set sheep value to whiteSheep index
        libertyVal = libertyValList[0];                                     // Set libertyVal to 0
        NDValue = NDValueList[0];                                           // NDValue is 0
        placeAble = false;
        lastPlaced = true;

    }


    public void EmptySheepSetter()                                          // Sets node to Empty Sheep Object
    {
        sheepVal = sheepValList[0];                                         // Set sheep value to emptySheep index
        libertyVal = libertyValList[1];                                     // Set libertyVal to 1
        NDValue = NDValueList[4];                                           // NDValue is reset to 4
        placeAble = true;
        NDgrpID = null;
        lastPlaced = false;

    }




//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    // Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                // Calls BoardGeneratorScript and NodeScript 
                                
    public void PlaceBlackSheepMethod()
    {
        // Check if the left mouse button was clicked
        BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gntr_Script.UpdateBoardNodeValues();
        Brd_Gntr_Script.UpdateBoardDisplay();

        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }
    

    // 09/05/2024 - Method Commented out to user Mouse1 for testing
    public void PlaceWhiteSheepMethod()
    {
        WhiteSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gntr_Script.UpdateBoardNodeValues();
        Brd_Gntr_Script.UpdateBoardDisplay();
        
        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }


    public void PlaceEmptySheepMethod()
    {
        EmptySheepSetter();
    }




}