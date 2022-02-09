public class FrenchStation : Station
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(25, TypeOfTile.Station);
		SetStation("French Station", 200);
	}
}
