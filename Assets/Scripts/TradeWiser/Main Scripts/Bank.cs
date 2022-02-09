using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
	private static int paydayAmount;
	public static Bank Instance = null;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		paydayAmount = 200;
	}

	public static void AddMoney(Player player, int moneyToAdd)
	{
		player.SetMoney(player.GetMoney() + moneyToAdd);
		UIManager.SetPlayerMoney(player);
	}

	public static void DeductMoney(Player player, int moneyToDeduct)
	{
		int playerWealth = player.GetMoney();

		if (playerWealth > moneyToDeduct)
		{
			player.SetMoney(playerWealth - moneyToDeduct);
			UIManager.SetPlayerMoney(player);
		}
		else
			EventBroker.InvokePlayerBankrupt(player);
	}

	public static void GetPayday(Player player)
	{
		UIManager.SetSecondaryText(player.GetName() + " has gained ₹200 for reaching Start!");
		player.SetMoney(player.GetMoney() + paydayAmount);
		UIManager.SetPlayerMoney(player);
	}

	public static void FinePlayer(Player player)
	{
		UIManager.SetSecondaryText(player.GetName() + " has been jailed and fined for ₹50.");
		DeductMoney(player, 50);
		UIManager.SetPlayerMoney(player);
	}

	public static void BuyProperty(Player player, Property property)
	{
		if (player.GetMoney() >= property.Cost)
		{
			player.SetMoney(player.GetMoney() - property.BuyProperty(player));
			player.Properties.Add(property);
			UIManager.SetPlayerMoney(player);

			if (property.TileType == Tile.TypeOfTile.Station)
				player.StationsOwned++;

			if (property.TileType == Tile.TypeOfTile.Utility)
				player.UtilOwned++;

			if (player.PropertiesOfColourGroupOwned.TryGetValue(property.ColourGroup, out int val))
			{
				player.PropertiesOfColourGroupOwned.Remove(property.ColourGroup);
				player.PropertiesOfColourGroupOwned.Add(property.ColourGroup, val + 1);
			}
			else
				player.PropertiesOfColourGroupOwned.Add(property.ColourGroup, 1);

			Debug.Log(player.GetName() + " has bought the Property \"" + property.Name + "\"");
			// Repopulate onboard property list for current player
			RePopulateOnBoardPropertyList(player);
		}
	}

	public static void SellProperty(Player player, Property property)
	{
		player.SetMoney(player.GetMoney() + property.SellProperty());
		player.Properties.Remove(property);
		UIManager.SetPlayerMoney(player);

		if (property.TileType == Tile.TypeOfTile.Station)
			player.StationsOwned--;

		if (property.TileType == Tile.TypeOfTile.Utility)
			player.UtilOwned--;

		if (player.PropertiesOfColourGroupOwned.TryGetValue(property.ColourGroup, out int val))
		{
			player.PropertiesOfColourGroupOwned.Remove(property.ColourGroup);
			player.PropertiesOfColourGroupOwned.Add(property.ColourGroup, val - 1);
		}

		Debug.Log(player.GetName() + " has sold the Property \"" + property.Name + "\"");
		// Repopulate onboard property list for current player
		RePopulateOnBoardPropertyList(player);
	}

	private static void RePopulateOnBoardPropertyList(Player player)
	{
		player.Properties.Sort();

		foreach (Transform child in UIManager.Instance.playerPropertyGrids[(int)player.GetPlayerNumber() - 1].transform)
			Destroy(child.gameObject);

		int count = 1;
		foreach (var prop in player.Properties)
		{
			GameObject deedImage = new GameObject("TitleDeed", typeof(CanvasRenderer), typeof(Image));
			deedImage.GetComponent<Image>().sprite = prop.Deed;
			deedImage.transform.SetParent(UIManager.Instance.playerPropertyGrids[(int)player.GetPlayerNumber() - 1].transform, false);

			// If there are less than 10 tiles in the Panel, then add another, otherwise don't add, as it will overflow
			if (count == 10)
				break;
		}
	}

}
