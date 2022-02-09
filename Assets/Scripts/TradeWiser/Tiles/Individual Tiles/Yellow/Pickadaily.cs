public class Pickadaily : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(29, TypeOfTile.City);
		SetCity("Pickadaily", 280, Colour.Yellow, 150, 150);
		SetRent(22, 120, 360, 850, 1025, 1200);
	}
}
