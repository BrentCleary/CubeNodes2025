using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeArrayScript : MonoBehaviour
{
    
    [SerializeField] public GameObject[] gNodeArray;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate3x3NodeArray()
    {
        int arrayColumnLength = 3;
        int arrayRowLength = 3;
        
        for(int i = 0; i < arrayColumnLength; i ++)
        {
            for(int j = 0; j < arrayRowLength; j ++)
            {
                gNodeArray[i,j] =  
            }
        }
    }
}
