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

	public void PopulateRooms()
	{
		rooms = new List<Room>();

		rooms.AddRange( PopulateIntro() );
		int endindex = rooms.Count - 1;
		rooms.AddRange ( PopulateTunnel());
		ConnectToEast( rooms[endindex], rooms[endindex+1]);
		endindex = rooms.Count - 1;

		rooms.AddRange( PopulateValley() );
		ConnectToEast( rooms[endindex], rooms[endindex+1]);
		endindex = rooms.Count - 1;
		int valleyend = endindex;

		rooms.Add ( new Room("The path is blocked off here with roughly hewn logs.",
		                     "The massive logs are stacked up higher than you can see past.  In the center, a small sign is posted. \"Labyrinth closed for repairs. Sorry.\"")
		           );
		ConnectToNorth(rooms[valleyend], rooms[endindex+1]);
		endindex = rooms.Count - 1;

		rooms.AddRange( PopulateForest() );
		ConnectToSouth( rooms[valleyend], rooms[endindex + 1]);
		endindex = rooms.Count - 1;

		rooms.AddRange ( PopulateCave() );
		ConnectToEast( rooms[valleyend], rooms[endindex + 1]);
		endindex = rooms.Count - 1;

		currentRoom = rooms[0];
		lastRoom = rooms[ endindex];

	}

	// just some statically defined room data
	List<Room> PopulateTestRooms()
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

		List<Room> testrooms = new List<Room>();
		testrooms.Add (first);
		testrooms.Add (second);
		testrooms.Add (third);
		testrooms.Add (fourth);
		testrooms.Add (fifth);

		currentRoom = first;
		lastRoom = third;
		return testrooms;
	}

	List<Room> PopulateIntro()
	{
		Room startroom = new Room( "You begin your quest standing outside a hermit's hut.  Perhaps you should 'look' around.", 
		                          "The Hermit Stan's shack is just south of here.  You hear the sound of rushing water nearby. A path heads east.");
		Room waterfall = new Room( "A small clearing at the base of a tall waterfall.",
		                          "The waterfall has created a large pool of water down below where you stand.  The rocks here are slippery due to the spray.");
		Room hermithut = new Room( "The abode of Stan the Hermit.  Stan sits outside on a stump.",
		                          "As you admire Stan's shack, he starts to speak. \"You must travel east to find the artifact you seek! To the east! Always... to the east.\"  He trails off, with a distant look in his eye. He doesn't seem to have anything else to say.  Perhaps you can find 'help' another way.");
		Room path = new Room("The path continues eastwards.",
		                     "This is a very nice path, if I do say so myself.");

		ConnectToEast(startroom, path);
		ConnectToNorth(startroom, waterfall);
		ConnectToSouth(startroom, hermithut);

		List<Room> intro = new List<Room>();
		intro.Add(startroom);	// by convention, the first room in the list is where we connect these little sub levels
		intro.Add (waterfall);
		intro.Add (hermithut);
		intro.Add (path);	// by convention, the last room in the list is the final room or where it connects to the next sub level

		return intro;
	}

	List<Room> PopulateTunnel()
	{
		Room redkey = new Room("You are at the entrance of an ornately carved tunnel.",
		                       "Traces of paint can still be seen on the weathered stone.  Long in the past, this tunnel would have been very brightly colored.");
		Room bluekey = new Room("A dimly lit tunnel",
		                        "The carvings continue into the depths of the tunnel.");
		Room yellowkey = new Room("A dimly lit tunnel",
		                          "The carvings in this portion of the tunnel seem to have a sun motif.");
		PuzzleRoom reddoor = new PuzzleRoom("A large door dominates this area of the tunnel.",
		                                    "The door extends to the ceiling of the tunnel and is very solidly constructed.  The paint is long faded, but the carvings suggest a pattern of flames.",
		                                    "red key");
		PuzzleRoom bluedoor = new PuzzleRoom("You come across another door.",
		                                     "After a dozen or so feet further down the tunnel, your path is blocked by another large door.  The patterns remind you of fish swimming in a river.",
		                                     "blue key");
		PuzzleRoom yellowdoor = new PuzzleRoom("A third large door fills the tunnel.",
		                                       "You walk for a few minutes until you come to a large double door.  A huge icon of a rising sun extends across both doors.",
		                                       "yellow key");
		Room path = new Room("You squint as you step into the sunlight beyond the door.",
		                     "After your eyes adjust to the bright light, you realize you now stand in a long narrow valley.  The path ahead of you is laid with stones, but they are very overgrown.  No human has traveled this path in many ages.");

		// connect rooms
		ConnectToEast(redkey, bluekey);
		ConnectToEast(bluekey, yellowkey);
		ConnectToEast(yellowkey, reddoor);

		HiddenToEast(reddoor, bluedoor);
		HiddenToEast(bluedoor, yellowdoor);
		HiddenToEast(yellowdoor, path);

		// add items
		redkey.items.Add ("red key");
		bluekey.items.Add ("blue key");
		yellowkey.hiddenItems.Add( "yellow key");

		List<Room> tunnel = new List<Room>();
		tunnel.Add (redkey);
		tunnel.Add (bluekey);
		tunnel.Add (yellowkey);
		tunnel.Add (reddoor);
		tunnel.Add (bluedoor);
		tunnel.Add (yellowdoor);
		tunnel.Add (path);

		return tunnel;
	}

	List<Room> PopulateValley()
	{
		Room westvalley = new Room("The west edge of the valley.", 
		                           "Steep mountains extend to the north and south, making progress impossible.  The overgrown path continues east.");
		Room centralvalley = new Room("The path splits into different directions here.",
		                              "The main stone path extends to the east, but you notice a smaller stone path leading south and a dirt path leading north.");

		ConnectToEast(westvalley, centralvalley);

		List<Room> valley = new List<Room>();
		valley.Add (westvalley);
		valley.Add(centralvalley);
		return valley;
	}

	List<Room> PopulateForest()
	{
		Room f1 = new Room( "The entrance to a dark forest",
		                   "The path lead into a dark forest.  You notice that the stones in the path are carved into small skulls.");
		Room f2 = new Room("You are in a dark forest.",
		                   "You seem to have lost the path.  The forest grows too thick to continue in this direction.");
		Room f3 = new Room("You are in a dark forest.",
		                   "Despite being a clear bright day, the sun is barely visible through the canopy of the forest.  The small stone skulls of the path stare up at you.");
		Room f4 = new Room("You are in a dark forest.",
		                   "The path appears to split into two directions here.  Curiously, the path to west seems to be made of a different type of stone.");
		Room f5 = new Room("You are in a dark forest.",
		                   "The stone path is very overgrown here.");
		Room f6 = new Room("You are in a dark forest.",
		                   "You can't seem to penetrate any further along the path.");
		Room f7 = new Room("You are in a small clearing.",
		                   "Just ahead you can see broken stones in piles.  A somewhat disappointed ghost floats in the center of the path.  \"Oh, hello.  There was supposed to be a big graveyard with ghosts and puzzles and stuff ahead, but I ran out of time trying to set it up.  So maybe just search around here at the entrance.  It's seriously a wreck in there.\"");

		f7.hiddenItems.Add ("skull key");

		ConnectToSouth(f1, f3);
		ConnectToWest(f1, f2);
		ConnectToWest(f3, f4);
		ConnectToWest(f4, f5);
		ConnectToNorth(f5, f6);
		ConnectToSouth(f4, f7);

		List<Room> forest = new List<Room>();
		forest.Add (f1);
		forest.Add (f2);
		forest.Add (f3);
		forest.Add (f4);
		forest.Add (f5);
		forest.Add (f6);
		forest.Add (f7); // this is the 'exit' room

		return forest;
	}

	List<Room> PopulateCave()
	{
		Room entrance = new Room("The entrance to a large cave.",
		                         "In the center of a steep wall of stone, an archway marks the entrance to some sort of cave.");
		Room c1 = new Room("You creep along a dark cave.",
		                   "The air is damp and musty.  You hear the dripping water in the distance.");
		Room c2 = new Room("You creep along a dark cave.",
		                   "You can make out two possible exits here, but is not clear which way to travel.");
		Room c3 = new Room("You creep along a dark cave.",
		                   "The floor is wet here.  Water seems to be dripping from the ceiling.");
		PuzzleRoom c4 = new PuzzleRoom("You creep along a dark cave.",
		                               "An enormous cast iron door in the shape of a skull dominates this room. There is a keyhole in the center of the door which you can just reach.",
		                               "skull key");
		Room c5 = new Room("You stumble into a large room.",
		                   "You are surprised at the immensity of the room behind the skull door.  Piles of glittering treasure line the walls and rich tapestries hang from the ceiling.  Foreign treasures you have never before seen the likes of lay amongst the treasure.");
		Room c6 = new Room("To the east of the large room sits a large throne.",
		                   "A throne sits prominently amongst the riches.  You are the first in many generations to discover the secret of being able to hold more than one thing at a time.  You may now rightly accend the throne, as Grand High Bag Master The Exalted!");

		ConnectToEast( entrance, c1);
		ConnectToNorth( c1, c2);
		ConnectToNorth( c2, c3);
		ConnectToEast(c2, c4);
		HiddenToEast(c4, c5);
		ConnectToEast(c5, c6);

		c5.items.Add ("purse");
		c5.items.Add ("backpack");
		c5.items.Add ("satchel");
		c5.items.Add ("hobo bindle");
		c5.items.Add ("messenger bag");
		c5.items.Add ("bag of holding");

		List<Room> cave = new List<Room>();
		cave.Add (entrance);
		cave.Add (c1);
		cave.Add (c2);
		cave.Add (c3);
		cave.Add (c4);
		cave.Add (c5);
		cave.Add (c6); // the exit room of this zone AND the game
		return cave;
	}

	#region utilfuncs

	void HiddenToEast( PuzzleRoom first, Room second)
	{
		first.hiddenExits.Add ("east", second);
		second.exits.Add ("west", first);
	}

	void ConnectToEast( Room first, Room second)
	{
		first.exits.Add ("east", second);
		second.exits.Add ("west", first);
	}

	void ConnectToWest( Room first, Room second)
	{
		first.exits.Add ("west", second);
		second.exits.Add ("east", first);
	}

	void ConnectToNorth( Room first, Room second)
	{
		first.exits.Add ("north", second);
		second.exits.Add ("south", first);
	}

	void ConnectToSouth( Room first, Room second)
	{
		first.exits.Add ("south", second);
		second.exits.Add ("north", first);
	}

	#endregion
}
