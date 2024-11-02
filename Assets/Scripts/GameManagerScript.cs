using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManagerScript : MonoBehaviour
{
    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator Brd_Gntr_Script;
    public NodeGroupManager ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    public GameObject NDArray;
    public List<GameObject> gNodeList;

    void Awake()
    {
        NDArray = GameObject.Find("gNodeArray");
        Brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();
        ND_Grp_Mngr_Scrp = NDArray.GetComponent<NodeGroupManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        // Board Generation on Start
        Brd_Gntr_Script.CreateBoard();
        Brd_Gntr_Script.UpdateBoardNodeValues();
        Brd_Gntr_Script.UpdateBoardDisplay();

        gNodeList = Brd_Gntr_Script.gNodeList;

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlaceBlackSheepMethod_GM(int nodeID)
    {
        Debug.Log("BlackSheep GM Response");
        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, nodeID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int ND_ID = crntNDScript.nodeID;

        // Update Played Node and Board Value State
        crntNDScript.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gntr_Script.UpdateBoardNodeValues();

        // Update Node Groups and NodeStates
        ND_Grp_Mngr_Scrp.UpdateGroups(ND_ID);

        // Update Node Values and Board Display
        Brd_Gntr_Script.UpdateBoardNodeValues();
        Brd_Gntr_Script.UpdateBoardDisplay();

    }


    public void PlaceWhiteSheepMethod_GM(int nodeID)
    {
        Debug.Log("WhiteSheep GM Response");
        
        GameObject node = ND_Grp_Mngr_Scrp.GetNodeWithID(gNodeList, nodeID);
        NodeScript crntNDScript = node.GetComponent<NodeScript>();
        int ND_ID = crntNDScript.nodeID;

        // Update Played Node and Board Value State
        crntNDScript.WhiteSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gntr_Script.UpdateBoardNodeValues();

        // Update Node Groups and NodeStates
        ND_Grp_Mngr_Scrp.UpdateGroups(ND_ID);

        // Update Node Values and Board Display
        Brd_Gntr_Script.UpdateBoardNodeValues();
        Brd_Gntr_Script.UpdateBoardDisplay();

    }


}
