using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
    public BoardGenerator Brd_Gntr_Script;
    public NodeGroupManager ND_Grp_Mngr_Scrp;
    public TargetNode Trgt_ND_Script;

    //* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
    public GameObject NDArray;

    void Awake()
    {
        NDArray = GameObject.Find("gNodeArray");
        Brd_Gntr_Script = NDArray.GetComponent<BoardGenerator>();



    }

    // Start is called before the first frame update
    void Start()
    {
        // Board Generation on Start
        Brd_Gntr_Script.CreateBoard();
        Brd_Gntr_Script.BoardValueUpdate();
        Brd_Gntr_Script.UpdateBoardDisplay();


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    // Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                // Calls BoardGeneratorScript and NodeScript 
                                
    public void PlaceBlackSheepMethod_GM(GameObject node)
    {
        NodeScript NDScript = node.GetComponent<NodeScript>();

        // Check if the left mouse button was clicked
        NDScript.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal
        Brd_Gntr_Script.BoardValueUpdate();
        Brd_Gntr_Script.UpdateBoardDisplay();

        // ND_Grp_Mngr_Scrp.UpdateGroupsMethod(gameObject);


    }


}
