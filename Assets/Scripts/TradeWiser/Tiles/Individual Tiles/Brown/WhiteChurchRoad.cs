public class WhiteChurchRoad : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(3, TypeOfTile.City);
		SetCity("White Church Road", 60, Colour.Brown, 50, 50);
		SetRent(4, 20, 60, 180, 320, 450);
	} 
}
