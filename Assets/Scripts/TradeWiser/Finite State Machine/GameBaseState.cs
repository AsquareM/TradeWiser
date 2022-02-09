using UnityEngine;

public abstract class GameBaseState
{
	public enum States { DiceToBeRolled, DiceRolling, PropertyBuying, TokenMoving, WinnerDeclared }

	public abstract States GetState();
	public abstract void EnterState();
	public abstract void Update();
}
