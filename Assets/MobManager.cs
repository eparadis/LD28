using UnityEngine;
using System.Collections;

public class Mob {
	public string name;
};

public class MobManager : MonoBehaviour {

	public void MoveMobs()
	{
	}

	public bool AnyMobsAdjacent( Room room)
	{
		return false;
	}

	public bool ContainsMob( Room room)
	{
		return false;
	}

	public Mob GetMobByRoom( Room room)
	{
		return new Mob();
	}

	public void MobActions()
	{
	}
}
