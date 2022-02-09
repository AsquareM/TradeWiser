
public class WineLane : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(19, TypeOfTile.City);
		SetCity("Wine Lane", 200, Colour.Orange, 100, 100);
		SetRent(16, 80, 220, 600, 800, 1000);
	}
}
