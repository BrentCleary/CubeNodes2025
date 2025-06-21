using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Group
{
  public int grpID;
  public int GrpLibs;
  public int Grp_ShpVal;
  public List<int> NDIDList;
  
  public static int groupCount = -1;

	private Group() { }  // Private constructor prevents accidental use

	public static Group Create() {

    Group group = new Group {

			grpID = ++groupCount,
			NDIDList = new List<int>()
		};
		
    Debug.Log("Group Created: grpID " + group.grpID);

		return group;
  }

}

