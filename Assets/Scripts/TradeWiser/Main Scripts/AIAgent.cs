using System;
using System.Collections;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
	public Player thisPlayer;

	private bool playerLoaded;
	private GameBaseState.States currentState;

	private int buyingMetric;

	// Start is called before the first frame update
	void Start()
	{
		EventBroker.AINewStateNotify += EventBroker_AINewStateNotify; // Publisher: GameController
	}

	#region -- Imp Methods --
	// Decides how to behave upon landing on any Tile
	public void NewTileEntered(Tile tile, Tile.TypeOfTile typeOfTile)
	{
		switch (typeOfTile)
		{
			case Tile.TypeOfTile.StartTile:
				{
					thisPlayer.currentTile = (StartTile)tile;

					if (!thisPlayer.doNotCollect && !thisPlayer.jailed)
					{
						UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() + ": I have reached start and gained ₹200!");
						Debug.Log("AI Agent - " + thisPlayer.GetPlayerNumber() + ": Payday! Reached Start.");
						Bank.GetPayday(thisPlayer);
					}
					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.City:
				{
					thisPlayer.currentTile = (City)tile;
					City currentCity = (City)tile;
					thisPlayer.landedOnOwnProperty = thisPlayer.currentTile.PlayerOwner == this;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have landed on " + thisPlayer.currentTile.Name + " which is a " + typeOfTile);
					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": Whose Property ? " + thisPlayer.currentTile.CurrentOwner);

					if (thisPlayer.landedOnOwnProperty)
					{
						UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() + ": I landed on my own property.");
						EventBroker.InvokeYieldPlayerTurn();
					}
					else if (thisPlayer.currentTile.CurrentOwner != GameController.Players.NoOne && thisPlayer.currentTile.PlayerOwner != thisPlayer)
					{
						int playerWealth = thisPlayer.GetMoney();
						int propertyRent = currentCity.GetRent();

						// If Player can afford the rent, show difference, else display Bankrupt
						if (propertyRent <= playerWealth)
						{
							UIManager.SetSecondaryText(
								"AI Agent - " + thisPlayer.GetName() +
								": I paid ₹" + propertyRent + " to " + currentCity.PlayerOwner.GetName()
							);

							// Deduct from rentee, give rent to owner
							Bank.DeductMoney(thisPlayer, propertyRent);
							Bank.AddMoney(currentCity.PlayerOwner, propertyRent);
						}
						else
						{
							DeclareBankrupty();
						}
						EventBroker.InvokeYieldPlayerTurn();
					}
					else
						DecideToBuy((Property)thisPlayer.currentTile);

					break;
				}

			case Tile.TypeOfTile.Station:
				{
					thisPlayer.currentTile = (Station)tile;
					Station currentStation = (Station)tile;
					thisPlayer.landedOnOwnProperty = thisPlayer.currentTile.PlayerOwner == this;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have landed on " + thisPlayer.currentTile.Name + " which is a " + typeOfTile);
					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": Whose Property ? " + thisPlayer.currentTile.CurrentOwner);

					if (thisPlayer.landedOnOwnProperty)
					{
						UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() + ": I landed on my own property.");
						EventBroker.InvokeYieldPlayerTurn();
					}
					else if (thisPlayer.currentTile.CurrentOwner != GameController.Players.NoOne && thisPlayer.currentTile.PlayerOwner != thisPlayer)
					{
						int playerWealth = thisPlayer.GetMoney();
						int propertyRent = currentStation.GetRent();

						// If Player can afford the rent, show difference, else display Bankrupt
						if (currentStation.GetRent() <= playerWealth)
						{
							UIManager.SetSecondaryText(
								"AI Agent - " + thisPlayer.GetName() +
								": I paid ₹" + propertyRent + " to " + currentStation.PlayerOwner.GetName()
							);

							// Deduct from rentee, give rent to owner
							Bank.DeductMoney(thisPlayer, propertyRent);
							Bank.AddMoney(currentStation.PlayerOwner, propertyRent);
						}
						else
						{
							DeclareBankrupty();
						}
						EventBroker.InvokeYieldPlayerTurn();
					}
					else
						DecideToBuy((Property)thisPlayer.currentTile);

					break;
				}

			case Tile.TypeOfTile.Utility:
				{
					thisPlayer.currentTile = (Utility)tile;
					Utility currentUtil = (Utility)tile;
					thisPlayer.landedOnOwnProperty = thisPlayer.currentTile.PlayerOwner == this;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have landed on " + thisPlayer.currentTile.Name + " which is a " + typeOfTile);
					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": Whose Property ? " + thisPlayer.currentTile.CurrentOwner);

					if (thisPlayer.landedOnOwnProperty)
					{
						UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() + ": I landed on my own property.");
						EventBroker.InvokeYieldPlayerTurn();
					}
					else if (thisPlayer.currentTile.CurrentOwner != GameController.Players.NoOne && thisPlayer.currentTile.PlayerOwner != thisPlayer)
					{
						int playerWealth = thisPlayer.GetMoney();
						int propertyRent = currentUtil.GetRent();

						// If Player can afford the rent, show difference, else display Bankrupt
						if (currentUtil.GetRent() <= playerWealth)
						{
							UIManager.SetSecondaryText(
								"AI Agent - " + thisPlayer.GetName() +
								": I paid ₹" + propertyRent + " to " + currentUtil.PlayerOwner.GetName()
							); ;

							// Deduct from rentee, give rent to owner
							Bank.DeductMoney(thisPlayer, propertyRent);
							Bank.AddMoney(currentUtil.PlayerOwner, propertyRent);
						}
						else
							DeclareBankrupty();

						EventBroker.InvokeYieldPlayerTurn();
					}
					else
						DecideToBuy((Property)thisPlayer.currentTile);

					break;
				}

			case Tile.TypeOfTile.IncomeTax:
				{
					thisPlayer.currentTile = (IncomeTax)tile;

					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
								": Income tax! I have been taxed ₹200.");

					if (thisPlayer.GetMoney() >= 200)
						Bank.DeductMoney(thisPlayer, 200);
					else
						DeclareBankrupty();

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.SuperTax:
				{
					thisPlayer.currentTile = (SuperTax)tile;

					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
								": Super tax! I have been taxed ₹100.");

					if (thisPlayer.GetMoney() >= 200)
						Bank.DeductMoney(thisPlayer, 200);
					else
						DeclareBankrupty();

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.CommunityChest:
				{
					thisPlayer.currentTile = (CommunityChest)tile;
					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have reached Community Chest");

					Bank.AddMoney(thisPlayer, 200);
					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
						": I have received IncomeTax Returns and gained ₹200!");

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.Chance:
				{
					thisPlayer.currentTile = (Chance)tile;
					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have reached Chance");

					//Send player Token back to Start
					thisPlayer.RetraceToStart();

					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
						": I have lost 100, will Proceed to Go, and won't collect 200 :(");

					if (thisPlayer.GetMoney() >= 100)
						Bank.DeductMoney(thisPlayer, 100);
					else
						DeclareBankrupty();

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.FreeParking:
				{
					thisPlayer.currentTile = (FreeParking)tile;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have reached Free Parking");
					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
						": I have reached Free Parking.");

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.GoToJail:
				{
					thisPlayer.currentTile = (GoToJail)tile;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I have been Jailed!");
					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
						": I have been jailed, and paid a fine of ₹50 :(");

					if (thisPlayer.GetMoney() >= 50)
						Bank.DeductMoney(thisPlayer, 50);
					else
						DeclareBankrupty();

					thisPlayer.GoToJail();
					EventBroker.InvokeYieldPlayerTurn();
					break;
				}

			case Tile.TypeOfTile.Jail:
				{
					thisPlayer.currentTile = (Jail)tile;

					Debug.Log("AI Agent - " + thisPlayer.GetName() + ": I am just visiting Jail.");
					UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
						": I am just visiting Jail.");

					EventBroker.InvokeYieldPlayerTurn();
					break;
				}
		}
	}

	// Simple, Rudimentary FSM
	private void EventBroker_AINewStateNotify(GameBaseState.States newState)
	{
		currentState = newState;

		if (playerLoaded && GameController.PlayerWhosTurn == thisPlayer)
		{
			switch (newState)
			{
				case GameBaseState.States.DiceToBeRolled:
					{
						DecideToSell();
						DecideToBuildHouse();
						RollTheDice();
						break;
					}

				case GameBaseState.States.WinnerDeclared:
					{
						Destroy(this);
						break;
					}

				default: break;
			}
		}
	}

	private void RollTheDice()
	{
		Debug.Log("AI Agent - " + thisPlayer.GetName() + ": Hey, It's my turn!");
		UIManager.Instance.SecondaryText.text = "AI Agent - " + thisPlayer.GetName() + ": Hey, It's my turn! Rolling the Dice...";
		GameObject.FindGameObjectWithTag("Dice").GetComponent<Dice>().RollTheDiceByAI();
	}

	private void DecideToBuy(Property property)
	{
		if (thisPlayer.Properties.Contains(property))
			EventBroker.InvokeYieldPlayerTurn();

		// Reset Buying Metric
		buyingMetric = 0;

		// Don't waste money on cheap properties unless we have bought most other proeprties already
		if (property.ColourGroup == Property.Colour.Brown && thisPlayer.Properties.Count < 20)
			buyingMetric -= 10;

		// If we are low on cash, skip buying
		if (thisPlayer.tileNumber < 30 && thisPlayer.GetMoney() <= 500)
			buyingMetric -= 4;
		else if (thisPlayer.GetMoney() <= 300)
			buyingMetric -= 4;

		// How much extra cash would we have if we bought this property?
		buyingMetric += (thisPlayer.GetMoney() > (property.Cost + 200)) ? 1 : 0;
		buyingMetric += (thisPlayer.GetMoney() > (property.Cost + 400)) ? 1 : 0;
		buyingMetric += (thisPlayer.GetMoney() > (property.Cost + 600)) ? 2 : 0;
		buyingMetric += (thisPlayer.GetMoney() > (property.Cost + 1000)) ? 2 : 0;

		// Prioritize properties of Higher Colour Group, i.e. properties with more rent
		buyingMetric += (int)property.ColourGroup;

		// To Buy Or Not to Buy
		Debug.Log("AI Agent - " + thisPlayer.GetName() + ": Based on my Heuristics, the Buying Metric is " + buyingMetric);
		if (buyingMetric > 0)
		{
			UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
				": Based on my Heuristics, I'm buying " + property.Name + ".");
			thisPlayer.BuyProperty();
		}
		else
			UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() +
				": Based on my Heuristics, I'm not buying " + property.Name + " for now.");

		EventBroker.InvokeYieldPlayerTurn();
	}

	private void DecideToSell()
	{
		if(thisPlayer.GetMoney() <= 100 && thisPlayer.Properties.Count > 1)
		{
			// if Player has built houses, sell houses before selling properties
			if (thisPlayer.noOfHousesCreated == 0)
			{
				int lowestPropertyCost = 10000;
				Property propertyWithLowestCost = null;

				foreach (Property property in thisPlayer.Properties)
				{
					if (property.Cost < lowestPropertyCost)
					{
						lowestPropertyCost = property.Cost;
						propertyWithLowestCost = property;
					}
				}

				Bank.SellProperty(thisPlayer, propertyWithLowestCost);
				Debug.Log("AI Agent - " + thisPlayer.GetName() +
					": I'm Selling " + propertyWithLowestCost.Name + " as I'm running low on funds!");
				UIManager.Instance.SecondaryText.text += "AI Agent - " + thisPlayer.GetName() + 
					": I'm Selling " + propertyWithLowestCost.Name + " as I'm running low on funds!\n";
			}
			else
			{
				int lowestCostPropertyThatHasHouse = 10000;
				City cityWithLowestCostThatHasHouse = null;

				foreach (Property property in thisPlayer.Properties)
				{
					if (property.TileType == Tile.TypeOfTile.City)
					{
						City city = (City)property;

						if (city.noOfHouses > City.Houses.None && 
							city.Cost < lowestCostPropertyThatHasHouse)
						{
							lowestCostPropertyThatHasHouse = city.Cost;
							cityWithLowestCostThatHasHouse = city;
						}
					}
				}

				if (cityWithLowestCostThatHasHouse == null)
					return;

				UIManager.SellHouseForAI(cityWithLowestCostThatHasHouse);
				Debug.Log("AI Agent - " + thisPlayer.GetName() +
					": I'm Selling a house from " + cityWithLowestCostThatHasHouse.Name + " as I'm running low on funds!");
				UIManager.Instance.SecondaryText.text += "AI Agent - " + thisPlayer.GetName() +
					": I'm Selling a house from " + cityWithLowestCostThatHasHouse.Name + " as I'm running low on funds!\n";
			}
		}
	}

	private void DecideToBuildHouse()
	{
		if (thisPlayer.GetMoney() < 500)
			return;

		int lowestHouseCostOfProperty = 10000;
		City propertyWithLowestHouseCost = null;

		foreach(var prop in thisPlayer.PropertiesOfColourGroupOwned)
		{
			if (
				((prop.Key == Property.Colour.Brown || prop.Key == Property.Colour.Blue) && prop.Value == 2)
				 || (prop.Key != Property.Colour.Station && prop.Key != Property.Colour.Utility && prop.Value == 3)
			)
			{
				foreach(var p in thisPlayer.Properties)
				{
					if (p.TileType == Tile.TypeOfTile.City)
					{
						City city = (City)p;

						if (city.ColourGroup == prop.Key && city.CostOfHouse < lowestHouseCostOfProperty)
						{
							lowestHouseCostOfProperty = city.Cost;
							propertyWithLowestHouseCost = city;
						}
					}
				}
			}
		}

		if (propertyWithLowestHouseCost == null)
			return;

		// To Buy Or Not to Buy
		if ((thisPlayer.GetMoney() + 500) > (propertyWithLowestHouseCost.CostOfHouse * 3))
		{
			UIManager.BuyHouseForAI(propertyWithLowestHouseCost);
			UIManager.Instance.SecondaryText.text += "AI Agent - " + thisPlayer.GetName() +
				": Based on my Heuristics, I'm buying a house on " + propertyWithLowestHouseCost.Name + ".\n";
		}
	}
	#endregion

	#region -- Other Methods --
	public void PlayerCreated(bool isAI)
	{
		if (!isAI)
			Destroy(this);
		else
			playerLoaded = true;
	}

	private void DeclareBankrupty()
	{
		UIManager.SetSecondaryText("AI Agent - " + thisPlayer.GetName() + "My funds are exhausted, I am bankrupt! Bye :(");
		StartCoroutine(nameof(NotifyBankruptcy), thisPlayer);
	}

	private IEnumerator NotifyBankruptcy(Player player)
	{
		yield return new WaitForSeconds(1f);
		EventBroker.InvokePlayerBankrupt(player);
	}

	private IEnumerator WaitForSomeSeconds(float secondsToWait)
	{
		yield return new WaitForSeconds(secondsToWait);
	}
	#endregion
}
