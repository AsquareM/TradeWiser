public class Mayfare : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(39, TypeOfTile.City);
		SetCity("Mayfare", 350, Colour.Blue, 200, 200);
		SetRent(50, 200, 600, 1400, 1700, 2000);
	}
}
