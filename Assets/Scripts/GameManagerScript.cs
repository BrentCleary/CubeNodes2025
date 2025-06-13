using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
	public static GameManagerScript instance;
	//* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------

	[Header("Scripts")]
	public BoardGenerator BrdGntScr;
	public GroupScript NDGrpScr;
	public TargetNode TrgNDScr;
	public PlayerScript Plyr_Scr;
	public GameRecorderScript Game_Rec_Scr;

	//* ---------------------------------------- OBJECT REFERENCES ----------------------------------------
	[Header("Object References")]
	public GameObject NDArr;
	public GameObject gameRecorderObj;
	public GameObject pauseMenu;
	public List<NodeScript> NDScrList;

	[Header("Current Board State Vals")]
	public GameState currentState;
	public List<int> crntBoardState;
	public int crntShpVal;
	public bool isKo = false;

	[Header("Last Board State Vals")]
	public List<int> prevBoardState;
	public int prevShpVal;

	[Header("Game Recorder")]
	public List<List<int>> Brd_State_Record;


	void Awake()
	{
		NDArr = GameObject.Find("gNodeArray");
		gameRecorderObj = GameObject.Find("GameRecorderObj");
		BrdGntScr = NDArr.GetComponent<BoardGenerator>();
		NDGrpScr = NDArr.GetComponent<GroupScript>();
		Plyr_Scr = gameObject.GetComponent<PlayerScript>();
		Game_Rec_Scr = gameRecorderObj.GetComponent<GameRecorderScript>();

		pauseMenu = GameObject.Find("PauseMenu");
	}

	// Start is called before the first frame update
	void Start()
	{
		// Board Generation on Start
		BrdGntScr.CreateBoard();
		BrdGntScr.UpdateBoardNodeValues();
		BrdGntScr.UpdateBoardDisplay();

		NDScrList = BrdGntScr.NDScrList;
		crntBoardState = BrdGntScr.Create_ShpValMap();
		prevBoardState = crntBoardState.ToList();

		// Create Players
		Plyr_Scr.CreatePlayer();
		Plyr_Scr.CreatePlayer();

		pauseMenu.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		if (NDScrList.Count < 1 && BrdGntScr.NDScrList != null)
		{
			NDScrList = BrdGntScr.NDScrList;
		}

	}

	public void SetGameState(GameState newState)
	{
		GameState currentState = newState;

		switch (currentState)
		{
			case GameState.TurnPlayer1:
				SceneManager.LoadScene("9x9BoardScene");
				break;
		}
	}


	public void PlaceBlackSheepMethod_GM(int ND_ID)
	{
		NodeScript crnt_ND_Scr = NDGrpScr.GetNodeScriptByID(NDScrList, ND_ID);
		int shpVal = crnt_ND_Scr.shpValList[1];                                                            // Set blackSheepVal

		bool isPlaceAble = NDGrpScr.CheckPlaceble(ND_ID, shpVal);
		Update_Ko_Status(ND_ID, shpVal);

		if (isPlaceAble && isKo == false)                                                                      // Update Played Node and Board Value State
		{
			crnt_ND_Scr.BlackSheepSetter();                                                                  // Set Node to BlacksheepVal

			BrdGntScr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes
			NDGrpScr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
			NDGrpScr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

			BrdGntScr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
			NDGrpScr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

			BrdGntScr.UpdateBoardDisplay();                                                             // Update Board Display

			prevBoardState = crntBoardState.ToList();
			LogListValues<int>(prevBoardState, "prevBoardState GM");

			crntBoardState = BrdGntScr.Create_ShpValMap();
			LogListValues<int>(crntBoardState, "crntBoardState GM");

			prevShpVal = prevBoardState[ND_ID];
			crntShpVal = crntBoardState[ND_ID];

			Game_Rec_Scr.RecordGameBoardState(crntBoardState);
		}
	}


	public void PlaceWhiteSheepMethod_GM(int ND_ID)
	{
		NodeScript crnt_ND_Scr = NDGrpScr.GetNodeScriptByID(NDScrList, ND_ID);
		int shpVal = crnt_ND_Scr.shpValList[2];                                                             // Set whiteSheepVal

		bool isPlaceAble = NDGrpScr.CheckPlaceble(ND_ID, shpVal);
		Update_Ko_Status(ND_ID, shpVal);

		if (isPlaceAble && !isKo)
		{
			// Update Played Node and Board Value State
			crnt_ND_Scr.WhiteSheepSetter();                                                                  // Set Node to whiteSheepVal
			BrdGntScr.UpdateBoardNodeValues();                                                          // Update Value of All BoardNodes

			NDGrpScr.CreateGroup_Method(ND_ID);                                                       // Create New Group for Placed Sheep
			NDGrpScr.UpdateGroups_Method();                                                           // Update All Groups and Delete Zero Val Groups 

			BrdGntScr.UpdateBoardNodeValues();                                                          // Update All NodeValues after Group Deletions
			NDGrpScr.UpdateGroups_Method();                                                           // Update Groups after Node Value Updates

			BrdGntScr.UpdateBoardDisplay();                                                             // Update Board Display

			prevBoardState = crntBoardState.ToList();
			LogListValues<int>(prevBoardState, "prevBoardState GM");

			crntBoardState = BrdGntScr.Create_ShpValMap();
			LogListValues<int>(crntBoardState, "crntBoardState GM");

			prevShpVal = prevBoardState[ND_ID];
			crntShpVal = crntBoardState[ND_ID];

			Game_Rec_Scr.RecordGameBoardState(crntBoardState);

		}
	}


	public void Update_Ko_Status(int ND_ID, int shpVal)
	{
		Debug.Log("NDGrpScr.perform_Ko_Check is " + NDGrpScr.perform_Ko_Check);
		if (NDGrpScr.perform_Ko_Check == true)
		{
			isKo = BrdGntScr.Check_Map_For_Ko(prevBoardState, ND_ID, shpVal);
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


}
