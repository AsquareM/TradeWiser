using UnityEngine;

public class Station : Property
{
	public void SetStation(string name, int cost)
	{
		Name = name;
		Cost = cost;
		ColourGroup = Colour.Station;
	}

	public override int GetRent()
	{
		if (PlayerOwner)
		{
			int stationsOwned = PlayerOwner.StationsOwned;

			return stationsOwned switch
			{
				2 => 50,
				3 => 100,
				4 => 200,
				_ => 25,
			};
		}
		else
			throw new MissingReferenceException("No Owner Found");
	}
}