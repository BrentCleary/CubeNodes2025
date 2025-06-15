using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	// ---------------------------------------- SCRIPT REFERENCES ----------------------------------------

	[Header("Scripts")]
	public BoardScript BrdScr;
	public GroupScript GrpScr;
	public TargetNode TrgNDScr;
	public PlayerScript Plyr_Scr;

	// ---------------------------------------- OBJECT REFERENCES ----------------------------------------
	[Header("Object References")]
	public GameObject NDArr;
	public GameObject gameRecorderObj;
	public GameObject pauseMenu;
	public List<NodeScript> NDScrList;

	[Header("Current Board State Vals")]
	public List<int> nowBrdState;
	public int nowShpVal;
	public bool isKo = false;

	[Header("Last Board State Vals")]
	public List<int> prvBrdState;
	public int prvShpVal;

	[Header("Game Recorder")]
	public List<List<int>> Brd_State_Record;


	[Header("RayCast Props")]
	public Camera mainCamera;
	public LayerMask layerMask;
	public bool rayCastMouseSelect;

	private GameObject _hitObject;
	public GameObject hitObject
	{
		get { return _hitObject; }
		set
		{
			if (_hitObject != value)
			{
				_hitObject = value;
				OnValueChanged();
			}
		}
	}



	void Awake()
	{
		NDArr = GameObject.Find("gNodeArray");
		gameRecorderObj = GameObject.Find("GameRecorderObj");
		BrdScr = NDArr.GetComponent<BoardScript>();
		GrpScr = NDArr.GetComponent<GroupScript>();
		Plyr_Scr = gameObject.GetComponent<PlayerScript>();
		TrgNDScr = gameObject.GetComponent<TargetNode>();

		pauseMenu = GameObject.Find("PauseMenu");

		mainCamera = Camera.main;

	}

	// Start is called before the first frame update
	void Start()
	{
		// Board Generation on Start
		BrdScr.CreateBoard();
		BrdScr.UpdateBoardNodeValues();
		BrdScr.UpdateBoardDisplay();

		NDScrList = BrdScr.NDScrList;
		nowBrdState = BrdScr.Create_ShpValMap();
		prvBrdState = nowBrdState.ToList();

		// Create Players
		Plyr_Scr.CreatePlayer();
		Plyr_Scr.CreatePlayer();

		pauseMenu.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (NDScrList.Count < 1 && BrdScr.NDScrList != null) {
			NDScrList = BrdScr.NDScrList;
		}

		// RayCast Method Calls
		DrawRay();
		hitObject = GetRaycastHitObject();

	}




	public void PlaceBlackSheepMethod_GM(int ND_ID)
	{
		NodeScript crnt_ND_Scr = GrpScr.GetNodeScriptByID(NDScrList, ND_ID);
		int shpVal = crnt_ND_Scr.shpValList[1];                                                            // Set blackSheepVal

		bool isPlaceAble = GrpScr.CheckPlaceble(ND_ID, shpVal);
		Update_Ko_Status(ND_ID, shpVal);

		if (isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
		{
			crnt_ND_Scr.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal

			BrdScr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes
			GrpScr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
			GrpScr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

			BrdScr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
			GrpScr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

			BrdScr.UpdateBoardDisplay();                                                             // Update Board Display

			prvBrdState = nowBrdState.ToList();
			LogListValues<int>(prvBrdState, "prvBrdState GM");

			nowBrdState = BrdScr.Create_ShpValMap();
			LogListValues<int>(nowBrdState, "nowBrdState GM");

			prvShpVal = prvBrdState[ND_ID];
			nowShpVal = nowBrdState[ND_ID];

		}
	}


	public void PlaceWhiteSheepMethod_GM(int ND_ID)
	{
		NodeScript crnt_ND_Scr = GrpScr.GetNodeScriptByID(NDScrList, ND_ID);
		int shpVal = crnt_ND_Scr.shpValList[2];                                                             // Set whiteSheepVal

		bool isPlaceAble = GrpScr.CheckPlaceble(ND_ID, shpVal);
		Update_Ko_Status(ND_ID, shpVal);

		if (isPlaceAble && !isKo)
		{
			// Update Played Node and Board Value State
			crnt_ND_Scr.WhiteSheepSetter();                                                                  // Set Node to whiteSheepVal
			BrdScr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes

			GrpScr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
			GrpScr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

			BrdScr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
			GrpScr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

			BrdScr.UpdateBoardDisplay();                                                             // Update Board Display

			prvBrdState = nowBrdState.ToList();
			LogListValues<int>(prvBrdState, "prvBrdState GM");

			nowBrdState = BrdScr.Create_ShpValMap();
			LogListValues<int>(nowBrdState, "nowBrdState GM");

			prvShpVal = prvBrdState[ND_ID];
			nowShpVal = nowBrdState[ND_ID];

		}
	}


	public void Update_Ko_Status(int ND_ID, int shpVal)
	{
		Debug.Log("GrpScr.perform_Ko_Check is " + GrpScr.perform_Ko_Check);
		if (GrpScr.perform_Ko_Check == true)
		{
			isKo = BrdScr.Check_Map_For_Ko(prvBrdState, ND_ID, shpVal);
			Debug.Log("isKo is " + isKo);
		}
		else
		{
			isKo = false;
		}
	}


	void LogListValues<T>(List<T> list, string listName)
	{
		string values = string.Join(", ", list);
		Debug.Log($"{listName} values: [{values}]");
	}




	// ----------------------------------- RAYCAST METHODS -----------------------------------

	public GameObject GetRaycastHitObject()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1000, layerMask))
		{
			return hit.transform.gameObject; // Return the GameObject that the ray hits
		}

		return null; // Return null if no object is hit
	}

	// DrawRay
public void DrawRay()
{
	if (mainCamera == null)
		return;

	// Get a ray from the mouse position
	Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

	// Draw the ray from the camera toward the mouse direction, 1000 units long
	Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);
}

	public void OnValueChanged()
	{
    if (_hitObject != null)
        Debug.Log("Raycast hit: " + _hitObject.name);
    else
        Debug.Log("Raycast hit: null");
	}







}
