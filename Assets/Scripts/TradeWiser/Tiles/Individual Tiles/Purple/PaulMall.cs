public class PaulMall : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(11, TypeOfTile.City);
		SetCity("Paul Mall", 140, Colour.Purple, 100, 100);
		SetRent(10, 50, 150, 450, 625, 750);
	}
}
