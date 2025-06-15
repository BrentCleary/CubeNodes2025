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
	public GameObject NDArr;

	public Material selectionMaterial;
	private List<Renderer> tileRendererList;
	public List<Material> tileMaterialList;

	private NodeScript parentNDScript;
	private BoardScript BrdScr;
	private GroupScript GrpScr;
	private GameManager GameManager;

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
		NDArr = parentND.transform.parent.gameObject;
		BrdScr = NDArr.GetComponent<BoardScript>();

		// get GroupManagerScriptScript
		GrpScr = NDArr.GetComponent<GroupScript>();

		// get GameManager
		GameManager = GameObject.Find("GameManagerObj").GetComponent<GameManager>();


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
		if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse0)) {
			int NDID = parentNDScript.NDID;
			GameManager.PlaceBlackSheepMethod_GM(NDID);
			nodeSelected = false;
			// Debug.Log("PlaceBlackSheep_OnClick");
		}
	}

	// 09/05/2024 - Method Commented out to user Mouse1 for testing
	public void PlaceWhiteSheep_OnClick_GM() {
		// Check if the left mouse button was clicked
		if (nodeSelected && parentNDScript.canPlace == true && Input.GetKeyDown(KeyCode.Mouse1)) {
			int NDID = parentNDScript.NDID;
			GameManager.PlaceWhiteSheepMethod_GM(NDID);
			nodeSelected = false;
			// Debug.Log("PlaceBlackSheep_OnClick");
		}
	}

	// Debug method for removing stones
	public void PlaceEmptySheep_OnClick() {
		// Check if the middle mouse button was clicked
		if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse2))
		{
			parentNDScript.PlaceEmptySheepMethod();
		}
	}


}