using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
	//* ---------------------------------------- PROPERTIES ----------------------------------------
	public GameObject grassContainer;
	public GameObject parentND;
	public GameObject ND_Array;
	public Camera mainCamera;

	public Material selectionMaterial;
	private List<Renderer> tileRendererList;
	public List<Material> tileMaterialList;

	private NodeScript parentNDScript;
	private BoardGenerator brd_Gntr_Script;
	private GroupScript ND_Grp_Mngr_Scrp;
	private GameManagerScript GameManagerScript;

	public bool nodeSelected = false;



	//* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
	//* Assigns Parent Object/Parent Scripts and GrassTileList 
	//* Calls Placement Method on Nodes 
	// Start is called before the first frame update
	void Start()
	{
		grassContainer = gameObject.transform.parent.gameObject;

		// node reference - parent reference
		parentND = grassContainer.transform.parent.gameObject;
		parentNDScript = parentND.GetComponent<NodeScript>();

		// board array reference - highest parent
		ND_Array = parentND.transform.parent.gameObject;
		brd_Gntr_Script = ND_Array.GetComponent<BoardGenerator>();

		// get GroupManagerScriptScript
		ND_Grp_Mngr_Scrp = ND_Array.GetComponent<GroupScript>();

		// get GameManager
		GameManagerScript = GameObject.Find("GameManagerObj").GetComponent<GameManagerScript>();


		// //* Color Settings (Moved into NodeScript 10/07/24)
		// tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();

		// foreach(Renderer renderer in tileRendererList) {
		//     tileMaterialList.Add(renderer.material);
		// }

	}

	// Update is called once per frame
	void Update()
	{
		PlaceBlackSheep_OnClick_GM();
		PlaceWhiteSheep_OnClick_GM();
	}



	//* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
	//* Highlights/Resets selected Nodes Color by changing GrassTiles materials 

	//* Mouse Enter/Exit Methods
	private void OnMouseEnter()
	{
		nodeSelected = true;
		parentNDScript.SetNodeColor_Selected();
	}

	private void OnMouseExit()
	{
		nodeSelected = false;
		parentNDScript.SetNodeColor_Not_Selected();
	}





	//* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
	//* Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
	//* Calls BoardGeneratorScript and NodeScript 
	public void PlaceBlackSheep_OnClick_GM()
	{
		// Check if the left mouse button was clicked
		if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse0))
		{
			int NDID = parentNDScript.NDID;
			GameManagerScript.PlaceBlackSheepMethod_GM(NDID);
			nodeSelected = false;
			// Debug.Log("PlaceBlackSheep_OnClick");
		}
	}

	// 09/05/2024 - Method Commented out to user Mouse1 for testing
	public void PlaceWhiteSheep_OnClick_GM()
	{
		// Check if the left mouse button was clicked
		if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse1))
		{
			int NDID = parentNDScript.NDID;
			GameManagerScript.PlaceWhiteSheepMethod_GM(NDID);
			nodeSelected = false;
			// Debug.Log("PlaceBlackSheep_OnClick");
		}
	}

	// Debug method for removing stones
	public void PlaceEmptySheep_OnClick()
	{
		// Check if the middle mouse button was clicked
		if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse2))
		{
			parentNDScript.PlaceEmptySheepMethod();
		}
	}



	/* Commented out 10/05/24 - Method placed in NodeScript
	// //* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
	//                         //* Highlights/Resets selected Nodes Color by changing GrassTiles materials 

	// public void AssignSheepToGroups()
	// {

	//     // Create new NodeGroup, assign groupID
	//     parentNodeScript.groupID = ND_Grp_Mngr_Scrp.CreateNewGroup(parentNode);
	//     int parentNodeGroupID = parentNodeScript.groupID;

	//     // Adjacent Node Scripts
	//     NodeScript leftNodeScript = parentNodeScript.leftNodeScript;
	//     NodeScript rightNodeScript = parentNodeScript.rightNodeScript;
	//     NodeScript bottomNodeScript = parentNodeScript.bottomNodeScript;
	//     NodeScript topNodeScript = parentNodeScript.topNodeScript;


	//     //* FIRST CHECK OCCURS ON LEFT NODE
	//     // Left Node Found
	//     if(leftNodeScript != null && leftNodeScript.sheepVal == parentNodeScript.sheepVal)
	//     {
	//         int leftNodeGroupID = leftNodeScript.groupID;

	//         List<GameObject> nodeGroup = ND_Grp_Mngr_Scrp.JoinGroups(parentNodeGroupID, leftNodeGroupID);

	//         foreach(GameObject node in nodeGroup)
	//         {
	//             node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
	//         }

	//         ND_Grp_Mngr_Scrp.DeleteGroup(leftNodeGroupID);
	//     }

	//     // Right Node Found
	//     if(rightNodeScript != null && rightNodeScript.sheepVal == parentNodeScript.sheepVal)
	//     {
	//         int rightNodeGroupID = rightNodeScript.groupID;

	//         if(rightNodeGroupID != parentNodeGroupID)
	//         {
	//             List<GameObject> nodeGroup = ND_Grp_Mngr_Scrp.JoinGroups(parentNodeGroupID, rightNodeGroupID);

	//             foreach(GameObject node in nodeGroup)
	//             {
	//                 Debug.Log("parentNodeID is " + parentNodeGroupID);
	//                 Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//                 node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
	//                 Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//             }

	//             ND_Grp_Mngr_Scrp.DeleteGroup(rightNodeGroupID);

	//             Debug.Log("New Node added to GroupID: " + rightNodeScript.groupID);
	//         }
	//     }

	//     // Bottom Node Found
	//     if(bottomNodeScript != null && bottomNodeScript.sheepVal == parentNodeScript.sheepVal)
	//     {
	//         int bottomNodeGroupID = bottomNodeScript.groupID;

	//         if(bottomNodeGroupID != parentNodeGroupID)
	//         {
	//             List<GameObject> nodeGroup = ND_Grp_Mngr_Scrp.JoinGroups(parentNodeGroupID, bottomNodeGroupID);

	//             foreach(GameObject node in nodeGroup)
	//             {
	//                 Debug.Log("parentNodeID is " + parentNodeGroupID);
	//                 Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//                 node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
	//                 Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//             }

	//             ND_Grp_Mngr_Scrp.DeleteGroup(bottomNodeGroupID);

	//             Debug.Log("New Node added to GroupID: " + bottomNodeScript.groupID);
	//         }
	//     }

	//     // Top Node Found
	//     if(topNodeScript != null && topNodeScript.sheepVal == parentNodeScript.sheepVal)
	//     {
	//         int topNodeGroupID = topNodeScript.groupID;

	//         if(topNodeGroupID != parentNodeGroupID)
	//         {
	//             List<GameObject> nodeGroup = ND_Grp_Mngr_Scrp.JoinGroups(parentNodeGroupID, topNodeGroupID);

	//             foreach(GameObject node in nodeGroup)
	//             {
	//                 Debug.Log("parentNodeID is " + parentNodeGroupID);
	//                 Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//                 node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
	//                 Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//             }

	//             ND_Grp_Mngr_Scrp.DeleteGroup(topNodeGroupID);

	//             Debug.Log("New Node added to GroupID: " + topNodeScript.groupID);
	//         }
	//     }
	// }

	// Possible Simplification of AssignSheep Method to be refactored later
	// public void RefactoredAssignSheepMethod()
	// {
	//     // Adjacent Node Scripts
	//     List<NodeScript> NodeScriptList = new List<NodeScript>();

	//     NodeScript leftNodeScript = parentNodeScript.leftNodeScript;
	//     NodeScript rightNodeScript = parentNodeScript.rightNodeScript;
	//     NodeScript bottomNodeScript = parentNodeScript.bottomNodeScript;
	//     NodeScript topNodeScript = parentNodeScript.topNodeScript;

	//     NodeScriptList.Add(leftNodeScript);
	//     NodeScriptList.Add(rightNodeScript);
	//     NodeScriptList.Add(bottomNodeScript);
	//     NodeScriptList.Add(topNodeScript);

	//     foreach(NodeScript NDScript in NodeScriptList)
	//     {
	//         // Top Node Found
	//         if(NDScript != null && NDScript.sheepVal == parentNodeScript.sheepVal)
	//         {
	//             int nodeGroupID = NDScript.groupID;
	//             int parentNodeGroupID = parentNodeScript.groupID;

	//             List<GameObject> nodeGroup = ND_Grp_Mngr_Scrp.JoinNodeGroups(parentNodeGroupID, nodeGroupID);

	//             foreach(GameObject node in nodeGroup)
	//             {
	//                 Debug.Log("parentNodeID is " + parentNodeGroupID);
	//                 Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//                 node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
	//                 Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
	//             }

	//             ND_Grp_Mngr_Scrp.DeleteNodeGroup(nodeGroupID);

	//             Debug.Log("New Node added to GroupID: " + NDScript.groupID);
	//         }
	//     }
	// }

			// public void PlaceBlackSheep_OnClick()
	// {
	//     // Check if the left mouse button was clicked
	//     if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse0))
	//     {
	//         parentNDScript.PlaceBlackSheepMethod();
	//         nodeSelected = false;
	//         // Debug.Log("PlaceBlackSheep_OnClick");

	//     }
	// }

	// 09/05/2024 - Method Commented out to user Mouse1 for testing
	// public void PlaceWhiteSheep_OnClick()
	// {
	//     // Check if the right mouse button was clicked
	//     if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse1))
	//     {
	//         parentNDScript.PlaceWhiteSheepMethod();
	//         nodeSelected = false;
	//         Debug.Log("PlaceWhiteSheep_OnClick");

	//     }
	// }
	*/


}