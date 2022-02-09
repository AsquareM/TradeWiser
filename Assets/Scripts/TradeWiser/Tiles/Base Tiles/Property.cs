using System;
using UnityEngine;

public abstract class Property : Tile, IComparable
{
	public enum Colour { None, Brown, Cyan, Purple, Orange, Red, Yellow, Green, Blue, Utility, Station }

	[Header("Property Properties")]
	public Colour ColourGroup;
	public string Name;
	public int Cost;
	public int MortageValue;

	public GameController.Players CurrentOwner;
	public Player PlayerOwner;

	public Sprite Deed;
	
	public Property()
	{
		PlayerOwner = null;
		CurrentOwner = GameController.Players.NoOne;
	}

	private void OnMouseDown()
	{
		UIManager.EnableHouseCreationPanel(this);
	}

	abstract public int GetRent();
	public int BuyProperty(Player player)
	{
		PlayerOwner = player;
		CurrentOwner = player.GetPlayerNumber();
		return Cost;
	}

	public int SellProperty()
	{
		PlayerOwner = null;
		CurrentOwner = GameController.Players.NoOne;
		return Cost / 2;
	}

	public int CompareTo(object obj)
	{
		return ColourGroup - ((Property)obj).ColourGroup;
	}
}