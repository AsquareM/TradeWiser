using System.Collections.Generic;
using UnityEngine;

public class City : Property
{
	public enum Houses { None, One, Two, Three, Four, Hotel }
	public Dictionary<Houses, int> Rent;

	[Header("City Properties")]
	public Houses noOfHouses;
	public int CostOfHouse;
	public int CostOfHotel;
	public List<GameObject> houses;
	public bool[] housesBuilt;

	public void SetCity(string name, int cost, Colour colour, int houseCost, int hotelCost)
	{
		Name = name;
		Cost = cost;
		noOfHouses = Houses.None;
		ColourGroup = colour;
		CostOfHouse = houseCost;
		CostOfHotel = hotelCost;

		houses = new List<GameObject>();
		housesBuilt = new bool[5];

		int houseIndex = 0;
		foreach (Transform child in transform)
		{
			houses.Add(child.gameObject);
			child.gameObject.SetActive(false);
			housesBuilt[houseIndex] = false;
		}
	}

	public override int GetRent()
	{
		Rent.TryGetValue(noOfHouses, out int val);
		return val;
	}

	/// <summary>
	/// Set the rent amounts of a city wrt the number of houses or presence of hotel.
	/// </summary>
	/// <param name="none">No Houses</param>
	/// <param name="one">One House</param>
	/// <param name="two">Two Houses</param>
	/// <param name="three">Three Houses</param>
	/// <param name="four">Four Houses</param>
	/// <param name="hotel">Hotel</param>
	public void SetRent(int none, int one, int two, int three, int four, int hotel)
	{
		Rent = new Dictionary<Houses, int>() {
			{ Houses.None, none },
			{ Houses.One, one },
			{ Houses.Two, two },
			{ Houses.Three, three },
			{ Houses.Four, four },
			{ Houses.Hotel, hotel },
		};
	}

} // City class