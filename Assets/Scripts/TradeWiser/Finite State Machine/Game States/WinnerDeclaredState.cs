public class WinnerDeclaredState : GameBaseState
{
	public override void EnterState()
	{
		UIManager.Instance.GameWinnerText.text 
			= (GameController.winner != null)
			? GameController.winner.GetPlayerNumber() + " - " + GameController.winner.GetName() + " has won the game!"
			: "Tie Detected. No Single Winner";
	}

	public override States GetState()
	{
		return States.WinnerDeclared;
	}

	public override void Update() {	}
}
