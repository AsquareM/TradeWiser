public class FairyInsington : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(6, TypeOfTile.City);
		SetCity("Fairy Insington", 100, Colour.Cyan, 50, 50);
		SetRent(6, 30, 90, 270, 400, 550);
	}
}
