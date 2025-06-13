using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
	static int numberOfPlayers = 1;


	[System.Serializable]
	public class Player
	{
		public int playerID;
		public string Name { get; set; }
		public int PlayerShpVal { get; set; } // "Black" or "White"
		public int playerNumber;
		public int CapturedStones { get; private set; }
		public bool isTurn;

		// Constructor
		public Player()
		{
			playerNumber = numberOfPlayers;
			numberOfPlayers++;
			CapturedStones = 0;
			isTurn = false;
		}

		public void CaptureStones(int count)
		{
			CapturedStones += count;
		}

	}

	// Serialized list to track players
	public List<Player> playerList = new List<Player>();

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CreatePlayer()
	{
		Player newPlayer = new Player();

		newPlayer.Name = "Player " + newPlayer.playerNumber;

		if (newPlayer.playerNumber % 2 == 0)
		{
			newPlayer.PlayerShpVal = 2;                           // playerNumber odd = shpVal 1, sheep Value is White (2)
		}
		else
		{
			newPlayer.PlayerShpVal = 1;
		}

		playerList.Add(newPlayer);
	}

}
