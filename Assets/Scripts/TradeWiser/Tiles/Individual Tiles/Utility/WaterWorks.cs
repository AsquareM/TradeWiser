public class WaterWorks : Utility
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(28, TypeOfTile.Utility);
        SetUtility("Water Works", 150);
    }
}
