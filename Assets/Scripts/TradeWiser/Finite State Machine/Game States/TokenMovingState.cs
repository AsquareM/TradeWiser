using UnityEngine;

public class TokenMovingState : GameBaseState
{
	private bool playerTokenReached;

	public TokenMovingState()
	{
		EventBroker.PlayerTokenReached += PlayerTokenReached;
		playerTokenReached = false;
	}

	public override void EnterState()
	{
		// Change UI text to Indicate the Dice Roll
		Debug.Log("Sum of both Dice Rolled = " + GameController.CurrentDiceRoll);
		UIManager.SetGameStateText(GameController.PlayerWhosTurn.GetName() + " rolled " + GameController.CurrentDiceRoll);

		// Calculate new tile number for Current Player, following the Dice Roll
		var playerCurrentTile = GameController.PlayerWhosTurn.GetTileNumber();
		var newTileNumber = (playerCurrentTile + GameController.CurrentDiceRoll) % 40;
		GameController.PlayerWhosTurn.SetTileNumber(newTileNumber);

		// Log
		Debug.Log("Taking " + GameController.PlayerWhosTurn.GetName()
			+ " from " + GameController.PlayerWhosTurn.transform.position + " to tile "
			+ newTileNumber + " at " + GameController.Waypoints[newTileNumber].transform.position);
	}

	public override States GetState()
	{
		return States.TokenMoving;
	}

	public override void Update()
	{
		if (playerTokenReached)
		{
			playerTokenReached = false;
			GameController.TransitionToState(GameController.PropertyBuying);
		}
	}


	/// <summary>
	/// Publisher: Player
	/// </summary>
	private void PlayerTokenReached()
	{
		EventBroker.InvokeNewTileEntered(GameController.PlayerWhosTurn); // Subscriber: Player
		playerTokenReached = true;
	}

}
