public class Stand : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(21, TypeOfTile.City);
        SetCity("Stand", 220, Colour.Red, 150, 150);
        SetRent(18, 90, 250, 700, 875, 1050);
    }
}
