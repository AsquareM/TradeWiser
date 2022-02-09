public class FlockStreet : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(23, TypeOfTile.City);
		SetCity("Flock Street", 220, Colour.Red, 150, 150);
		SetRent(18, 90, 250, 700, 875, 1050);
	}
}
