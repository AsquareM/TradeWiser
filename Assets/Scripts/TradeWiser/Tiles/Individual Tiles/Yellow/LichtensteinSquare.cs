public class LichtensteinSquare : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(26, TypeOfTile.City);
        SetCity("Lichtenstein Square", 220, Colour.Yellow, 150, 150);
        SetRent(22, 110, 330, 800, 975, 1150);
    }
}
