using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameRecorderScript : MonoBehaviour
{
    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator brd_Gntr_Script;
    public GroupManagerScript ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;
    public PlayerScript Plyr_Scrp;

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    public GameObject NDArray;
    public List<int> crntBoardState;
    public int crntShpVal;

    public List<int> prevBoardState;
    public int prevShpVal;
    public List<int> ND_GrpID_List;
    public static int globalMoveCount = 0;
    public int currentPlayer;

    [System.Serializable]
    public class BoardState
    {
        public string turnNumber;
        public List<int> boardValues;
        public List<int> ND_GrpID_List;


        public BoardState()
        {
            globalMoveCount++;
            turnNumber =  "Turn " + globalMoveCount;
        }
    }
    public List<BoardState> Brd_State_Record;


    // Start is called before the first frame update
    void Start()
    {
        NDArray = GameObject.Find("gNodeArray");
        brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<GroupManagerScript>();
        Plyr_Scrp = gameObject.GetComponent<PlayerScript>();

        Brd_State_Record = new List<BoardState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValidate()
    {
        for (int i = 0; i < ND_GrpID_List.Count; i++)
        {
            Brd_State_Record[i].turnNumber = $"Element {i + 1}";
        }
    }


    /* Method to Track the Game State after each move.
        For game reviews, record keeping, and undo's midgame
        Will need to record
        BoardSheepValues
        GroupListState
        PlayerStates
            Timer
            Captures
            CurrentPlayerTurn
        
    */
    public void RecordGameBoardState(List<int> crntBoardState)
    {
        BoardState newBrdState = new BoardState();
    
        newBrdState.boardValues = crntBoardState;                                           // Assign current List of ND ShpVals
        newBrdState.ND_GrpID_List = ND_Grp_Mngr_Scrp.GetAll_ND_GrpID();                     // Update List of GrpID for all NDs

        Brd_State_Record.Add(newBrdState);
    }
}
