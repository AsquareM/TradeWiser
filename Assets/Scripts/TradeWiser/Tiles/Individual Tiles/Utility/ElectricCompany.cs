public class ElectricCompany : Utility
{
    // Start is called before the first frame update
    void Start()
    {
        SetTileInfo(12, TypeOfTile.Utility);
        SetUtility("Electric Company", 150);
    }
}
