public class CovenantStreet : City
{
	// Start is called before the first frame update
	void Start()
	{
		SetTileInfo(27, TypeOfTile.City);
		SetCity("Covenant Street", 220, Colour.Yellow, 150, 150);
		SetRent(22, 110, 330, 800, 975, 1150);
	}
}
