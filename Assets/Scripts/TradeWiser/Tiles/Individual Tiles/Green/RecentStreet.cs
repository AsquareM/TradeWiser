public class RecentStreet : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(31, TypeOfTile.City);
        SetCity("Recent Street", 300, Colour.Green, 200, 200);
        SetRent(26, 130, 390, 900, 1100, 1275);
    }

}
