using UnityEngine;

public class DiceToBeRolledState : GameBaseState
{
	// Keeps track of if Dice has been Rolled
	public bool diceRollTriggered;

	// Constructor to initialize objects
	public DiceToBeRolledState()
	{
		// Suscribe to the DiceTapped event
		EventBroker.DiceTapped += DiceHasBeenTapped;
	}

	// Executed each time upon entering this state
	public override void EnterState()
	{
		diceRollTriggered = false;
		UIManager.SetGameStateText("Player " + GameController.PlayerWhosTurn.GetName() + "'s turn to roll the dice!");
	}

	public override States GetState()
	{
		return States.DiceToBeRolled;
	}

	// GameController will call this in it's Update
	public override void Update()
	{
		// If Dice Roll has been triggered, transition to Dice Rolling State
		if (diceRollTriggered)
		{
			Debug.Log("DiceToBeRolledState: Transitioning to DiceRolling");
			GameController.TransitionToState(GameController.DiceRolling);
		}
	}

	/// <summary>
	/// Subscriber to the DiceTapped event that allows this state to
	/// know when to transition to Dice Rolling State
	/// </summary>
	private void DiceHasBeenTapped()
	{
		Debug.Log("DiceToBeRolledState: DiceTapped Event has been detected.");
		diceRollTriggered = true;
	}
	
} 
