using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room {
	public string description;
	public List<string> items, hiddenItems;
	public Dictionary<string,Room> exits;
	public Room( string d) { 
		description = d;
		items = new List<string>();
		hiddenItems = new List<string>();
		exits = new Dictionary<string,Room>();
	}
	public string GetExitOptions()
	{
		string resp = "Available exits: ";
		foreach( string s in exits.Keys)
		{
			resp += s + " ";
		}
		return resp;
	}
	public string GetItemsInRoom()
	{
		if( items.Count == 0 )
			return "";
		else if( items.Count == 1 )
			return "There is a " + items[0] + " here.";
		else {
			string resp = "In this room, there are: ";
			foreach( string s in items)
			{
				resp += s + " ";
			}
			return resp;
		}
	}
};

public class RoomManager : MonoBehaviour {

	public List<Room> rooms;
	public Room currentRoom;



	// Use this for initialization
	void Start () {
		PopulateTestRooms();
	}

	// just some statically defined room data
	void PopulateTestRooms()
	{
		Room first = new Room( "You are in the entrance of a tunnel");
		Room second = new Room( "You are in the depths of a tunnel");
		Room third = new Room( "You have escaped the tunnel!");

		first.exits.Add ("north", second);
		second.exits.Add ("south", first);

		second.exits.Add ("north", third);
		third.exits.Add ("south", second);

		first.hiddenItems.Add ("magic sword");
		second.items.Add ("heavy rock");

		rooms = new List<Room>();
		rooms.Add (first);
		rooms.Add (second);
		rooms.Add (third);

		currentRoom = first;
	}
}
