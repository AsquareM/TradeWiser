using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Variables
	[Space]
	[Header("Player Properties")]
	[SerializeField]
	private GameController.Players playerNumber;

	[SerializeField]
	private string playerName;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private int money;

	public int StationsOwned;
	public int UtilOwned;
	public int noOfHousesCreated;
	public int noOfHotelsCreated;

	[SerializeField]
	private Property[] myProperties;

	public readonly int[] cornerWaypointIndices = { 40, 10, 20, 30 };

	[Space]
	[Header("Player Positions")]
	public int tileNumber;
	public int nextCornerIndex;
	public int nextCornerTile;
	public int otherPlayerPositionsChecked = 0;
	public int newPositionFlag = 2;

	// Distance used to seperate two tokens on the same tile
	private readonly float deltaX = 0.8f;
	private readonly float deltaY = 0.8f;

	[SerializeField]
	private Vector3 targetPosition;

	[SerializeField]
	private Vector3 intermediateCorner;

	[Space]
	[Header("Player Flags")]
	[SerializeField]
	private bool newPositionSet;

	[SerializeField]
	private bool needToTurnCorner;
	private bool freeSlotfound;
	private bool cornerAlreadyCheckedFor;

	public bool isAI;
	public bool landedOnOwnProperty;
	public bool doNotCollect;
	public bool jailed;
	public bool advanceToGo;

	private AIAgent AI;

	// Rigidbody component of this player
	private BoxCollider2D myCollider;

	[Space]
	[Header("Player Tile")]
	// Object of Base Type tile to latch onto Tiles;
	public Tile tile;

	// dynamic type, can change object type during run time
	// stores a reference to the type of tile we are on at the moment
	public dynamic currentTile;

	// Type of current Tile
	public Tile.TypeOfTile typeOfTile;

	public List<Property> Properties;
	public Dictionary<Property.Colour, int> PropertiesOfColourGroupOwned;
	#endregion

	#region Methods

	#region -- Main methods -- 
	private void Start()
	{
		myCollider = gameObject.GetComponent<BoxCollider2D>();
		currentTile = GameObject.Find("(0) Start").GetComponent<StartTile>();

		nextCornerIndex = 1;
		tileNumber = 0;
		nextCornerTile = cornerWaypointIndices[nextCornerIndex];

		newPositionSet = false;
		needToTurnCorner = false;
		doNotCollect = false;
		jailed = false;

		targetPosition = transform.position;

		Properties = new List<Property>();
		PropertiesOfColourGroupOwned = new Dictionary<Property.Colour, int>();
		AI = GetComponent<AIAgent>();
	}

	private void Update()
	{
		// If New Position has been set, i.e. after Dice Roll
		if (newPositionSet)
		{
			// Check if any other player is present on the tile already
			// If so, calculate free slot out of 4 slots on each tile
			while (!freeSlotfound)
			{
				// Check if current slot is occupied by any other player
				foreach (var player in GameController.ListOfPlayers)
				{
					if ( (Math.Round(player.transform.position.x, 1) != Math.Round(targetPosition.x, 1))
						|| (Math.Round(player.transform.position.y, 1) != Math.Round(targetPosition.y, 1)) )
						otherPlayerPositionsChecked++;
					else
						break;
				}

				// If current slot is occupied by any player
				// We do this by checking if 'otherPlayerPositionsChecked' equals the Total Number of Active Players
				// Bug Occured here: Instead of Using variable NoOfPlayers, I hardcoded the value 4 assuming 4 players
				// This caused the game to hang whenever there were less than 4 active players
				if (otherPlayerPositionsChecked != GameController.NoOfPlayers)
				{
					// modify targetPosition to shift it to next slot
					switch (newPositionFlag)
					{
						case 2: targetPosition = new Vector3(targetPosition.x, targetPosition.y + deltaY, 0f); // 2nd slot
								break;

						case 3: targetPosition = new Vector3(targetPosition.x + deltaX, targetPosition.y, 0f); // 3rd slot
								break;

						case 4:	targetPosition = new Vector3(targetPosition.x, targetPosition.y - deltaY, 0f); // 4th slot
								break;
					}
					newPositionFlag++; // Indicates which slot to choose next
				}
				else
					freeSlotfound = true; // Found a slot

				// Reset this counter for next iteration
				otherPlayerPositionsChecked = 0;
			}

			// Check if Player needs to turn a corner on the Player tile's travel to the new tile
			// Otherwise player crosses directly between the board to reach target tile
			// If we already found out that we Need To Turn Corner, don't check again, save system resources
			if (!cornerAlreadyCheckedFor && !needToTurnCorner)
			{
				// If nextCornerIndex == 0, it is a corner case and needs to be handled differently
				if (nextCornerIndex == 0 && (tileNumber > 0 && tileNumber < 30))
				{
					needToTurnCorner = true;
					intermediateCorner = GameController.Waypoints[0].transform.position;

					nextCornerIndex = 1;
					nextCornerTile = cornerWaypointIndices[nextCornerIndex];
				}
				// if new tilenumber is greater than the tilenumber of next corner tile,
				// we need to turn around a corner
				else if (tileNumber >= nextCornerTile)
				{
					if (tileNumber > nextCornerTile)
					{
						needToTurnCorner = true;
						intermediateCorner = GameController.Waypoints[cornerWaypointIndices[nextCornerIndex]].transform.position;
					}

					nextCornerIndex = (nextCornerIndex + 1) % 4;
					nextCornerTile = cornerWaypointIndices[nextCornerIndex];
				}

				cornerAlreadyCheckedFor = true;
			}

			// Move Player Token to new tile's calculated position
			if (needToTurnCorner)
			{
				transform.position = Vector2.MoveTowards(transform.position, intermediateCorner, Time.deltaTime * 20f);

				if (transform.position == intermediateCorner)
				{
					// If we are passing Start, collect 200
					if (!jailed && !doNotCollect && myCollider.IsTouching(GameController.Waypoints[0].GetComponent<BoxCollider2D>()))
					{
						Bank.GetPayday(this);
						Debug.Log("Player " + GetPlayerNumber() + ": Passing Start, Collect 200");
					}

					needToTurnCorner = false;
					Debug.Log("Player " + GetPlayerNumber() + ": Corner Reached");
				}
			}
			else
			{
				transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * 15f);

				if (transform.position == targetPosition)
				{
					newPositionSet = false;
					freeSlotfound = false;
					cornerAlreadyCheckedFor = false;

					newPositionFlag = 2;
					Debug.Log("Player " + GetPlayerNumber() + ": Reached Location");

					// Notifies TokenMoving State that Token has Reached destination
					if (GameController.PlayerWhosTurn == this && !jailed && !doNotCollect && !advanceToGo)
						EventBroker.InvokePlayerTokenReached();
				}
			}
		}
	}

	/// <summary>
	/// Dictates what action to be taken when a new tile is entered, based on type of tile.
	/// Called by TokenMovingState after TokenReached Event
	/// </summary>
	public void NewTileEntered()
	{
		if (tile)
		{
			if (isAI)
				AI.NewTileEntered(tile, typeOfTile);
			else
			{
				switch (typeOfTile)
				{
					case Tile.TypeOfTile.StartTile:
						{
							currentTile = (StartTile)tile;

							if (!doNotCollect && !jailed)
							{
								UIManager.Instance.MessageNotifierText.text = GetName() + " has reached start and gained ₹200!";
								UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
								Debug.Log("Player " + GetPlayerNumber() + ": Payday! Reached Start.");
								Bank.GetPayday(this);
							}

							break;
						}

					case Tile.TypeOfTile.City:
						{
							currentTile = (City)tile;
							landedOnOwnProperty = currentTile.PlayerOwner == this;

							Debug.Log(playerNumber + " has landed on " + currentTile.Name + " which is a " + typeOfTile);
							Debug.Log(playerNumber + ": Whose Property ? " + currentTile.CurrentOwner);

							if (landedOnOwnProperty)
							{
								if (isAI)
									EventBroker.InvokeYieldPlayerTurn();
								else
								{
									UIManager.EnableInfoPanel(currentTile);
									UIManager.Instance.MessageNotifierText.text = GetName() + " has landed on his own property.";
									UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
								}
							}
							else if (currentTile.CurrentOwner != GameController.Players.NoOne && currentTile.PlayerOwner != this)
								UIManager.EnableRentOptions(currentTile, this);
							else
								UIManager.EnableBuyOptions(currentTile, GetMoney());

							break;
						}

					case Tile.TypeOfTile.Station:
						{
							currentTile = (Station)tile;
							landedOnOwnProperty = currentTile.PlayerOwner == this;

							Debug.Log(playerNumber + " has landed on " + currentTile.Name + " which is a " + typeOfTile);
							Debug.Log(playerNumber + ": Whose Property ? " + currentTile.CurrentOwner);

							if (landedOnOwnProperty)
							{
								UIManager.EnableInfoPanel(currentTile);
								UIManager.Instance.MessageNotifierText.text = GetName() + " has landed on his own property.";
								UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							}
							else if (currentTile.CurrentOwner != GameController.Players.NoOne && currentTile.PlayerOwner != this)
								UIManager.EnableRentOptions(currentTile, this);
							else
								UIManager.EnableBuyOptions(currentTile, GetMoney());

							break;
						}

					case Tile.TypeOfTile.Utility:
						{
							currentTile = (Utility)tile;
							landedOnOwnProperty = currentTile.PlayerOwner == this;

							Debug.Log(playerNumber + " has landed on " + currentTile.Name + " which is a " + typeOfTile);
							Debug.Log(playerNumber + ": Whose Property ? " + currentTile.CurrentOwner);

							if (landedOnOwnProperty)
							{
								UIManager.EnableInfoPanel(currentTile);
								UIManager.Instance.MessageNotifierText.text = GetName() + " has landed on his own property.";
								UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							}
							else if (currentTile.CurrentOwner != GameController.Players.NoOne && currentTile.PlayerOwner != this)
								UIManager.EnableRentOptions(currentTile, this);
							else
								UIManager.EnableBuyOptions(currentTile, GetMoney());

							break;
						}

					case Tile.TypeOfTile.IncomeTax:
						{
							currentTile = (IncomeTax)tile;

							UIManager.Instance.MessageNotifierText.text = "Income tax! " + GetName() + " has been taxed ₹200.";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							Bank.DeductMoney(this, 200);

							break;
						}

					case Tile.TypeOfTile.SuperTax:
						{
							currentTile = (SuperTax)tile;

							UIManager.Instance.MessageNotifierText.text = "Super tax! " + GetName() + " has been taxed ₹100.";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							Bank.DeductMoney(this, 100);

							break;
						}

					case Tile.TypeOfTile.CommunityChest:
						{
							currentTile = (CommunityChest)tile;
							Debug.Log(playerNumber + ": has reached Community Chest");

							Bank.AddMoney(this, 200);
							UIManager.SetSecondaryText(GetPlayerNumber() + " has received IncomeTax Returns and gains 200!");
							UIManager.Instance.MessageNotifierText.text = GetName() + " has received IncomeTax Returns and gains 200!";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);

							break;
						}

					case Tile.TypeOfTile.Chance:
						{
							currentTile = (Chance)tile;
							Debug.Log(playerNumber + ": has reached Chance");
							RetraceToStart();

							UIManager.SetSecondaryText(GetPlayerNumber() + " has lost 100, will Pass Go, and does not collect 200.");
							UIManager.Instance.MessageNotifierText.text = GetName() + " has lost 100, will Pass Go, and does not collect 200.";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							Bank.DeductMoney(this, 100);

							break;
						}

					case Tile.TypeOfTile.FreeParking:
						{
							currentTile = (FreeParking)tile;

							Debug.Log(playerNumber + ": has reached Free Parking");
							UIManager.Instance.MessageNotifierText.text = GetName() + " has landed on Free Parking.";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);

							break;
						}

					case Tile.TypeOfTile.GoToJail:
						{
							currentTile = (GoToJail)tile;

							UIManager.Instance.MessageNotifierText.text = GetName() + " has landed on GoToJail, and has been jailed!";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);
							Bank.FinePlayer(this);
							GoToJail();

							break;
						}

					case Tile.TypeOfTile.Jail:
						{
							currentTile = (Jail)tile;

							UIManager.Instance.MessageNotifierText.text = GetName() + " is just visiting the Jail.";
							UIManager.Instance.MessageNotifierText.gameObject.SetActive(true);

							break;
						}
				}
			}
		}
	}

	#endregion

	#region -- other methods --
	public void RetraceToStart()
	{
		SetTileNumber(0);
		nextCornerIndex = 1;
		tileNumber = 0;
		nextCornerTile = cornerWaypointIndices[nextCornerIndex];
		doNotCollect = true;

		if (gameObject.activeInHierarchy)
			StartCoroutine(nameof(CanCollectPayday));
	}

	public void GoToJail()
	{
		jailed = true;
		SetTileNumber(10);

		if (gameObject.activeInHierarchy)
			StartCoroutine(nameof(GetOutOfJail));
	}

	/// <summary>
	/// When this Player's Collider is triggered by a collision with a tile's collider.
	/// Get the Tile type that exists on it.
	/// </summary>
	/// <param name="collision"></param>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		tile = collision.gameObject.GetComponent<Tile>();
		typeOfTile = tile.TileType;
	}

	/// <summary>
	/// Called by UIManager when the Buy Property button is pressed.
	/// Instructs Bank to Buy current Property for this Player
	/// </summary>
	public void BuyProperty()
	{
		Bank.BuyProperty(this, (Property)currentTile);
		StartCoroutine(nameof(UpdateValuesOfPropertyArray));
	}

	public void SellProperty(Property cityToBuildHouseOn)
	{
		Bank.SellProperty(this, cityToBuildHouseOn);
		StartCoroutine(nameof(UpdateValuesOfPropertyArray));
	}

	IEnumerator UpdateValuesOfPropertyArray()
	{
		yield return new WaitForSeconds(1f);
		myProperties = Properties.ToArray();
	}

	IEnumerator GetOutOfJail()
	{
		yield return new WaitForSeconds(5f);
		jailed = false;
	}

	IEnumerator CanCollectPayday()
	{
		yield return new WaitForSeconds(5f);
		doNotCollect = false;
	}

	IEnumerator HasAdvancedToGo()
	{
		yield return new WaitForSeconds(5f);
		advanceToGo = false;
	}

	/// <summary>
	/// When game ends, this method is called by GameController. 
	/// Counts total wealth by summing up cash, property costs and houses/hotels, if any.
	/// </summary>
	public int GetFinalWealth()
	{
		int finalWealth = GetMoney();

		foreach(var property in Properties)
		{
			finalWealth += property.Cost;
			if (property.TileType == Tile.TypeOfTile.City)
			{
				City city = (City)property;
				finalWealth += city.CostOfHouse * Math.Min((int)city.noOfHouses, 4);

				if (city.noOfHouses == City.Houses.Hotel)
					finalWealth += city.CostOfHotel;
			}
		}

		return finalWealth;
	}

	/// <summary>
	/// End of game for this player. Occurs when player is out of wealth.
	/// </summary>
	public void DeclareBankruptcy()
	{
		Debug.Log("Player " + GetPlayerNumber() + ". " + GetName() + " is bankrupt!");
		UIManager.SetGameStateText("Player " + GetPlayerNumber() + " - " + GetName() + " is bankrupt!");

		foreach (var property in Properties)
		{
			property.CurrentOwner = GameController.Players.NoOne;
			property.PlayerOwner = null;
		}

		Properties.Clear();
		GameController.RemovePlayer(this, 0);
	}

	/// <summary>
	/// End of game for this player. Occurs when player gives up through UI GiveUp Button
	/// </summary>
	public void GiveUp()
	{
		foreach (var property in Properties)
		{
			property.CurrentOwner = GameController.Players.NoOne;
			property.PlayerOwner = null;
		}

		Properties.Clear();
		GameController.RemovePlayer(this, 1);
	}
	#endregion

	#region -- Setters -- 
	public void SetInfo(GameController.Players number, string playerName, Sprite sprite, int startingMoney, int isAI)
	{
		playerNumber = number;
		this.playerName = playerName;
		this.sprite = sprite;
		money = startingMoney;

		tileNumber = 0;
		this.isAI = (isAI == 1);
		AI.PlayerCreated(this.isAI);
	}

	public void SetMoney(int newMoney)
	{
		money = newMoney;
	}

	public void SetPlayerNumber(GameController.Players newNumber)
	{
		playerNumber = newNumber;
	}

	public void SetTileNumber(int newTileNumber)
	{
		tileNumber = newTileNumber;
		targetPosition = GameController.Waypoints[tileNumber].transform.position;
		newPositionSet = true;
	}
	#endregion

	#region -- Getters --
		public GameController.Players GetPlayerNumber()
		{
			return playerNumber;
		}

		public string GetName()
		{
			return playerName;
		}

		public Sprite GetSprite()
		{
			return sprite;
		}

		public int GetMoney()
		{
			return money;
		}

		public int GetTileNumber()
		{
			return tileNumber;
		}

	#endregion
	#endregion

} // Player
