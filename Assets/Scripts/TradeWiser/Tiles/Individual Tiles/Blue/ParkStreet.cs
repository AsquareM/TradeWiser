public class ParkStreet : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(38, TypeOfTile.City);
        SetCity("Park Street", 350, Colour.Blue, 200, 200);
        SetRent(235, 175, 500, 1100, 1300, 1500);
    }
}
