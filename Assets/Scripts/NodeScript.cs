using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeScript : MonoBehaviour
{

	//* ---------------------------------------- (this) PROPERTIES ----------------------------------------
	public int NDID;   // Set on Initialization in BoardGenerator
	public int NDVal;
	public int prvNDVal;
	public int shpVal;
	public int libVal;
	public int grpID = -1;
	public bool hasGrp;
	public bool canPlace;
	public bool lastPlaced;                                                            //? the most recently placed Node 

	public List<int> NDValList  = new List<int> { 0, 1, 2, 3, 4 };                    // Node Values when not occupied
	public List<int> shpValList = new List<int> { 0, 1, 2 };                          // { emptySpace, shpBlack, shpWhite }
	public List<int> libValList = new List<int> { 0, 1 };                             //  LibertyValue{ 1 , 0 }
	public List<bool> canPlaceList = new List<bool> { false, true };                  // is node canPlace for current player
	public List<GameObject> shpTileList = new List<GameObject> { };                   // { emptySpace, shpBlack, shpWhite }
	public List<GameObject> tileList    = new List<GameObject> { };

	

  //* ---------------------------------------- TRANSFORM PRESETS ----------------------------------------
  TransformData TData0 = new TransformData {
    pos = new Vector3(0, 0.5f, 0), 
    rtn = Quaternion.identity,	
    scl = new Vector3(1, 1, 1)
  };
	TransformData TData1 = new TransformData {
    pos = new Vector3(0, 1.5f, 0), 
    rtn = Quaternion.identity, 
    scl = new Vector3(1, 1, 1)
  };
	TransformData TData2 = new TransformData {
    pos = new Vector3(0, 2f,   0), 
    rtn = Quaternion.identity,	
    scl = new Vector3(1, 2, 1)
  };
  TransformData TData3 = new TransformData {
    pos = new Vector3(0, 2.5f, 0), 
    rtn = Quaternion.identity,
    scl = new Vector3(1, 3, 1)
  };
  TransformData TData4 = new TransformData {
    pos = new Vector3(0, 3f,   0), 
    rtn = Quaternion.identity,	
    scl = new Vector3(1, 4, 1)
  };
	
  public List<TransformData> transformPresets;


  //* ---------------------------------------- MESHRENDERER PRESETS ----------------------------------------
	public List<Material> tileNormalMatList = new List<Material>();
	public List<Material> tileHoverMatList  = new List<Material>();
	
	public TileHighlight tileHLScript = GameObject.Find("Tile (1)").GetComponent<TileHighlight>();


	//* ---------------------------------------- NODE ARRAY PROPERTIES ----------------------------------------
	public GameObject NDArray;
	public int[] arrPos = new int[2];
	public float duration = .25f;

	//* ---------------------------------------- GROUP PARAMETERS ----------------------------------------
	// Adjacent NodeScripts
	public NodeScript LNDScr;                                                         // left
	public NodeScript RNDScr;                                                         // right
	public NodeScript TNDScr;                                                         // top
	public NodeScript BNDScr;                                                         // bottom
	public List<NodeScript> adjNDScrList;


	//* ---------------------------------------- SCRIPT REFERENCES ----------------------------------------
	[SerializeField] GameManager GM;


	//* ---------------------------------------- SHEEP OBJ REFERENCES ----------------------------------------
	private GameObject whtShp;
	private GameObject blkShp;

	public void Awake()
	{
		// Initialize sheep references first
		blkShp = transform.Find("BlackSheep").gameObject;
		whtShp = transform.Find("WhiteSheep").gameObject;

		// Initialize GM References
		NDArray = GameObject.Find("nodeArray");
		GM      = GameObject.Find("GameManagerObj").GetComponent<GameManager>();

		// Initialize Lists
		adjNDScrList = new List<NodeScript>() { LNDScr, RNDScr, BNDScr, TNDScr };
    transformPresets = new List<TransformData>() { TData0, TData1, TData2, TData3, TData4 };

	}


	//* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
	//* Sets Initial Node Values to Default on Creation 
	void Start()
	{
		NDVal  = NDValList[4];
		shpVal = shpValList[0];
		libVal = 1;
		canPlace = true;

		//// Get reference to Node Array and scripts
		NDArray = GameObject.Find("nodeArray");
		GM      = GameObject.Find("GameManagerObj").GetComponent<GameManager>();

		//// Initialize sheep references first
		// blkShp = transform.Find("BlackSheep").gameObject;
		// whtShp = transform.Find("WhiteSheep").gameObject;

		//// Initialize Lists
		adjNDScrList = new List<NodeScript>() { LNDScr, RNDScr, BNDScr, TNDScr };
    transformPresets = new List<TransformData>() { TData0, TData1, TData2, TData3, TData4 };

	}

	// Update is called once per frame
	void Update() { }


	//* ---------------------------------------- NODE DISPLAY METHODS ----------------------------------------
	public void UpdateNodeDisplay()
	{                                             // Updates Node Display 
		SetSheepDisplay();
		SetTileDisplay();
	
	}

	public void SetSheepDisplay()
	{
		for (int i = shpTileList.Count - 1; i >= 0; i--) {
			shpTileList[i].SetActive(false);                                          // Sets all SheepTiles to inactive
		}
		shpTileList[shpVal].SetActive(true);                                        // Set Current SheepTile active
	}

	public void SetTileDisplay()
	{
      //! Commented out for testing. Working version of SetTileDisplay
      // for (int i = 0; i < tileList.Count; i++)
      // {
      // 	tileList[i].SetActive(false);                                             // Sets all tiles inactive
      // }
      // tileList[NDVal].SetActive(true);                                            // Set current NDVal tile active
    
    // for (int i = 0; i < tileList.Count; i++)
		// {
		// 	tileList[i].SetActive(false);                                             // Sets all tiles inactive
		// }
		// tileList[1].SetActive(true);                                                // Set current NDVal tile active

		// transformPresets[NDVal].ApplyTo(tileList[1].transform);
    

			Debug.Log("prvNDVal = " + prvNDVal + ": NDVal = " + NDVal);
			ChangeNodeColor(prvNDVal, NDVal);
			ChangeNodeDisplaySize(prvNDVal, NDVal);

	}


	// *---------------------------------------- SHEEP SETTER  METHODS ----------------------------------------
	#region Sheet Setter Methods

	public void BlackSheepSetter()                                                // Sets node to Black Sheep Object
	{
		shpVal = shpValList[1];                                                     // Set shp value to blackSheep index
		libVal = libValList[0];                                                     // Set libVal to 0
		NDVal = NDValList[0];                                                     // ND_Val is 0
		blkShp.SetActive(true);
		whtShp.SetActive(false);
		hasGrp = true;
		canPlace = false;
		lastPlaced = true;
	}

	public void WhiteSheepSetter()                                          // Sets node to Black Sheep Object
	{
		shpVal = shpValList[2];                                         // Set shp value to whiteSheep index
		libVal = libValList[0];                                     // Set libVal to 0
		NDVal = NDValList[0];                                           // ND_Val is 0
		blkShp.SetActive(false);
		whtShp.SetActive(true);
		hasGrp = true;
		canPlace = false;
		lastPlaced = true;
	}

	public void EmptySheepSetter()                                          // Sets node to Empty Sheep Object
	{
		shpVal = shpValList[0];                                         // Set shp value to emptySheep index
		libVal = libValList[1];                                     // Set libVal to 1
		NDVal = NDValList[4];                                           // ND_Val is reset to 4
		blkShp.SetActive(false);
		whtShp.SetActive(false);
		grpID = -1;
		hasGrp = false;
		canPlace = true;
		lastPlaced = false;
	}

	#endregion

	public void ChangeNodeColor(int startNDVal, int endNDVal){
		tileHLScript.normalMaterial 	 = tileNormalMatList[endNDVal];
		tileHLScript.highlightMaterial = tileHoverMatList[endNDVal];
	}



	//* ---------------------------------------- TILE SHRINK VALUES ----------------------------------------

	public void ChangeNodeDisplaySize(int startNDVal, int endNDVal)
	{
		StopAllCoroutines();

		StartCoroutine(LerpToPreset(tileList[1].transform, startNDVal, endNDVal, duration));

	}

  public IEnumerator LerpToPreset(Transform target, int startIdx, int endIdx, float duration)
  {
    TransformData start = transformPresets[startIdx];
    TransformData end   = transformPresets[endIdx];

    float  elapsed = 0f;
    while (elapsed < duration)
    {
      float t = elapsed / duration;
      TransformData lerped = TransformData.Lerp(start, end, t);
      lerped.ApplyTo(target);
      elapsed += Time.deltaTime;
      yield return null;
    }

    // Ensure exact final state
    end.ApplyTo(target);
  }



  [System.Serializable]
  public class TransformData
  {
    public Vector3    pos;
    public Quaternion rtn;
    public Vector3    scl;

    public static TransformData Lerp(TransformData start, TransformData end, float t)
    {
      return new TransformData
      {
        pos = Vector3.Lerp   (start.pos, end.pos, t),
        rtn = Quaternion.Lerp(start.rtn, end.rtn, t),
        scl = Vector3.Lerp   (start.scl, end.scl, t)
      };
    }

    public void ApplyTo(Transform target)
    {
      target.localPosition = pos;
      target.localRotation = rtn;
      target.localScale    = scl;
    }
  }


}


	// Tile Position Settings
	/*
  Tile 0
  UnityEditor.TransformWorldPlacementJSON:{"position":{"x":0.0,"y":0.125,"z":0.0},"rotation":{"x":0.0,"y":0.0,"z":0.0,"w":1.0},"scale":{"x":1.0,"y":1.0,"z":1.0}}

  Tile 1
  UnityEditor.TransformWorldPlacementJSON:{"position":{"x":0.0,"y":0.375,"z":0.0},"rotation":{"x":0.0,"y":0.0,"z":0.0,"w":1.0},"scale":{"x":1.0,"y":1.0,"z":1.0}}

  Tile 2
  UnityEditor.TransformWorldPlacementJSON:{"position":{"x":0.0,"y":0.5,"z":0.0},"rotation":{"x":0.0,"y":0.0,"z":0.0,"w":1.0},"scale":{"x":1.0,"y":2.0,"z":1.0}}
  
  Tile 3
  UnityEditor.TransformWorldPlacementJSON:{"position":{"x":0.0,"y":0.625,"z":0.0},"rotation":{"x":0.0,"y":0.0,"z":0.0,"w":1.0},"scale":{"x":1.0,"y":3.0,"z":1.0}}

  Tile 4
  UnityEditor.TransformWorldPlacementJSON:{"position":{"x":0.0,"y":0.75,"z":0.0},"rotation":{"x":0.0,"y":0.0,"z":0.0,"w":1.0},"scale":{"x":1.0,"y":4.0,"z":1.0}}
  Color


  */

