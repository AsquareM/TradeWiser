public class Pentoffville : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(9, TypeOfTile.City);
        SetCity("Pentoffville", 100, Colour.Cyan, 50, 50);
        SetRent(8, 40, 100, 300, 450, 600);
    }

}
