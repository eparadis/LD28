using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room {
	public string description, longDesc;
	public List<string> items, hiddenItems;
	public Dictionary<string,Room> exits;
	public Room( string d, string ld) { 
		description = d;
		longDesc = ld;
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

public class PuzzleRoom : Room
{
	public bool requiresKey;
	public string keyItem;
	public Dictionary<string,Room> hiddenExits;
	public PuzzleRoom( string desc, string ldesc, string key)
		: base (desc, ldesc)
	{
		keyItem = key;
		hiddenExits = new Dictionary<string,Room>();
	}
	public string UnlockRoom() // move hidden exits to normal exits
	{
		foreach( KeyValuePair<string,Room> kvp in hiddenExits)
		{
			exits[kvp.Key] = kvp.Value;
		}
		if( hiddenExits.Keys.Count == 0 )
			return "Nothing happened.";
		else if( hiddenExits.Keys.Count == 1 )
		{
			string resp = "A hidden exit opened: ";
			foreach( string s in hiddenExits.Keys)
			{
				resp += s + " ";
			}
			hiddenExits.Clear (); // we don't need these any more
			return resp;
		}
		else {
			string resp = "Several hidden exits opened:";
			foreach( string s in hiddenExits.Keys)
			{
				resp += " " + s;
			}
			hiddenExits.Clear (); // we don't need these any more
			return resp;
		}
	}

};

public class RoomManager : MonoBehaviour {

	public List<Room> rooms;
	public Room currentRoom;
	public Room lastRoom;

	// just some statically defined room data
	public void PopulateTestRooms()
	{
		Room first = new Room( "You are in the entrance of a tunnel", "Vines have nearly overgrown the entrance to a tunnel.  The harsh sun overhead prevents you from seeing more than a few feet inside.");
		Room second = new Room( "You are in the depths of a tunnel", "The air feels damp and smells musty.  You get the feeling that no living thing has been this way in many years.");
		Room third = new Room( "You have escaped the tunnel!", "You step blinking into the bright sunlight of the surface.  You are free!");
		PuzzleRoom fourth = new PuzzleRoom( "This room has a locked door.", "An enormous iron door emblazoned with a skull dominates this room.  There is a keyhole in the center of the door you can just reach.", "skull key");
		Room fifth = new Room( "You are at a dead end.", "This appears to be an empty room, but you wonder why would it be behind a locked door.");
		PuzzleRoom sixth = new PuzzleRoom( "This room has a strange pedestal in the center.", "An ornate pedestal rises from the center of the room.  A single ray of light from somewhere in the ceiling shines onto the pedistal, illuminating a small bowl at the top.", "diamond ring");

		first.exits.Add ("north", second);
		second.exits.Add ("south", first);

		second.exits.Add ("north", sixth);
		sixth.exits.Add ("south", second);

		third.exits.Add ("east", sixth);
		sixth.hiddenExits.Add ("west", third);

		first.hiddenItems.Add ("magic sword");
		second.items.Add ("heavy rock");

		fourth.exits.Add ("west", second);
		second.exits.Add ("east", fourth);
		fourth.items.Add ("skull key");
		fourth.items.Add ("small rock");

		fourth.hiddenExits.Add ("east", fifth);
		fifth.exits.Add ("west", fourth);
		fifth.hiddenItems.Add ("diamond ring");

		GetComponent<MobManager>().MoveMobToRoom( "spooky ghost", fifth);	// put a ghost in there with that ring!

		rooms = new List<Room>();
		rooms.Add (first);
		rooms.Add (second);
		rooms.Add (third);
		rooms.Add (fourth);
		rooms.Add (fifth);

		currentRoom = first;
		lastRoom = third;
	}
}
