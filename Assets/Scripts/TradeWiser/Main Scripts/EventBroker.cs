using System;
using System.Collections;
using UnityEngine;

public class EventBroker : MonoBehaviour
{
	#region Event Declarations
	public static EventBroker instance = null;

	public static event Action YieldPlayerTurn;

	public static event Action DiceCanBeRolled;	// New turn has begin, enable tapping on the Dice for roll
	public static event Action DiceTapped;      // One of the Dice have been tapped on, initiate roll

	// Declare a delegate to handle the DiceRolled Event and then create an event for the same
	public delegate void DiceRollNotifier(int finalSide);
	public static event DiceRollNotifier DiceRolled;

	// Declare a delegate to notify any AI Agents that a new State has occured.
	public delegate void AINewStateNotifier(GameBaseState.States newState);
	public static event AINewStateNotifier AINewStateNotify;

	public static event Action PlayerTokenReached;
	#endregion

	private void Awake()
	{
		instance = this;
	}

	#region Methods to Invoke Events

	/// <summary>
	/// Publisher: UIManager,
	/// Subscriber: PropertyBuyingState
	/// </summary>
	public static void InvokeYieldPlayerTurn()
	{
		YieldPlayerTurn?.Invoke();
	}

	/// <summary>
	/// Indicates that the current Turn has ended, and hence switches PlayerWhosTurn to Next Player
	/// and notifies the Dice that they can be rolled again.
	/// Publisher: PropertyBuyingState,
	/// Subscriber: Dice
	/// </summary>
	public static void InvokeDiceCanBeRolled()
	{
		UIManager.SetGameStateText("Handing the turn over to the next player");
		instance.StartCoroutine(nameof(WaitBeforeChangingTurn));
	}

	// waits 2 seconds before invoking DiceCanBeRolled
	private IEnumerator WaitBeforeChangingTurn()
	{
		if (GameController.PlayerWhosTurn.isAI)
			yield return new WaitForSeconds(3f);
		else
			yield return new WaitForSeconds(2f);

		// Resetting Dice Roll to Zero for Next Turn
		GameController.CurrentDiceRoll = 0;

		// Change FSM State to DiceToBeRolled
		GameController.TransitionToState(GameController.DiceToBeRolled);

		// Invoke event to notify that dice that they can be tapped on
		DiceCanBeRolled?.Invoke();
	}

	/// <summary>
	/// Notifies that the Dice have been tapped on, and to transition to DiceRollingState.
	/// Publisher: Dice,
	/// Subscriber: DiceToBeRolledState
	/// </summary>
	public static void InvokeDiceTapped()
	{
		DiceTapped?.Invoke();
	}

	/// <summary>
	/// Notifies the DiceRollingState of the FSM of the number on top of dice.
	/// Publisher: Dice,
	/// Subscriber: DiceRollingState
	/// </summary>
	/// <param name="finalSide"></param>
	public static void InvokeDiceRollNotifier(int finalSide)
	{
		Debug.Log("DiceRoll Event Invoked");
		DiceRolled?.Invoke(finalSide);
	}

	/// <summary>
	/// Notifies PropertBuyingState that Player Token has reached destination.
	/// Publisher: Player,
	/// Subscriber: PropertyBuyingState
	/// </summary>
	public static void InvokePlayerTokenReached()
	{
		PlayerTokenReached?.Invoke();
	}

	/// <summary>
	/// Publisher: TokenMoving,
	/// Subscriber: Player
	/// </summary>
	public static void InvokeNewTileEntered(Player player)
	{
		player.NewTileEntered();
	}

	/// <summary>
	/// Invoked Usually by the Bank to indicate that a player is Bankrupt.
	/// Can be invoked by AIAgent to tell the sane to attached Player.
	/// </summary>
	/// <param name="player"></param>
	public static void InvokePlayerBankrupt(Player player)
	{
		player.DeclareBankruptcy();
	}

	/// <summary>
	/// Indicates to any present AIs that a change in state has occured
	/// Publisher: GameController
	/// Subscriber: AIAgent
	/// </summary>
	/// <param name="newState"></param>
	public static void NotifyAIOfNewState(GameBaseState.States newState)
	{
		AINewStateNotify?.Invoke(newState);
	}
	#endregion
}
