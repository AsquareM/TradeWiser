public class BowlingLane : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(16, TypeOfTile.City);
		SetCity("Bowling Lane", 180, Colour.Orange, 100, 100);
		SetRent(16, 80, 220, 600, 800, 1000);
	}

}
