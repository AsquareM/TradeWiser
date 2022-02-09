using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
	//// PRIVATE ---------------------------------------------------------------------------
	private static Player playerWhosTurn;

	private static int noOfPlayers;
	private static int startingAmount;
	private static int currentDiceRoll;

	// Context State Object, stores reference of current State
	public static GameBaseState currentState;

	// String version of currentState that can show up on Inspector
	public string currentStateIndicator;

	[SerializeField]
	private string playerWhoseTurn;

	[SerializeField]
	private Players playerNumberWhoseTurn;
	// --------------------------------------------------------------------------------------


	//// PUBLIC -----------------------------------------------------------------------------
	public static GameController instance;

	public enum Players { NoOne, Player1, Player2, Player3, Player4 };

	// The object of type Player 
	public static Player PlayerWhosTurn 
	{ 
		get => playerWhosTurn;
		set => playerWhosTurn = value;
	}

	// The enum member of type Players Enum
	public static Players WhosTurn;

	// Declaring FSM States
	public static DiceToBeRolledState DiceToBeRolled = new DiceToBeRolledState();
	public static DiceRollingState DiceRolling = new DiceRollingState();
	public static TokenMovingState TokenMoving = new TokenMovingState();
	public static PropertyBuyingState PropertyBuying = new PropertyBuyingState();
	public static WinnerDeclaredState WinnerDeclared = new WinnerDeclaredState();

	public static List<Player> ListOfPlayers;

	public static List<GameObject> Waypoints;

	private static Dictionary<Player, int> scoreBoard;

	// Random Number Generator for the Dice
	public static System.Random randomNumberGenerator;

	// Stores the sum of current Dice Roll, and how many dice have been rolled out of the two
	public static int CurrentDiceRoll 
	{ 
		get => currentDiceRoll; 
		set => currentDiceRoll = value;
	}

	public static int NoOfPlayers
	{
		get => noOfPlayers;
		set => noOfPlayers = value;
	}
	public static Player winner = null;
	// --------------------------------------------------------------------------------------

	#region Methods
	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	private void Start()
	{
		// Intialise objects to instances
		Waypoints = GameObject.FindGameObjectsWithTag("Waypoints").ToList();
		Waypoints.Add(Waypoints[0]);
		currentState = DiceToBeRolled;

		// Initialise variables
		noOfPlayers = PlayerPrefs.GetInt("NoOfPlayers", 2);
		startingAmount = 1500;
		currentDiceRoll = 0;
		ListOfPlayers = new List<Player>();
		scoreBoard = new Dictionary<Player, int>();
		WhosTurn = Players.Player1;

		// Initialise the Random Number Generator using a Single Number Generator in GameController
		// instead of one per dice allows both Dice to have different numbers, instead of both being
		// set to one as would happen if each Dice had its own RNG object
		randomNumberGenerator = new System.Random();

		for (var currentPlayer = Players.Player1; currentPlayer <= (Players)noOfPlayers; currentPlayer++)
		{
			// Typecast from PlayerEnum to int
			int playerNumber = (int)currentPlayer;
			Debug.Log("Setting info for Player" + playerNumber);

			// Creating Player GameObject and initialising its info
			GameObject player = GameObject.Find("Player" + playerNumber);
			if (player)
				Debug.Log(player.name + " latched onto.");

			// Set all the properties of current Player object
			Player playerScript = player.GetComponent<Player>();
			playerScript.SetInfo(
				currentPlayer,
				PlayerPrefs.GetString("Player" + playerNumber),
				UIManager.SetSprite(PlayerPrefs.GetString("Player" + playerNumber + "_Token")),
				startingAmount,
				PlayerPrefs.GetInt("Player" + playerNumber + "_isAI", 0));
			ListOfPlayers.Add(playerScript);

			UIManager.SetPlayerUI(player, playerNumber - 1, ListOfPlayers);
		}

		if (noOfPlayers != 4)
			for (int i = noOfPlayers; i < 4; i++)
				Destroy(GameObject.Find("Player" + (i + 1)));

		playerWhosTurn = ListOfPlayers[0];
		playerWhoseTurn = ListOfPlayers[0].GetName();
		playerNumberWhoseTurn = ListOfPlayers[0].GetPlayerNumber();

		// Set context of current state of FSM
		TransitionToState(DiceToBeRolled);
	}
	
	// Update is called before every new Frame
	private void Update()
	{
		currentState.Update();
	}

	public static void TransitionToState(GameBaseState newState)
	{
		currentState = newState;
		currentState.EnterState();
		EventBroker.NotifyAIOfNewState(newState.GetState());

		instance.currentStateIndicator = currentState.GetType().ToString();
		Debug.Log("Current State: " + instance.currentStateIndicator);
	}

	public static void UpdatePlayerWhoseTurn()
	{
		instance.playerWhoseTurn = playerWhosTurn.GetName();
		instance.playerNumberWhoseTurn = WhosTurn;
	}

	public static void RemovePlayer(Player player, int reason)
	{
		scoreBoard.Add(player, reason);
		
		if ((int)player.GetPlayerNumber() != NoOfPlayers)
			for (int playerNumber = (int)player.GetPlayerNumber(); playerNumber < NoOfPlayers; playerNumber++)
			{
				ListOfPlayers[playerNumber].SetPlayerNumber(ListOfPlayers[playerNumber - 1].GetPlayerNumber());
			}

		NoOfPlayers--;
		ListOfPlayers.Remove(player);
		player.gameObject.SetActive(false);
		UIManager.Instance.playerPanels[(int)player.GetPlayerNumber() - 1].SetActive(false);

		if (noOfPlayers == 1)
		{
			winner = ListOfPlayers[0];
			UIManager.Instance.ScoreboardTextNames.text += winner.GetName() + ": " + "\n";
			UIManager.Instance.ScoreboardTextWealths.text += winner.GetFinalWealth() + "\n";
			UIManager.Instance.GameEndReasonText.text = "All other players forfeited or went bankrupt!";
			UIManager.Instance.GameEndOptionsPanel.SetActive(false);
			UIManager.Instance.TurnOptionsPanel.SetActive(false);
			UIManager.Instance.GameEndScreen.SetActive(true);

			foreach (var play_er in scoreBoard.Keys)
			{
				UIManager.Instance.ScoreboardTextNames.text += play_er.GetName() + ": " + "\n";
				_ = (reason == 0) 
					? UIManager.Instance.ScoreboardTextWealths.text += 0 + " (Bankrupt)\n" 
					: UIManager.Instance.ScoreboardTextWealths.text += 0 + " (Gave Up)\n";
			}

			TransitionToState(WinnerDeclared);
		}
	}

	public static void ForfeitGame()
	{
		var SortedListOfPlayers = new List<Player>(ListOfPlayers);

		foreach (var player in SortedListOfPlayers)
		{
			scoreBoard.Add(player, player.GetFinalWealth());
		}

		// Sort Scoreboard Dictionary by Value
		SortedListOfPlayers.Sort((a, b) => a.GetFinalWealth().CompareTo(b.GetFinalWealth()));

		foreach (var player in SortedListOfPlayers)
		{
			scoreBoard.TryGetValue(player, out int wealth);
			UIManager.Instance.ScoreboardTextNames.text += player.GetName() + ": " + "\n";
			UIManager.Instance.ScoreboardTextWealths.text += wealth + "\n";
		}

		if (SortedListOfPlayers[0].GetFinalWealth() != SortedListOfPlayers[1].GetFinalWealth())
			winner = SortedListOfPlayers[0];
		else
			winner = null;

		TransitionToState(WinnerDeclared);
	}
	#endregion

} // GameController