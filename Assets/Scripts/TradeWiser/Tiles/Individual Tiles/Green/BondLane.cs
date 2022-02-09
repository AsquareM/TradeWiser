public class BondLane : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(34, TypeOfTile.City);
		SetCity("Bond Lane", 320, Colour.Green, 200, 200);
		SetRent(28, 150, 450, 1000, 1200, 1400);
	}
}
