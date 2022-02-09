public class WhiteHallway : City
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(15, TypeOfTile.City);
        SetCity("White Hallway", 140, Colour.Purple, 100, 100);
        SetRent(10, 50, 150, 450, 625, 750);
    }
}
