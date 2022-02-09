public class OxfortStreet : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(32, TypeOfTile.City);
        SetCity("Oxford Street", 300, Colour.Green, 200, 200);
        SetRent(26, 130, 390, 900, 1100, 1275);
    }
}
