using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private static UIManager instance;
	public static UIManager Instance
	{
		get => instance;
		set => instance = value;
	}

	#region Variables
	[SerializeField] private Image changingBG;

	[Header("Turn End UI")]
	public GameObject TurnOptionsPanel;
	public Text MessageNotifierText;
	public GameObject InfoPanel;
	public GameObject BuyOptionsPanel;
	public Button BuyPropertyButton;
	public Text BuyingInfoText;
	public GameObject RentNotifierPanel;
	public Text RentingInfoText;
	public GameObject GameEndOptionsPanel;
	public Image TitleDeed;
	public GameObject PlayerPropertyPanel;
	public GameObject PlayerPropertyHolder;
	public Text PropertyHolderPlayerName;
	public Text PropertyHolderPlayerInfo;
	public GameObject HouseCreationPanel;
	public GameObject HouseCreationTitleDeed;
	public GameObject HouseCreationSellProperty;
	public GameObject HouseCreationBuyHouse;
	public GameObject HouseCreationSellHouse;

	[Header("On Board UI")]
	public Text GameStateText;
	public Text SecondaryText;
	public Text[] PlayerNameTexts;
	public Text[] PlayerMoneyTexts;
	
	public Image[] PlayerSpriteImages;
	public GameObject[] playerPanels;
	public GameObject[] playerPropertyGrids;

	[Header("GameEnd UI")]
	public GameObject GameEndScreen;
	public Text GameEndReasonText;
	public Text GameWinnerText;
	public Text ScoreboardTextNames;
	public Text ScoreboardTextWealths;

	// Reference to all Sprite Assets in the Resources folder 
	private static Sprite Car;
	private static Sprite Cat;
	private static Sprite Hat;
	private static Sprite Horse;
	private static Sprite Ship;
	private static Sprite Shoe;

	public static City cityToBuildHouseOn;
	#endregion

	#region Methods

	#region Private Methods
	// Start is called before the first frame update
	private void OnEnable()
	{
		instance = this;

		// Disable extra panels and players
		int noOfPlayers = PlayerPrefs.GetInt("NoOfPlayers", 2);
		if (noOfPlayers != 4)
			for (int i = noOfPlayers; i < 4; i++)
				Destroy(playerPanels[i]);

		// Load Sprites
		Car = Resources.Load<Sprite>("Players/Car");
		Cat = Resources.Load<Sprite>("Players/Cat");
		Hat = Resources.Load<Sprite>("Players/Hat");
		Horse = Resources.Load<Sprite>("Players/Horse");
		Ship = Resources.Load<Sprite>("Players/Ship");
		Shoe = Resources.Load<Sprite>("Players/Shoe");
	}

	// FixedUpdate is called once per 0.02s
	private void FixedUpdate()
	{
		CycleHueSpectrum();
	}

	/// <summary>
	/// Change the background color very very slowly, making it travel
	/// through all the colours of the Hue spectrum
	/// </summary>
	private void CycleHueSpectrum()
	{
		var currentColour = changingBG.color;
		Color.RGBToHSV(currentColour, out float h, out _, out _);
		changingBG.color = Color.HSVToRGB((h + 0.001f) % 1f, 0.4f, 1f);
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Returns a Sprite Resources equivalent of the String Name of the Sprite
	/// </summary>
	/// <param name="spriteName"></param>
	/// <returns> Sprite </returns>
	public static Sprite SetSprite(string spriteName)
	{
		return spriteName switch
		{
			"Car" => Car,
			"Cat" => Cat,
			"Hat" => Hat,
			"Horse" => Horse,
			"Ship" => Ship,
			"Shoe" => Shoe,
			_ => throw new NotImplementedException(spriteName + " is unknown Sprite."),
		};
	}

	public static void SetPlayerUI(GameObject player, int playerNumber, List<Player> listOfPlayers)
	{
		SpriteRenderer playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
		playerSpriteRenderer.sprite = listOfPlayers[playerNumber].GetSprite();

		Debug.Log("UIManager: Current Player = " + playerNumber);
		// Update info on UI Panels
		instance.PlayerNameTexts[playerNumber].text = listOfPlayers[playerNumber].GetName();
		instance.PlayerSpriteImages[playerNumber].sprite = listOfPlayers[playerNumber].GetSprite();
		instance.PlayerMoneyTexts[playerNumber].text	= "₹" + listOfPlayers[playerNumber].GetMoney();
	}

	public static void SetGameStateText(string s)
	{
		instance.GameStateText.text = s;
	}

	public static void SetSecondaryText(string s)
	{
		instance.SecondaryText.text = s;
		instance.StartCoroutine(nameof(SetSecondaryTextBackToBlank));
	}
	IEnumerator SetSecondaryTextBackToBlank()
	{
		yield return new WaitForSeconds(2f);
		instance.SecondaryText.text = "";
	}

	public static void SetPlayerMoney(Player player)
	{
		instance.PlayerMoneyTexts[(int)player.GetPlayerNumber() - 1].text = "₹" + player.GetMoney().ToString();
	}

	public static void EnableTurnOptions()
	{
		instance.TurnOptionsPanel.SetActive(true);
		InputSystem.CannotPan = true;
	}

	public static void EnableBuyOptions(Property property, int playerWealth)
	{
		instance.BuyOptionsPanel.SetActive(true); // Enable BuyOptionsPanel
		instance.TitleDeed.sprite = property.Deed; // Set Current property's Title Deed
		instance.TitleDeed.gameObject.SetActive(true); // Enable the Title Deed
		instance.BuyPropertyButton.gameObject.SetActive(true); // Enable the Buy Button

		// If Player can afford the property, show difference, else display Funds Not Enough
		if (property.Cost <= playerWealth)
			instance.BuyingInfoText.text = "Cost: " + property.Cost
				+ "\nWealth: " + playerWealth + "\nDifference: "
				+ (playerWealth - property.Cost);
		else
		{
			instance.BuyingInfoText.text = "Cost: " + property.Cost + "\nWealth: " + playerWealth + "\nFunds Not Enough!";
			instance.BuyPropertyButton.gameObject.SetActive(false);
		}

	}

	public static void EnableRentOptions(Property property, Player player)
	{
		instance.RentNotifierPanel.SetActive(true); // Enable RentNotifierPanel
		instance.TitleDeed.sprite = property.Deed; // Set Current property's Title Deed
		instance.TitleDeed.gameObject.SetActive(true); //Enable title Deed

		int playerWealth = player.GetMoney();
		int propertyRent = property.GetRent();

		// If Player can afford the rent, show difference, else display Bankrupt
		if (property.GetRent() <= playerWealth)
		{
			instance.RentingInfoText.text = "Rent Paid: " + propertyRent
				+ "\nWealth: " + playerWealth + "\nDifference: "
				+ (playerWealth - propertyRent);

			// Deduct from rentee, give rent to owner
			Bank.DeductMoney(player, propertyRent);
			Bank.AddMoney(property.PlayerOwner, propertyRent);
		}
		else
		{
			instance.RentingInfoText.text = "Funds exhausted, You are bankrupt!";
			instance.StartCoroutine(nameof(NotifyBankruptcy), player);
		}
	}

	private IEnumerator NotifyBankruptcy(Player player)
	{
		yield return new WaitForSeconds(1f);
		EventBroker.InvokePlayerBankrupt(player);
	}

	public static void EnableInfoPanel(Property property)
	{
		instance.InfoPanel.SetActive(true);
		instance.TitleDeed.gameObject.SetActive(true);
		instance.TitleDeed.sprite = property.Deed;
	}

	public void EnablePlayerPropertyPanel(int playerNumber)
	{
		if (InputSystem.CannotPan)
			return;

		InputSystem.CannotPan = true;
		instance.PlayerPropertyPanel.SetActive(true);
		Player player = GameController.ListOfPlayers[playerNumber];
		PropertyHolderPlayerName.text = player.GetName() + " - Properties Owned";
		PropertyHolderPlayerInfo.text = "Cash Wealth: " + player.GetMoney() + "          Cumulative Wealth: " + player.GetFinalWealth();

		player.Properties.Sort();

		foreach (var property in player.Properties)
		{
			GameObject titleDeed = new GameObject("deedImage", typeof(CanvasRenderer), typeof(Image));
			titleDeed.GetComponent<Image>().sprite = property.Deed;
			titleDeed.transform.SetParent(PlayerPropertyHolder.transform, false);
		}
	}

	public void DisablePlayerPropertyPanel()
	{
		instance.PlayerPropertyPanel.SetActive(false);
		InputSystem.CannotPan = false;

		foreach (Transform child in PlayerPropertyHolder.transform)
			Destroy(child.gameObject);
	}

	public static void EnableHouseCreationPanel(Property property)
	{
		if (InputSystem.CannotPan)
			return;

		InputSystem.CannotPan = true;
		instance.HouseCreationPanel.gameObject.SetActive(true);
		instance.HouseCreationTitleDeed.GetComponent<Image>().sprite = property.Deed;

		if (GameController.PlayerWhosTurn == property.PlayerOwner)
		{
			if (property.TileType == Tile.TypeOfTile.City)
			{
				City city = (City)property;
				if (city.noOfHouses == City.Houses.None)
					instance.HouseCreationSellProperty.gameObject.SetActive(true);
				else
					instance.HouseCreationSellProperty.gameObject.SetActive(false);
			}
			else
				instance.HouseCreationSellProperty.gameObject.SetActive(true);
		}

		Property.Colour colourGroup = property.ColourGroup;
		int numberOfPropertiesOfSameColourGroupOwned = 0;
		foreach (var prop in GameController.PlayerWhosTurn.Properties)
			if (prop.ColourGroup == colourGroup)
				numberOfPropertiesOfSameColourGroupOwned++;

		//Enable House Buy Button or not
		if (property.TileType == Tile.TypeOfTile.City && GameController.PlayerWhosTurn == property.PlayerOwner)
		{
			cityToBuildHouseOn = (City)property;
			int totalCostOfBuildingHouse =
				(cityToBuildHouseOn.ColourGroup == Property.Colour.Brown || cityToBuildHouseOn.ColourGroup == Property.Colour.Blue) ?
				cityToBuildHouseOn.CostOfHouse * 2 : cityToBuildHouseOn.CostOfHouse * 3;

			if (GameController.PlayerWhosTurn.GetMoney() >= totalCostOfBuildingHouse && cityToBuildHouseOn.noOfHouses < City.Houses.Hotel)
			{
				if ((colourGroup == Property.Colour.Brown || colourGroup == Property.Colour.Blue)
					&& numberOfPropertiesOfSameColourGroupOwned == 2)
				{
					instance.HouseCreationBuyHouse.gameObject.SetActive(true);
				}
				else if (numberOfPropertiesOfSameColourGroupOwned == 3)
				{
					instance.HouseCreationBuyHouse.gameObject.SetActive(true);
				}
				else
					instance.HouseCreationBuyHouse.gameObject.SetActive(false);
			}
		}

		// Enable sell house button or not
		if (property.TileType == Tile.TypeOfTile.City && GameController.PlayerWhosTurn == property.PlayerOwner)
		{
			cityToBuildHouseOn = (City)property;
			if (cityToBuildHouseOn.noOfHouses > City.Houses.None)
				instance.HouseCreationSellHouse.gameObject.SetActive(true);
			else
				instance.HouseCreationSellHouse.gameObject.SetActive(false);
		}
	}

	public void BuyHouse()
	{
		int totalCostOfBuildingHouse =
			(cityToBuildHouseOn.ColourGroup == Property.Colour.Brown || cityToBuildHouseOn.ColourGroup == Property.Colour.Blue) ?
			cityToBuildHouseOn.CostOfHouse * 2 : cityToBuildHouseOn.CostOfHouse * 3;

		Bank.DeductMoney(GameController.PlayerWhosTurn, totalCostOfBuildingHouse);

		foreach (Property property in GameController.PlayerWhosTurn.Properties)
			if (property.ColourGroup == cityToBuildHouseOn.ColourGroup)
			{
				City city = (City)property;
				if (city.noOfHouses < City.Houses.Four)
					city.houses[(int)city.noOfHouses].SetActive(true);
				else
				{
					city.houses[(int)City.Houses.One - 1].SetActive(false);
					city.houses[(int)City.Houses.Two - 1].SetActive(false);
					city.houses[(int)City.Houses.Three - 1].SetActive(false);
					city.houses[(int)City.Houses.Four - 1].SetActive(false);
					city.houses[(int)City.Houses.Hotel - 1].SetActive(true);
				}

				city.housesBuilt[(int)city.noOfHouses] = true;
				city.noOfHouses++;
				Debug.Log(GameController.PlayerWhosTurn.GetName() + " has created house no. " + city.noOfHouses + " on the City \"" + city.Name + "\"");
			}
		GameController.PlayerWhosTurn.noOfHousesCreated++;

		DisableHouseCreationPanel();
	}

	public void SellHouse()
	{
		if (cityToBuildHouseOn.noOfHouses > City.Houses.None)
		{
			int totalCostOfBuildingHouse =
				(cityToBuildHouseOn.ColourGroup == Property.Colour.Brown || cityToBuildHouseOn.ColourGroup == Property.Colour.Blue) ?
				cityToBuildHouseOn.CostOfHouse * 2 : cityToBuildHouseOn.CostOfHouse * 3;

			Bank.AddMoney(GameController.PlayerWhosTurn, totalCostOfBuildingHouse);

			foreach (Property property in GameController.PlayerWhosTurn.Properties)
				if (property.ColourGroup == cityToBuildHouseOn.ColourGroup)
				{
					City city = (City)property;
					city.noOfHouses--;

					if (city.noOfHouses < City.Houses.Four)
						city.houses[(int)city.noOfHouses].SetActive(false);
					else
					{
						city.houses[(int)City.Houses.One - 1].SetActive(true);
						city.houses[(int)City.Houses.Two - 1].SetActive(true);
						city.houses[(int)City.Houses.Three - 1].SetActive(true);
						city.houses[(int)City.Houses.Four - 1].SetActive(true);
						city.houses[(int)City.Houses.Hotel - 1].SetActive(false);
					}
					city.housesBuilt[(int)city.noOfHouses] = false;
					Debug.Log(GameController.PlayerWhosTurn.GetName() + " has created house no. " + city.noOfHouses + " on the City \"" + city.Name + "\"");
				}
			GameController.PlayerWhosTurn.noOfHousesCreated--;
		}
		DisableHouseCreationPanel();
	}

	public void DisableHouseCreationPanel()
	{
		HouseCreationSellProperty.gameObject.SetActive(false);
		HouseCreationBuyHouse.gameObject.SetActive(false);
		HouseCreationSellHouse.gameObject.SetActive(false);

		HouseCreationPanel.gameObject.SetActive(false);
		InputSystem.CannotPan = false;
	}

	public static void BuyHouseForAI(City cityToBuildHouseOn)
	{
		int totalCostOfBuildingHouse =
			(cityToBuildHouseOn.ColourGroup == Property.Colour.Brown || cityToBuildHouseOn.ColourGroup == Property.Colour.Blue) ?
			cityToBuildHouseOn.CostOfHouse * 2 : cityToBuildHouseOn.CostOfHouse * 3;

		Bank.DeductMoney(GameController.PlayerWhosTurn, totalCostOfBuildingHouse);

		foreach (Property property in GameController.PlayerWhosTurn.Properties)
			if (property.ColourGroup == cityToBuildHouseOn.ColourGroup)
			{
				City city = (City)property;
				if (city.noOfHouses < City.Houses.Four)
					city.houses[(int)city.noOfHouses].SetActive(true);
				else
				{
					city.houses[(int)City.Houses.One - 1].SetActive(false);
					city.houses[(int)City.Houses.Two - 1].SetActive(false);
					city.houses[(int)City.Houses.Three - 1].SetActive(false);
					city.houses[(int)City.Houses.Four - 1].SetActive(false);
					city.houses[(int)City.Houses.Hotel - 1].SetActive(true);
					GameController.PlayerWhosTurn.noOfHotelsCreated++;
				}

				city.housesBuilt[(int)city.noOfHouses] = true;
				city.noOfHouses++;
				Debug.Log("AI Agent - " + GameController.PlayerWhosTurn.GetName() + ": I have created house no. " + city.noOfHouses + " on the City \"" + city.Name + "\"");
			}
		GameController.PlayerWhosTurn.noOfHousesCreated++;
	}

	public static void SellHouseForAI(City cityToBuildHouseOn)
	{
		if (cityToBuildHouseOn.noOfHouses > City.Houses.None)
		{
			int totalCostOfBuildingHouse =
				(cityToBuildHouseOn.ColourGroup == Property.Colour.Brown || cityToBuildHouseOn.ColourGroup == Property.Colour.Blue) ?
				cityToBuildHouseOn.CostOfHouse * 2 : cityToBuildHouseOn.CostOfHouse * 3;

			Bank.AddMoney(GameController.PlayerWhosTurn, totalCostOfBuildingHouse);

			foreach (Property property in GameController.PlayerWhosTurn.Properties)
				if (property.ColourGroup == cityToBuildHouseOn.ColourGroup)
				{
					City city = (City)property;
					city.noOfHouses--;

					if (city.noOfHouses < City.Houses.Four)
						city.houses[(int)city.noOfHouses].SetActive(false);
					else
					{
						city.houses[(int)City.Houses.One - 1].SetActive(true);
						city.houses[(int)City.Houses.Two - 1].SetActive(true);
						city.houses[(int)City.Houses.Three - 1].SetActive(true);
						city.houses[(int)City.Houses.Four - 1].SetActive(true);
						city.houses[(int)City.Houses.Hotel - 1].SetActive(false);
						GameController.PlayerWhosTurn.noOfHotelsCreated--;
					}
					city.housesBuilt[(int)city.noOfHouses] = false;
					Debug.Log("AI Agent - " + GameController.PlayerWhosTurn.GetName() + ": I have created house no. " + city.noOfHouses + " on the City \"" + city.Name + "\"");
				}
			GameController.PlayerWhosTurn.noOfHousesCreated--;
		}
	}

	public void YieldTurn()
	{
		TitleDeed.gameObject.SetActive(false);
		BuyOptionsPanel.SetActive(false);
		InfoPanel.SetActive(false);
		RentNotifierPanel.SetActive(false);
		instance.TitleDeed.gameObject.SetActive(false);
		TurnOptionsPanel.SetActive(false);
		MessageNotifierText.gameObject.SetActive(false);

		InputSystem.CannotPan = false;
		EventBroker.InvokeYieldPlayerTurn();
	}

	public void GiveUp()
	{
		SetSecondaryText(GameController.PlayerWhosTurn.GetPlayerNumber() 
			+ ": " + GameController.PlayerWhosTurn.GetName() + " has Given Up!");

		GameController.PlayerWhosTurn.GiveUp();
		YieldTurn();
	}

	public void BuyProperty()
	{
		GameController.PlayerWhosTurn.BuyProperty();
		YieldTurn();
	}

	public void SellProperty()
	{
		GameController.PlayerWhosTurn.SellProperty(cityToBuildHouseOn);
		YieldTurn();
		DisableHouseCreationPanel();
	}

	public void ForfeitGame()
	{
		GameStateText.text = "This game has been forfeit!";
		GameEndOptionsPanel.SetActive(false);
		TurnOptionsPanel.SetActive(false);
		GameEndScreen.SetActive(true);
		GameEndReasonText.text = "This game has been forfeit!";

		GameController.ForfeitGame();
	}

	public void QuitToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	#endregion

	#endregion
}
