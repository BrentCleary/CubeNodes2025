public enum GameState
{
	MainMenu,           // Landing Page
	TurnPlayer1,        // AI Asks a question
	TurnPlayer2,        // AI or Player do their guess Or Final Guess
	Paused,             // First Scene and Screen
	GameComplete,       // Game has eneded (Loss or Win)
	Credits             // Manual Player Marking state
}