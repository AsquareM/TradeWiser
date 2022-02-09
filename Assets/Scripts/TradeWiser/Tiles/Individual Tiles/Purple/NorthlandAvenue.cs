public class NorthlandAvenue : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(14, TypeOfTile.City);
		SetCity("Northland Avenue", 220, Colour.Purple, 100, 100);
		SetRent(12, 60, 180, 500, 700, 900);
	}
}
