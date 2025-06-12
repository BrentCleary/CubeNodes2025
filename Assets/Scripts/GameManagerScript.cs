using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;
    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    
    [Header("Scripts")]
    public BoardGenerator Brd_Gnt_Scr;
    public GroupManagerScript ND_Grp_Scr;
    public TargetNode Trg_ND_Scr;
    public PlayerScript Plyr_Scr;
    public GameRecorderScript Game_Rec_Scr;

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    [Header("Object References")]
    public GameObject NDArray;
    public GameObject gameRecorderObj;
    public GameObject pauseMenu;
    public List<GameObject> ND_List;
    
    [Header("Current Board State Vals")]
    public GameState currentState;
    public List<int> crntBoardState;
    public int crntShpVal;
    public bool isKo = false;

    [Header("Last Board State Vals")]
    public List<int> prevBoardState;
    public int prevShpVal;

    [Header("Game Recorder")]
    public List<List<int>> Brd_State_Record;


    void Awake()
    {
        NDArray = GameObject.Find("gNodeArray");
        gameRecorderObj = GameObject.Find("GameRecorderObj");
        Brd_Gnt_Scr = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Scr = NDArray.GetComponent<GroupManagerScript>();
        Plyr_Scr = gameObject.GetComponent<PlayerScript>();
        Game_Rec_Scr = gameRecorderObj.GetComponent<GameRecorderScript>();

        pauseMenu = GameObject.Find("PauseMenu");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Board Generation on Start
        Brd_Gnt_Scr.CreateBoard();
        Brd_Gnt_Scr.UpdateBoardNodeValues();
        Brd_Gnt_Scr.UpdateBoardDisplay();

        ND_List = Brd_Gnt_Scr.ND_List;
        crntBoardState = Brd_Gnt_Scr.Create_ShpValMap();
        prevBoardState = crntBoardState.ToList();

        // Create Players
        Plyr_Scr.CreatePlayer();
        Plyr_Scr.CreatePlayer();

        pauseMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ND_List.Count < 1 && Brd_Gnt_Scr.ND_List != null)
        {
            ND_List = Brd_Gnt_Scr.ND_List;
        }

    }

    public void SetGameState(GameState newState)
    {
        GameState currentState = newState;

        switch (currentState)
        {
            case GameState.TurnPlayer1:
                SceneManager.LoadScene("9x9BoardScene");
                break;
        }
    }


    public void PlaceBlackSheepMethod_GM(int ND_ID)
    {        
        GameObject node = ND_Grp_Scr.GetNodeWithID(ND_List, ND_ID);
        NodeScript crnt_ND_Scr = node.GetComponent<NodeScript>();
        int shpVal = crnt_ND_Scr.shpValList[1];                                                            // Set blackSheepVal
    
        bool isPlaceAble = ND_Grp_Scr.Check_IsPlaceAble(ND_ID, shpVal);
        Update_Ko_Status(ND_ID, shpVal);

        if(isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
        {
            crnt_ND_Scr.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
            
            Brd_Gnt_Scr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes
            ND_Grp_Scr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
            ND_Grp_Scr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

            Brd_Gnt_Scr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
            ND_Grp_Scr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

            Brd_Gnt_Scr.UpdateBoardDisplay();                                                             // Update Board Display

            prevBoardState = crntBoardState.ToList();
            LogListValues<int>(prevBoardState, "prevBoardState GM");

            crntBoardState = Brd_Gnt_Scr.Create_ShpValMap();
            LogListValues<int>(crntBoardState, "crntBoardState GM");
            
            prevShpVal = prevBoardState[ND_ID];
            crntShpVal = crntBoardState[ND_ID];

            Game_Rec_Scr.RecordGameBoardState(crntBoardState);
        }
    }


    public void PlaceWhiteSheepMethod_GM(int ND_ID)
    {        
        GameObject node = ND_Grp_Scr.GetNodeWithID(ND_List, ND_ID);
        NodeScript crnt_ND_Scr = node.GetComponent<NodeScript>();
        int shpVal = crnt_ND_Scr.shpValList[2];                                                             // Set whiteSheepVal
    
        bool isPlaceAble = ND_Grp_Scr.Check_IsPlaceAble(ND_ID, shpVal);
        Update_Ko_Status(ND_ID, shpVal);

        if(isPlaceAble && !isKo)
        {
            // Update Played Node and Board Value State
            crnt_ND_Scr.WhiteSheepSetter();                                                                  // Set Node to whiteSheepVal
            Brd_Gnt_Scr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes

            ND_Grp_Scr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
            ND_Grp_Scr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

            Brd_Gnt_Scr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
            ND_Grp_Scr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

            Brd_Gnt_Scr.UpdateBoardDisplay();                                                             // Update Board Display

            prevBoardState = crntBoardState.ToList();
            LogListValues<int>(prevBoardState, "prevBoardState GM");

            crntBoardState = Brd_Gnt_Scr.Create_ShpValMap();
            LogListValues<int>(crntBoardState, "crntBoardState GM");
            
            prevShpVal = prevBoardState[ND_ID];
            crntShpVal = crntBoardState[ND_ID];

            Game_Rec_Scr.RecordGameBoardState(crntBoardState);

        }
    }


    public void Update_Ko_Status(int ND_ID, int shpVal)
    {
        Debug.Log("ND_Grp_Scr.perform_Ko_Check is " +  ND_Grp_Scr.perform_Ko_Check);
        if(ND_Grp_Scr.perform_Ko_Check == true)
        {
            isKo = Brd_Gnt_Scr.Check_Map_For_Ko(prevBoardState, ND_ID, shpVal);
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


}
