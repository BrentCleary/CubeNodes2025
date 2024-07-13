using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{

    // NODE VARIABLES
    List<int> nodeValue = new List<int> { 0, 1, 2, 3, 4 };   // Node Values when not occupied
    List<bool> placeable = new List<bool> { false, true };   // is node placeable for current player
    List<int> libertyValue = new List<int> { 0, 1 }          //  LibertyValue{ 1 , 0 }
    
    // 
    List<string> nodeContains = new List<string> { empty, sheepBlack, sheepWhite }; // List displaying current Node Occupant
    
    // STATES
    List<string> transitionStates = new List<string> { beingCaptured };             // States for transition


// OBJECT PROPERTIES
// Objects for displaying value
// - List containing game object references to display or toggle based on Node state
// - To be hidden toggle their renders on or off


// SHEEP OBJECT
// Will need an over arching variable to determine behavior based on Black or White







// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
