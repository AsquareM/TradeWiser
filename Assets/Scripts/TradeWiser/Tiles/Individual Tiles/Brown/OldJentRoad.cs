using System.Collections.Generic;

public class OldJentRoad : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(1, TypeOfTile.City);
		SetCity("Old Kent Road", 60, Colour.Brown, 50, 50);
		SetRent(2, 10, 30, 90, 160, 250);
	}

}
