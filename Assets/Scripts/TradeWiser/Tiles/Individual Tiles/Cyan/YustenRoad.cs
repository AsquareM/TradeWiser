public class YustenRoad : City
{
	// Start is called before the first frame update
	void Start()
    {
        SetTileInfo(8, TypeOfTile.City);
        SetCity("Yusten Road", 100, Colour.Cyan, 50, 50);
        SetRent(6, 30, 90, 270, 400, 550);
    }

}
