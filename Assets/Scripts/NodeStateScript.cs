using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Refactor{
public class NodeStateScript : MonoBehaviour
{

    //* ---------------------------------------- (this) PROPERTIES ----------------------------------------
    public int ND_ID;   // Set on Initialization in BoardGenerator

    public int ND_Val;
    public List<int> ND_Val_List = new List<int> { 0, 1, 2, 3, 4 };                    // Node Values when not occupied
    public List<GameObject> GrassTileList = new List<GameObject> {};
    
    public int Clr_Idx;                                                                // Color Index
    public List<int> Clr_Idx_List = new List<int> {0, 1, 2};                           // { empty, Black, White }
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
    public GameObject lft_ND;                                                           //? left
    public NodeScript lft_ND_Scr;
    public GameObject rgt_ND;                                                          //? right
    public NodeScript rgt_ND_Scr;
    public GameObject top_ND;                                                            //? top
    public NodeScript top_ND_Scr;
    public GameObject btm_ND;                                                         //? bottom
    public NodeScript btm_ND_Scr;
    // Adjacent Node Scripts
    public List<NodeScript> adj_ND_Scr_List;
    public List<GameObject> adj_ND_List;

    public int lft_ND_Lib;
    public int rgt_ND_Lib;
    public int btm_ND_Lib;
    public int top_ND_Lib;

    public int ND_Grp_ID = -1;
    public bool isInGrp;


    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator Brd_Gen_Scr;
    public GroupManagerScript Grp_Mng_Scr;
    public TargetNode Tgt_ND_Scr;



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
        ND_Val = ND_Val_List[4];  // sets value to 4 - { 0, 1, 2, 3, 4 }
        libertyVal = 1;
        placeAble = true;
        Clr_Idx = Clr_Idx_List[0];
        adj_ND_Scr_List = new List<NodeScript>() {lft_ND_Scr, rgt_ND_Scr, btm_ND_Scr, top_ND_Scr};
        adj_ND_List = new List<GameObject>() {lft_ND, rgt_ND, btm_ND, top_ND };

        // Get reference to Node Array and scripts
        NDArray = transform.parent.gameObject;
        Brd_Gen_Scr = NDArray.GetComponent<BoardGenerator>();
        Grp_Mng_Scr = NDArray.GetComponent<GroupManagerScript>();

        // NodeColor Variables
        grassContainer = gameObject.transform.Find("GrassContainer").gameObject;
        BuildTileRendererList();
    
    }

    



    //* ---------------------------------------- NODE DISPLAY METHODS ----------------------------------------

    public void UpdateNodeDisplay()                                                 // Updates Node Display 
    {
        SetSheepDisplay();
        SetTileDisplay();
    }

    public void SetSheepDisplay()
    {
        bool ND_Active = true;
        
        // Sets all SheepTiles to inactive
        for (int i = sheepTileList.Count-1; i >= 0; i--)    
        {
            sheepTileList[i].SetActive(!ND_Active);                               // Sets all SheepTiles to inactive
        }
        sheepTileList[Clr_Idx].SetActive(ND_Active);                             // Set Current SheepTile active
    }

    public void SetTileDisplay()
    {
        bool tileActive = true;                                                     // Sets initialize bool
        for (int i = ND_Val; i >= 0; i--)                                          // Sets all tiles from NDValue and lower true
        {
            GrassTileList[i].GetComponent<MeshRenderer>().enabled = tileActive;
            GrassTileList[i].SetActive(tileActive);
        }
        for (int i = ND_Val_List.Count - 1; i > ND_Val; i--)                       // Sets all tiles above NDValue false
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
        Clr_Idx = Clr_Idx_List[1];                                         // Set sheep value to blackSheep index
        libertyVal = libertyValList[0];                                     // Set libertyVal to 0
        ND_Val = ND_Val_List[0];                                           // NDValue is 0
        placeAble = false;
        lastPlaced = true;
        isInGrp = true;

    }


    public void WhiteSheepSetter()                                          // Sets node to Black Sheep Object
    {
        Clr_Idx = Clr_Idx_List[2];                                         // Set sheep value to whiteSheep index
        libertyVal = libertyValList[0];                                     // Set libertyVal to 0
        ND_Val = ND_Val_List[0];                                           // NDValue is 0
        placeAble = false;
        lastPlaced = true;
        isInGrp = true;

    }


    public void EmptySheepSetter()                                          // Sets node to Empty Sheep Object
    {
        Clr_Idx = Clr_Idx_List[0];                                         // Set sheep value to emptySheep index
        libertyVal = libertyValList[1];                                     // Set libertyVal to 1
        ND_Val = ND_Val_List[4];                                           // NDValue is reset to 4
        placeAble = true;
        ND_Grp_ID = -1;
        lastPlaced = false;
        isInGrp = false;

    }




//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    // Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                // Calls BoardGeneratorScript and NodeScript 
                                
    public void PlaceBlackSheepMethod()
    {
        // Check if the left mouse button was clicked
        BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gen_Scr.UpdateBoardNodeValues();
        Brd_Gen_Scr.UpdateBoardDisplay();

        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }
    

    // 09/05/2024 - Method Commented out to user Mouse1 for testing
    public void PlaceWhiteSheepMethod()
    {
        WhiteSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gen_Scr.UpdateBoardNodeValues();
        Brd_Gen_Scr.UpdateBoardDisplay();
        
        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);

    }


    public void PlaceEmptySheepMethod()
    {
        EmptySheepSetter();
    }


}
}