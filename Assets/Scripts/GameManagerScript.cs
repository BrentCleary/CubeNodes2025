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

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    public GameObject NDArray;
    public List<GameObject> gNodeList;
    public List<int> crntBoardState;
    int crntShpVal;

    public List<int> prevBoardState;
    int prevShpVal;


    void Awake()
    {
        NDArray = GameObject.Find("gNodeArray");
        brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<GroupManagerScript>();

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

        foreach(int val in prevBoardState)
        {
            Debug.Log(val);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gNodeList.Count < 1 && brd_Gntr_Script.gNodeList != null)
        {
            gNodeList = brd_Gntr_Script.gNodeList;
        }


    }


    public void PlaceBlackSheepMethod_GM(int nodeID)
    {
        Debug.Log("BlackSheep GM Response");
        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, nodeID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int ND_ID = crntNDScript.nodeID;
        int shpVal = crntNDScript.sheepValList[1];                                                            // Set blackSheepVal
    
        bool isPlaceAble = ND_Grp_Mngr_Scrp.Check_IsPlaceAble(ND_ID, shpVal);
        bool isKo = brd_Gntr_Script.Check_Map_For_Ko(prevBoardState, ND_ID, shpVal);

        Debug.Log("isKo: " + isKo);

        if(isPlaceAble && isKo == false)
        {
            // Update Played Node and Board Value State
            crntNDScript.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
            brd_Gntr_Script.UpdateBoardNodeValues();

            // Update Node Groups and NodeStates
            ND_Grp_Mngr_Scrp.CreateGroup_Method(ND_ID);
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();

            // Update Node Values, Board Display, and GrpLiberties
            brd_Gntr_Script.UpdateBoardNodeValues();
            brd_Gntr_Script.UpdateBoardDisplay();
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();

            // ND_Grp_Mngr_Scrp.CalculateGrpLiberties();
            
            //KO CHECK METHODS
            prevBoardState = crntBoardState.ToList();
            crntBoardState = brd_Gntr_Script.Create_ShpValMap();
        }

        prevShpVal = prevBoardState[ND_ID];
        crntShpVal = crntBoardState[ND_ID];

        Debug.Log("prevShpVal: " + prevShpVal);
        Debug.Log("crntShpVal: " + crntShpVal);

    }


    public void PlaceWhiteSheepMethod_GM(int nodeID)
    {
        Debug.Log("WhiteSheep GM Response");
        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, nodeID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int ND_ID = crntNDScript.nodeID;
        int shpVal = crntNDScript.sheepValList[2];                                                             // Set whiteSheepVal
    
        bool isPlaceAble = ND_Grp_Mngr_Scrp.Check_IsPlaceAble(ND_ID, shpVal);
        bool isKo = brd_Gntr_Script.Check_Map_For_Ko(prevBoardState, ND_ID, shpVal);

        Debug.Log("isKo: " + isKo);

        if(isPlaceAble && !isKo)
        {
            // Update Played Node and Board Value State
            crntNDScript.WhiteSheepSetter();                                                                  // Set Node to whiteSheepVal
            brd_Gntr_Script.UpdateBoardNodeValues();

            // Update Node Groups and NodeStates
            ND_Grp_Mngr_Scrp.CreateGroup_Method(ND_ID);
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();

            // Update Node Values, Board Display, and GrpLiberties
            brd_Gntr_Script.UpdateBoardNodeValues();
            brd_Gntr_Script.UpdateBoardDisplay();
            ND_Grp_Mngr_Scrp.UpdateGroups_Method();
            
            // ND_Grp_Mngr_Scrp.CalculateGrpLiberties();

            //KO CHECK METHODS
            prevBoardState = crntBoardState.ToList();
            crntBoardState = brd_Gntr_Script.Create_ShpValMap();
        }

        prevShpVal = prevBoardState[ND_ID];
        crntShpVal = crntBoardState[ND_ID];

        Debug.Log("prevShpVal: " + prevShpVal);
        Debug.Log("crntShpVal: " + crntShpVal);

    }


}
