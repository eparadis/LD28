using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {

	private string singleSlot = "";	// ok so for this game, we can only carry a single things (its part of the theme!) in the future, this could be expanded to a 'true' inventory system

	public bool CanCarry( string item)
	{
		if( singleSlot == "")
			return true;
		else
			return false;
	}

	public void Carry( string item)
	{
		singleSlot = item;
	}

	public bool Carrying( string item)
	{
		if( singleSlot == item)
			return true;
		else
			return false;
	}

	public void Drop( string item)
	{
		singleSlot = "";
	}

	public string GetContents()
	{
		return singleSlot;
	}
}
