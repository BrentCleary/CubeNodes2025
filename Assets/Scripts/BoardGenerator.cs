using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class BoardGenerator : MonoBehaviour
{
	//* ---------------------------------------- PROPERTIES ----------------------------------------

	[SerializeField] public List<GameObject> ND_List;
	[SerializeField] public GameObject[,] ND_Arr;
	public Transform ND_ArrTransform;

	[Header("Board Array Settings")] // ! Array Size Controls
	public int arrColLen = 9;                                              // Array Dimensions - Column
	public int arrRowLen = 9;                                                 // Array Dimensions - Row
	public int arrNDCount => arrColLen * arrRowLen;                         // arrColLen * arrRowLen
	public float ND_Spacing = 1.8f;                                             // Space Between Nodes


	public GameObject ND_Prefab;
	public List<int> startNDValMap;
	public List<int> crntNDValMap;

	public List<GameObject> Shp_Grp_List;

	public void CreateBoard()  //? Called in GameManagerScript                      // Instantiates Variables, Calls Methods Below
	{
		ND_Arr = new GameObject[arrColLen, arrRowLen];
		ND_ArrTransform = gameObject.transform;
		ND_List = new List<GameObject>();
		InstantiateNodes();
		SetNodeTransformPosition();

		BuildNodeArray();
		AdjacentSheepNodeMapper(ND_List);
	}
	// *---------------------------------------- CreateBoard Methods ----------------------------------------
	public void InstantiateNodes()                                                  // Instantiates Nodes, Assigns names and values, Adds them to ND_List
	{
		for (int i = 0; i < arrNDCount; i++)
		{
			GameObject node = Instantiate(ND_Prefab, ND_ArrTransform);
			node.name = $"Node ({i})";
			node.GetComponent<NodeScript>().NDID = i;                            //! Sets NDID in NodeScript
			ND_List.Add(node);
		}
	}
	public void SetNodeTransformPosition()                                          // Sets Node transform.position ( uses NodeSpacingValue, array vals  )
	{
		int NDCounter = 0;                                // Increments Node reference in ND_List 
		for (int i = 0; i < arrColLen; i++)
		{         // Assigns positions to each gNode in ND_Arr
			for (int j = 0; j < arrRowLen; j++)
			{
			ND_List[NDCounter].transform.position = new Vector3(i * ND_Spacing, 0, j * ND_Spacing); // Example positioning
				NDCounter++;
			}
		}
	}
	public void BuildNodeArray()                                                    // Generate Array using Length x Row using nodes in ND_List
	{
		int NDCounter = 0;                                                        // Increments Node reference in ND_List 
		for (int i = 0; i < arrColLen; i++)
		{                                // Assigns positions to each gNode in ND_Arr
			for (int j = 0; j < arrRowLen; j++)
			{
				ND_Arr[i, j] = ND_List[NDCounter];                           // Set curent ND_List object to current array position

				// Maps Board Array position for reference
				NodeScript NDScr = ND_List[NDCounter].GetComponent<NodeScript>();
				NDScr.arrPos[0] = i;
				NDScr.arrPos[1] = j;

				// Add Array Position to Node Name
				ND_Arr[i, j].name = ND_Arr[i, j].name + " [" + i + "," + j + "]";
				NDCounter++;                              // Increment ND_List index
			}
		}
	}

	public void AdjacentSheepNodeMapper(List<GameObject> ND_List)                 // Loops over ND_List and assigns adjacent Nodes
	{
		foreach (GameObject crntND in ND_List)
		{
			NodeScript NDScr = crntND.GetComponent<NodeScript>();
			int[] arrPos = NDScr.arrPos;


      // left index is out of range 
			if (arrPos[0] == 0) { NDScr.LND = null; } 
      // left index is a node
			else { 
				NDScr.LND = ND_Arr[arrPos[0] - 1, arrPos[1]].gameObject;
				NDScr.LNDScr = ND_Arr[arrPos[0] - 1, arrPos[1]].GetComponent<NodeScript>();
			}
			
      // right index is out of range 
			if (arrPos[0] == arrColLen - 1){ NDScr.RND = null; }
      // right index is a node
			else {      
				NDScr.RND = ND_Arr[arrPos[0] + 1, arrPos[1]].gameObject;
				NDScr.RNDScr = ND_Arr[arrPos[0] + 1, arrPos[1]].GetComponent<NodeScript>();
			}

      // bottom index is out of range
			if (arrPos[1] == 0) { NDScr.BND = null; } 
      // bottom index is a node
			else { 
				NDScr.BND = ND_Arr[arrPos[0], arrPos[1] - 1].gameObject;
				NDScr.BNDScr = ND_Arr[arrPos[0], arrPos[1] - 1].GetComponent<NodeScript>();
			}

      // top index is out of range 
			if (arrPos[1] == arrRowLen - 1) { NDScr.TND = null; }
      // top index is a node
			else { 
				NDScr.TND = ND_Arr[arrPos[0], arrPos[1] + 1].gameObject;
				NDScr.TNDScr = ND_Arr[arrPos[0], arrPos[1] + 1].GetComponent<NodeScript>();
			}

		}
	}

  



	public void UpdateBoardNodeValues()  //? Called GameManagerScript               // Calls Functions Step1, Step2, Step3 
	{
		List<int> NDValueMap = Create_NDValueMap_Step1();
		List<int> NDValueMapUpdated = Set_NDValueMap_Step2(NDValueMap);                 // Update NDValueMap based on position and board state
		Update_NDValues_Step3(NDValueMapUpdated);
	}
	// *---------------------------------------- Node Value Update Methods ----------------------------------
	public List<int> Create_NDValueMap_Step1()                                      // Displays Array based on nodeValues
	{

		List<int> NDValueMap = new List<int>();                                         // List to hold update values for arrayNodes

		foreach (GameObject crntND in ND_List)                                         // Loops over all nodes in ND_List    
		{
			NodeScript NDScr = crntND.GetComponent<NodeScript>();                    // Set liberty value based on masterNode

			if (NDScr.shpVal == NDScr.shpValList[0])                           // No Sheep
			{
				NDValueMap.Add(NDScr.NDValList[4]);                                // Assigns max NodeValue to each position
				NDScr.libVal = NDScr.libValList[1];
			}
			else                                                                        // Sheep placed
			{
				NDValueMap.Add(NDScr.NDValList[0]);                                // Assigns min NodeValue to each position
				NDScr.libVal = NDScr.libValList[0];
			}
		}

		return NDValueMap;
	}
	public List<int> Set_NDValueMap_Step2(List<int> NDValueMap)                     // Maps nodeValues for updating in Step 3
	{
		List<int> newValueMap = NDValueMap;

		int crntNDval = 0;

		// Map node values to NDValueMap based on current board state
		for (int i = 0; i < arrColLen; i++)
		{         // Assigns positions to each gNode in ND_Arr
			for (int j = 0; j < arrRowLen; j++)
			{
				// Left Index Check
				if (j == 0)
				{      // Check left index is not out of range 
					newValueMap[crntNDval] -= 1;
				}
				if (j != 0)
				{      // Check left position
					if (ND_Arr[i, j - 1].GetComponent<NodeScript>().libVal == 0)
					{     // Check left crntNDval is not null
						newValueMap[crntNDval] -= 1;
					}
				}

				// Right Index Check
				if (j == arrRowLen - 1)
				{      // Check right crntNDval is not out of range 
					newValueMap[crntNDval] -= 1;
				}
				if (j != arrRowLen - 1)
				{       // Check right position
					if (ND_Arr[i, j + 1].GetComponent<NodeScript>().libVal == 0)
					{     // Check right crntNDval is not null
						newValueMap[crntNDval] -= 1;
					}
				}

				// Top Index Check
				if (i == 0)
				{      // Check top crntNDval is not out of range 
					newValueMap[crntNDval] -= 1;
				}
				if (i != 0)
				{      //  Check top position
					if (ND_Arr[i - 1, j].GetComponent<NodeScript>().libVal == 0)
					{
						newValueMap[crntNDval] -= 1;
					}
				}

				// Bottom Index Check
				if (i == arrColLen - 1)
				{      // Check bottom crntNDval is not out of range 
					newValueMap[crntNDval] -= 1;
				}
				if (i != arrColLen - 1)
				{      // Check bottom position 
					if (ND_Arr[i + 1, j].GetComponent<NodeScript>().libVal == 0)
					{
						newValueMap[crntNDval] -= 1;
					}
				}

				crntNDval += 1;
			}
		}

		return newValueMap;
	}
	public void Update_NDValues_Step3(List<int> NDValueMap)                         // Sets Node Display values to Map Values
	{
		int arrayIndex = 0;

		// Map NDValueMap values to NDArray
		for (int i = 0; i < arrColLen; i++)         // Assigns positions to each gNode in ND_Arr
		{
			for (int j = 0; j < arrRowLen; j++)
			{
				GameObject crntND = ND_Arr[i, j];
				NodeScript crntNDScr = crntND.GetComponent<NodeScript>();

				// Debug.Log("NDmapValue" + arrayIndex + " is " + NDValueMap[arrayIndex]);

				if (NDValueMap[arrayIndex] < 0)
				{
					crntNDScr.NDVal = 0;
				}
				else
				{
					crntNDScr.NDVal = NDValueMap[arrayIndex];
				}

				// Debug.Log(crntND.name + " NDValue is " + crntNDScr.NDValue);
				arrayIndex += 1;
			}
		}
	}


	// *---------------------------------------- Node Display Update Method ---------------------------------
	public void UpdateBoardDisplay()  //? Called in GameManagerScript               // Updated Display of Nodes
	{
		foreach (GameObject crntNode in ND_List)
		{
			NodeScript crntNDScrpt = crntNode.GetComponent<NodeScript>();
			crntNDScrpt.UpdateNodeDisplay();
		}
	}


	// *---------------------------------------- Ko Check Methods ---------------------------------
	public List<int> Create_ShpValMap()  //? Called in GameManagerScript               // Displays Array based on nodeValues
	{
		List<int> ShpValMap = new List<int>();                                         // List to hold update values for arrayNodes

		foreach (GameObject crntND in ND_List)                                         // Loops over all nodes in ND_List    
		{
			NodeScript NDScr = crntND.GetComponent<NodeScript>();                    // Set liberty value based on masterNode
			ShpValMap.Add(NDScr.shpVal);
		}

		return ShpValMap;
	}


	public bool Check_Map_For_Ko(List<int> prevShpValMap, int ND_ID, int ShpVal)        // Map of board state before last move, ND_ID and Value
	{
		bool isKo = false;                                                              // Ko is initially set false

		// Debug.Log("prevShpValMap[" + ND_ID + "] : " + prevShpValMap[ND_ID] );

		List<int> newShpValMap = prevShpValMap.ToList();                                         // Create a new copy of ShpValMap for updating and comparing
		newShpValMap[ND_ID] = ShpVal;                                                   // Change List val to ShpVal at ND_ID index (index list matches ND_ID)
																																										// Debug.Log("newShpValMap[" + ND_ID + "] : " + newShpValMap[ND_ID] );

		bool sequenceCheck = newShpValMap.SequenceEqual(prevShpValMap);
		Debug.Log("sequenceCheck: " + sequenceCheck);

		List<int> differingIndices = FindDifferences(newShpValMap, prevShpValMap);
		if (differingIndices.Count > 0)
		{
			Debug.Log("Sequences differ at indices: " + string.Join(", ", differingIndices));
		}
		else
		{
			Debug.Log("Sequences are identical.");
		}

		if (sequenceCheck)                                               // Compare to prev ShpValMap for Board state
		{
			isKo = true;                                                                // If they match, the move will violate KO rules
			Debug.Log("** KO is True **");
			LogListValues<int>(prevShpValMap, "prevShpValMap");
			LogListValues<int>(newShpValMap, "newShpValMap");
		}
		else
		{
			isKo = false;
		}

		return isKo;                                                                    // Return the Ko bool Value
	}
	// DEBUG METHOD FOR KO CHECK METHODS
	List<int> FindDifferences(List<int> newShpValMap, List<int> prevShpValMap)
	{
		List<int> differences = new List<int>();

		for (int i = 0; i < newShpValMap.Count; i++)
		{
			if (newShpValMap[i] != prevShpValMap[i])
			{
				differences.Add(i); // Record the index where they differ
			}
		}

		return differences;

	}


	void LogListValues<T>(List<T> list, string listName)
	{
		string values = string.Join(", ", list);
		Debug.Log($"{listName} values: [{values}]");
	}



	/* Depreciated Methods - Commented out for reference
	// GNODE LIST MAPPER
	// public List<int> AdjacentNodeMapIndexer(List<int> NDValueMap)
	// {
	//     // Counter to increment through index in NDValueMap
	//     int index = 0;     

	//     // Map node values to NDValueMap based on current board state
	//     foreach(GameObject node in ND_List)
	//     {
	//         NodeScript nScript = node.GetComponent<NodeScript>();
	//         List<NodeScript> adjList = nScript.adjNDScrList;

	//         foreach(NodeScript adjScript in adjList)
	//         {
	//             if(adjScript == null || adjScript.libertyVal == 0)
	//             {
	//                 NDValueMap[index] -= 1;
	//                 Debug.Log("Adj Script " + adjScript.name + " is null at " + node.name);
	//             }
	//         }

	//         index += 1;
	//     }   

	//     return NDValueMap;
	// }




	// --------------------------------------------- // gNodeValue Updater Part 3 ---------------------------------------------
																								//* Calls SetGrassTileDisplay
	// public void NodeValueListUpdater(List<int> NDValueMap)
	// {
	//     int arrayIndex = 0;

	//     // Map NDValueMap values to NDArray
	//     foreach(GameObject node in ND_List)
	//     {         

	//         NodeScript crntNDScr = node.GetComponent<NodeScript>();

	//         if(NDValueMap[arrayIndex] < 0) {
	//             crntNDScr.NDValue = 0;
	//         }
	//         else {
	//             crntNDScr.NDValue = NDValueMap[arrayIndex];
	//         }

	//         node.GetComponent<NodeScript>().SetGrassTileDisplay();
	//         Debug.Log(node.GetComponent<NodeScript>().name + "'s NDValue is " + node.GetComponent<NodeScript>().NDValue);

	//         arrayIndex += 1;

	//     }

	//     // Debug.Log("ND_Arr Update Complete");
	// }

	*/

}
