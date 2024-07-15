using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeArrayScript : MonoBehaviour
{
    
    [SerializeField] public GameObject[,] gNodeArray;
    int arrayColumnLength = 3;                          // Array Dimensions - Column
    int arrayRowLength = 3;                             // Array Dimensions - Row

    [SerializeField] public List<GameObject> gNodeList;
    public GameObject gNode1;
    public GameObject gNode2; 
    public GameObject gNode3;
    public GameObject gNode4; 
    public GameObject gNode5; 
    public GameObject gNode6; 
    public GameObject gNode7; 
    public GameObject gNode8; 
    public GameObject gNode9; 

    public NodeScript gNodeScript1;
    public NodeScript gNodeScript2;
    public NodeScript gNodeScript3;
    public NodeScript gNodeScript4;
    public NodeScript gNodeScript5;
    public NodeScript gNodeScript6;
    public NodeScript gNodeScript7;
    public NodeScript gNodeScript8;
    public NodeScript gNodeScript9;

    // Start is called before the first frame update
    void Start()
    {
        gNodeArray = new GameObject[3,3];

        // gNode List and Script Reference
        gNodeList = new List<GameObject> { gNode1, gNode2, gNode3, gNode4, gNode5, gNode6, gNode7, gNode8, gNode9 };

        gNodeScript1 = gNode1.GetComponent<NodeScript>();
        gNodeScript2 = gNode2.GetComponent<NodeScript>();
        gNodeScript3 = gNode3.GetComponent<NodeScript>();
        gNodeScript4 = gNode4.GetComponent<NodeScript>();
        gNodeScript5 = gNode5.GetComponent<NodeScript>();
        gNodeScript6 = gNode6.GetComponent<NodeScript>();
        gNodeScript7 = gNode7.GetComponent<NodeScript>();
        gNodeScript8 = gNode8.GetComponent<NodeScript>();
        gNodeScript9 = gNode9.GetComponent<NodeScript>();
    
        Generate3x3NodeArray();
    }

    // Update is called once per frame
    void Update()
    {
        DisplayArray();
    }

    public void Generate3x3NodeArray()                      // Generate a 3x3 array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 

        for(int i = 0; i < arrayColumnLength; i ++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j ++)
            {
                gNodeArray[i,j] = gNodeList[nodeCounter];
                nodeCounter = nodeCounter + 1;
            }
        }
    }

    public void DisplayArray()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            for(int i = 0; i < arrayColumnLength; i ++)         // Assigns positions to each gNode in gNodeArray
            {
                for(int j = 0; j < arrayRowLength; j ++)
                {
                    Debug.Log(gNodeArray[i,j].name);
                }
            }
        }
    }
    
}
