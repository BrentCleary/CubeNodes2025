using System.Collections;
using System.Collections.Generic;
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

    [System.Serializable]
    public class BoardState
    {
        public List<int> boardValues;
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
        newBrdState.boardValues = crntBoardState;

        Brd_State_Record.Add(newBrdState);
    }
}
