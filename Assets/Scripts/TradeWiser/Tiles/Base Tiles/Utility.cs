public class Utility : Property
{
	public void SetUtility(string name, int cost)
	{
		Name = name;
		Cost = cost;
		ColourGroup = Colour.Utility;
	}

	public override int GetRent()
	{
		if (PlayerOwner.UtilOwned == 2)
			return 10 * GameController.CurrentDiceRoll;
		else
			return 4 * GameController.CurrentDiceRoll;
	}
}