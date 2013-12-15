using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour {


	public bool CanCarry( string item)
	{
		return false;
	}

	public void Carry( string item)
	{
	}

	public bool Carrying( string item)
	{
		return false;
	}

	public void Drop( string item)
	{
	}

	public string GetContents()
	{
		return "";
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
