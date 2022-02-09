
public class MerryboroLane : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(18, TypeOfTile.City);
		SetCity("Bowling Lane", 180, Colour.Orange, 100, 100);
		SetRent(14, 70, 200, 550, 750, 950);
	}
}
