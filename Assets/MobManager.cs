using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mob {
	public string name;
	public Room location;
};

public class MobManager : MonoBehaviour {

	List<Mob> mobs;

	public void PopulateMobs()
	{
		mobs = new List<Mob>();
		// nothing for now, but mobs has to be defined
	}

	void PopulateTestMobs()
	{
		mobs = new List<Mob>();

		Mob ghost = new Mob();
		ghost.name = "spooky ghost";

		mobs.Add (ghost);
	}

	// this is an organizationally ugly method... mobs really need to be refactored in the future
	public void MoveMobToRoom( string name, Room room)
	{
		Mob mobToMove = mobs.Find ( delegate( Mob m) { return m.name == name; } );
		if( mobToMove != null )
			mobToMove.location = room;
	}

	public bool AnyMobsAdjacent( Room room)
	{
		bool adj = false;
		foreach( Mob m in mobs)
		{
			// first check if any of the 'known' exits of the mob's current location are the room in question
			foreach( Room re in m.location.exits.Values)
			{
				if( re == room)
					adj = true;
			}
			// ... then the same for hidden rooms
			if( m.location is PuzzleRoom)
			{
				PuzzleRoom pr = m.location as PuzzleRoom;
				foreach( Room re in pr.hiddenExits.Values)
				{
					if( re == room)
						adj = true;
				}
			}
			if( adj)
				break;
		}
		return adj;
	}

	public bool ContainsMob( Room room)
	{
		foreach( Mob m in mobs)
		{
			if( m.location == room)
				return true;
		}
		return false;
	}

	public Mob GetMobByRoom( Room room)
	{
		foreach( Mob m in mobs)
		{
			if( m.location == room)
				return m;
		}
		return null;
	}

	public void MobActions()
	{
		// if a mob is in the current room as the player, attack the player
		// if no mob is the same room as the player, and the current player room is adjance to the mob's current room, move into the current player room
		// i guess otherwise stay put
	}
}
