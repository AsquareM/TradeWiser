public class PropertyBuyingState : GameBaseState
{
	private bool changeTurn;
	
	public PropertyBuyingState()
	{
		EventBroker.YieldPlayerTurn += EventBroker_YieldPlayerTurn;
	}

	public override States GetState()
	{
		return States.PropertyBuying;
	}

	public override void EnterState() 
	{
		if (GameController.PlayerWhosTurn.isAI)
			return;

		UIManager.EnableTurnOptions();
	}

	public override void Update()
	{
		if (changeTurn)
		{
			changeTurn = false;
			ChangeTurnToNextPlayer();
		}
	}

	private void EventBroker_YieldPlayerTurn()
	{
		changeTurn = true;
	}

	public void ChangeTurnToNextPlayer()
	{
		// Who's turn next?
		if (GameController.WhosTurn == (GameController.Players)GameController.NoOfPlayers)
			GameController.WhosTurn = GameController.Players.Player1;
		else
			GameController.WhosTurn++;

		GameController.PlayerWhosTurn = GameController.ListOfPlayers[(int)GameController.WhosTurn - 1];
		GameController.UpdatePlayerWhoseTurn();
		EventBroker.InvokeDiceCanBeRolled();
	}
}
