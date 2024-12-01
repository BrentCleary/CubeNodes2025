using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManagerScript : MonoBehaviour
{
    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator brd_Gntr_Script;
    public GroupManagerScript ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;
    public PlayerScript Plyr_Scrp;

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    public GameObject NDArray;
    public List<GameObject> gNodeList;
    public List<int> crntBoardState;
    public int crntShpVal;

    public List<int> prevBoardState;
    public int prevShpVal;
    public bool isKo = false;


    public List<List<int>> BRD_State_Record;

    void Awake()
    {
        NDArray = GameObject.Find("gNodeArray");
        brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<GroupManagerScript>();
        Plyr_Scrp = gameObject.GetComponent<PlayerScript>();

    }

    // Start is called before the first frame update
    void Start()
    {
        // Board Generation on Start
        brd_Gntr_Script.CreateBoard();
        brd_Gntr_Script.UpdateBoardNodeValues();
        brd_Gntr_Script.UpdateBoardDisplay();

        gNodeList = brd_Gntr_Script.gNodeList;
        crntBoardState = brd_Gntr_Script.Create_ShpValMap();
        prevBoardState = crntBoardState.ToList();

        Plyr_Scrp.CreatePlayer();
        Plyr_Scrp.CreatePlayer();

    }

    // Update is called once per frame
    void Update()
    {
        if (gNodeList.Count < 1 && brd_Gntr_Script.gNodeList != null)
        {
            gNodeList = brd_Gntr_Script.gNodeList;
        }

    }


    public void PlaceBlackSheepMethod_GM(int ND_ID)
    {        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, ND_ID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int shpVal = crntNDScript.sheepValList[1];                                                            // Set blackSheepVal
    
        bool isPlaceAble = ND_Grp_Mngr_Scrp.Check_IsPlaceAble(ND_ID, shpVal);
        Update_Ko_Status(ND_ID, shpVal);

        if(isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
        {
            crntNDScript.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
            
            brd_Gntr_Script.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes
            ND_Grp_Mngr_Scrp.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

            brd_Gntr_Script.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

            brd_Gntr_Script.UpdateBoardDisplay();                                                             // Update Board Display

            prevBoardState = crntBoardState.ToList();
            LogListValues<int>(prevBoardState, "prevBoardState GM");

            crntBoardState = brd_Gntr_Script.Create_ShpValMap();
            LogListValues<int>(crntBoardState, "crntBoardState GM");
            
            prevShpVal = prevBoardState[ND_ID];
            crntShpVal = crntBoardState[ND_ID];
        }
    }


    public void PlaceWhiteSheepMethod_GM(int ND_ID)
    {        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, ND_ID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int shpVal = crntNDScript.sheepValList[2];                                                             // Set whiteSheepVal
    
        bool isPlaceAble = ND_Grp_Mngr_Scrp.Check_IsPlaceAble(ND_ID, shpVal);
        Update_Ko_Status(ND_ID, shpVal);

        if(isPlaceAble && !isKo)
        {
            // Update Played Node and Board Value State
            crntNDScript.WhiteSheepSetter();                                                                  // Set Node to whiteSheepVal
            brd_Gntr_Script.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes

            ND_Grp_Mngr_Scrp.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

            brd_Gntr_Script.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

            brd_Gntr_Script.UpdateBoardDisplay();                                                             // Update Board Display

            prevBoardState = crntBoardState.ToList();
            LogListValues<int>(prevBoardState, "prevBoardState GM");

            crntBoardState = brd_Gntr_Script.Create_ShpValMap();
            LogListValues<int>(crntBoardState, "crntBoardState GM");
            
            prevShpVal = prevBoardState[ND_ID];
            crntShpVal = crntBoardState[ND_ID];
        }
    }


    public void Update_Ko_Status(int ND_ID, int shpVal)
    {
        Debug.Log("ND_Grp_Mngr_Scrp.perform_Ko_Check is " +  ND_Grp_Mngr_Scrp.perform_Ko_Check);
        if(ND_Grp_Mngr_Scrp.perform_Ko_Check == true)
        {
            isKo = brd_Gntr_Script.Check_Map_For_Ko(prevBoardState, ND_ID, shpVal);
            Debug.Log("isKo is " + isKo);
        }
        else
        {
            isKo = false;
        }
    }



    void LogListValues<T>(List<T> list, string listName)
    {
        string values = string.Join(", ", list);
        Debug.Log($"{listName} values: [{values}]");
    }


    // Method to Track the Game State after each move.
    // For game reviews, record keeping, and undo's midgame
    // Will need to record
    /*
        BoardSheepValues
        GroupListState
        PlayerStates
            Timer
            Captures
            CurrentPlayerTurn
        
    */
    public void RecordGameBoardState()
    {

    }



}
