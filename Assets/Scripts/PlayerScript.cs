using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int PlayerSheepValue = 1;

    [System.Serializable]
    public class Player
    {
        public string Name { get; set; }
        public int SheepValue { get; set; } // "Black" or "White"
        public int playerNumber = 0;
        public int CapturedStones { get; private set; }
        public bool isTurn;

        // Constructor
        public Player()
        {
            SheepValue = 
            playerNumber++;
            CapturedStones = 0;
            isTurn = false;
        }

        public void CaptureStones(int count)
        {
            CapturedStones += count;
        }

    }

        // Serialized list to track players
    public List<Player> Players = new List<Player>();

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

        if(PlayerSheepValue % 2 == 0)
        {
            newPlayer.SheepValue = 2;
        }
        else
        {
            newPlayer.SheepValue = 1;
        }

        Players.Add(newPlayer);
    }

}
