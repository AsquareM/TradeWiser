using UnityEngine;

public class Tile : MonoBehaviour
{
	public enum TypeOfTile { StartTile, City, Station, Utility, Chance, CommunityChest, GoToJail, Jail, SuperTax, IncomeTax, FreeParking };

	[Header("Tile Properties")]	
	public TypeOfTile TileType;
	public int TileIndex;

	public void SetTileInfo(int index, TypeOfTile tileType)
	{
		TileIndex = index;
		TileType = tileType;
	}

} // Tile Class