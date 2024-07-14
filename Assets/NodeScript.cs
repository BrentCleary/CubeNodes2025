using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{

    // NODE STATE VARIABLES
    private List<int> nodeValue = new List<int> { 0, 1, 2, 3, 4 };   // Node Values when not occupied
    private List<int> libertyValue = new List<int> { 0, 1 };          //  LibertyValue{ 1 , 0 }
    private List<bool> placeable = new List<bool> { false, true };   // is node placeable for current player
    
    public List<GameObject> GrassTileList = new List<GameObject> {};
    
    private List<string> nodeContains = new List<string> { "empty", "sheepBlack", "sheepWhite" }; // List displaying current Node GameObject
    private List<string> transitionStates = new List<string> { "beingCaptured" };             // States for transition


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
