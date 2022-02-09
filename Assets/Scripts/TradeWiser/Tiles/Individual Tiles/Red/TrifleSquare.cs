public class TrifleSquare : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(24, TypeOfTile.City);
        SetCity("Flock Street", 240, Colour.Red, 150, 150);
        SetRent(20, 100, 300, 750, 925, 1100);
    }
}
