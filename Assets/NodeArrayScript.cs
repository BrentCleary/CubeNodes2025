using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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


    // Start is called before the first frame update
    void Start()
    {
        gNodeArray = new GameObject[3,3];

        // gNode List and Script Reference
        gNodeList = new List<GameObject> { gNode1, gNode2, gNode3, gNode4, gNode5, gNode6, gNode7, gNode8, gNode9 };
    
        Generate3x3NodeArray();
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }

    public void DebugControls()
    {
        if(Input.GetKeyDown(KeyCode.O)) { ArrayPositionNodeValueSetter(); }

        if(Input.GetKeyDown(KeyCode.P)) { DisplayArray(); }
    }


    public void Generate3x3NodeArray()              // Generate a 3x3 array containing Nodes contained gNodeList
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

    public void DisplayArray()                      // Debugs Array Nodes in Console
    {
        for(int i = 0; i < arrayColumnLength; i ++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j ++)
            {
                Debug.Log(gNodeArray[i,j].name);
            }
        }
    }
    
    
    public void ArrayPositionNodeValueSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.nodeValue = 4;           // Resets nodeValue of all Nodes to 4 before Setting

                // Check left position
                if(j == 0)      // Check left index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(j != 0)
                {
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0)     // Check left index is not null
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                // Check right position
                if(j == arrayRowLength-1)      // Check right index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(j != arrayRowLength-1)
                {
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0)     // Check right index is not null
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                // Check top position
                if(i == 0)      // Check top index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(i != 0)      // Check top index is not null
                {
                    if(gNodeArray[i-1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                // Check bottom position
                if(i == arrayColumnLength-1)      // Check bottom index is not out of range 
                {
                    currentNodeScript.nodeValue -= 1;
                }
                if(i != arrayColumnLength-1)      // Check bottom index is not null 
                {
                    if(gNodeArray[i+1,j].GetComponent<NodeScript>().libertyValue == 0)
                    {
                        currentNodeScript.nodeValue -= 1;
                    }
                }

                currentNode.GetComponent<NodeScript>().settingState = true;
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();

                Debug.Log(currentNode.GetComponent<NodeScript>().name + "'s nodeValue is " + currentNode.GetComponent<NodeScript>().nodeValue);
            }
        }
    }


}
    
