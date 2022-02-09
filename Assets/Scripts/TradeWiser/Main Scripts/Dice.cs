using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
	// Array of dice sides sprites to load from Resources folder
	private Sprite[] diceSides;

	// Reference to the other dice
	[SerializeField]
	private GameObject otherdie;

	// Reference to sprite renderer to change sprites
	private SpriteRenderer rend;

	// To check if we can roll the Dice
	public bool canRollDice;

	// Use this for initialization
	private void Start()
	{
		// Assign Renderer component
		rend = GetComponent<SpriteRenderer>();

		// Set reference to instance of other die via code
		GameObject[] dice = GameObject.FindGameObjectsWithTag("Dice");
		otherdie = (dice[0] == gameObject) ? dice[1] : dice[0];

		// Subscribe to the DiceCanBeRolled event
		EventBroker.DiceCanBeRolled += DiceCanBeRolled;

		// Load dice sides sprites to array from DiceSides subfolder of Resources folder
		diceSides = Resources.LoadAll<Sprite>("DiceSides/");

		// Set this to true in the Start
		canRollDice = true;
	}

	// For the other tapped Dice to notify this twin die to roll 
	public void OtherDiceTapped()
	{
		// Cannot roll dice until next player's turn
		canRollDice = false;
		StartCoroutine(nameof(RollTheDice));
	}

	// Subscriber to DiceCanBeRolled Notifier
	private void DiceCanBeRolled()
	{
		canRollDice = true;
	}

	// If you left click/tap over the dice then RollTheDice coroutine is started
	private void OnMouseUpAsButton()
	{
		if (canRollDice && !GameController.PlayerWhosTurn.isAI)
		{
			// Cannot roll dice until next player's turn
			canRollDice = false;

			// Notify subscribers that a dice roll has been
			// triggered by tapping on the dicc
			EventBroker.InvokeDiceTapped();

			// Notify the twin die to roll
			otherdie.GetComponent<Dice>().OtherDiceTapped();

			// Roll the Dice, Animate and Random Generate Number
			StartCoroutine(nameof(RollTheDice));
		}
	}

	/// <summary>
	/// For AI to Invoke
	/// </summary>
	public void RollTheDiceByAI()
	{
		// Cannot roll dice until next player's turn
		canRollDice = false;

		// Notify subscribers that a dice roll has been
		// triggered by tapping on the dicc
		EventBroker.InvokeDiceTapped();

		// Notify the twin die to roll
		otherdie.GetComponent<Dice>().OtherDiceTapped();

		// Roll the Dice, Animate and Random Generate Number
		StartCoroutine(nameof(RollTheDice));
	}

	// Coroutine that rolls the dice
	private IEnumerator RollTheDice()
	{
		// Variable to contain random dice side number.
		int randomDiceSide = 0;

		// Loop to switch dice sides ramdomly before final side appears. 
		for (int i = 0; i <= 20; i++)
		{
			// Pick up random value from 0 to 5
			randomDiceSide = GameController.randomNumberGenerator.Next(0, 6);

			// Set sprite to upper face of dice from array according to random value
			rend.sprite = diceSides[randomDiceSide];

			// Pause before next iteration
			yield return new WaitForSeconds(0.05f);
		}

		// Final side or value that dice reads in the end of coroutine
		// +1 has been added as correction for 0-based index
		int finalSide = randomDiceSide + 1;

		// Show final dice value in Console
		Debug.Log("Dice: Finalside:" + finalSide);

		// If DiceRolled Event has been subscribed to, Invoke it.
		EventBroker.InvokeDiceRollNotifier(finalSide);
	}
}