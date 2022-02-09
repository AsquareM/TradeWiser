using UnityEngine;

public class DiceRollingState : GameBaseState
{
	// Stores how many of the two Dice have rolled out a number
	public int diceRolledCount;

	// Stores sum of the numbers on both dice for the
	// current roll of the dice
	public int currentDiceRoll;

	// Indicates that both dices have rolled and reported
	// their face-up number
	private bool diceRollDone;

	public DiceRollingState()
	{
		// Subscribe to DiceRolled Event
		EventBroker.DiceRolled += CalculateDiceNumber;
	}

	public override void EnterState()
	{
		UIManager.SetGameStateText(GameController.PlayerWhosTurn.GetName() + " rolling the dice...");
	}

	public override States GetState()
	{
		return States.DiceRolling;
	}

	public override void Update()
	{
		if (diceRollDone)
		{
			Debug.Log("Both dice rolled a sum of " + currentDiceRoll);

			// Deliver current Dice roll sum to GameController
			GameController.CurrentDiceRoll = currentDiceRoll;

			//Reset Local variables
			diceRollDone = false;
			currentDiceRoll = 0;
			diceRolledCount = 0;

			// instruct gameController to transition to Token Moving State
			GameController.TransitionToState(GameController.TokenMoving);
		}
	}

	private void CalculateDiceNumber(int finalSide)
	{
		currentDiceRoll += finalSide;
		diceRolledCount++;
		Debug.Log("2. DiceRollingState: Received " + finalSide + " from a Die");

		if (diceRolledCount == 2)
		{
			diceRollDone = true;
			Debug.Log("3. Both Dice Rolled, " + diceRolledCount);
		}
	}

}
