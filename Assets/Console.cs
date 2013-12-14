using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {

	public GUIText inputText;
	public int maxBufferLength;
	Queue<string> scrollBuffer;
	bool clearCursor = true;
	RoomManager rm;

	// Use this for initialization
	void Start () {
		scrollBuffer = new Queue<string>();
		AddLineToBuffer("'You Only Have One'");
		AddLineToBuffer("LD24 Entry, (c) 2013 Ed Paradis");
		inputText.text = "_";
		rm = GetComponent<RoomManager>();
		AddLineToBuffer(rm.currentRoom.description);
	}
	
	void Update() {
		// ripped off from Unity's docs for Input.inputString
		foreach (char c in Input.inputString) 
		{
			if(clearCursor)
			{
				inputText.text = "";
				clearCursor = false;
			}
			if (c == "\b"[0])
			{
				if (inputText.text.Length != 0)
					inputText.text = inputText.text.Substring(0, inputText.text.Length - 1);
			} else {
				if (c == "\n"[0] || c == "\r"[0]) // user has hit <return> or similar
				{
					ParseCommand(inputText.text);
					inputText.text = "_";
					clearCursor = true;
				}
				else
					inputText.text += c;
			}
		}
	}

	void AddLineToBuffer( string t)
	{
		scrollBuffer.Enqueue(t);

		//check to see if we've got too much scroll back and drop a line
		if( scrollBuffer.Count > maxBufferLength )
			scrollBuffer.Dequeue();

		string concat = "";
		foreach( string s in scrollBuffer)
		{
			concat += s + "\n";
		}
		guiText.text = concat;
	}

	void ParseCommand( string inp)
	{
		string response;

		if( inp == "look")
		{
			response = rm.currentRoom.longDesc + "\n" + rm.currentRoom.GetExitOptions() + "\n" + rm.currentRoom.GetItemsInRoom();
		} else if( inp == "north" || inp == "south" || inp == "east" || inp == "west" || inp == "up" || inp == "down")
		{
			// user is trying to move rooms; first check if its a valid exit for the current room
			if(rm.currentRoom.exits.ContainsKey(inp))
			{
				// travel there and print what it looks like
				rm.currentRoom = rm.currentRoom.exits[inp];
				response = rm.currentRoom.description + "\n" + rm.currentRoom.GetExitOptions();
			} else {
				response = "You cannot travel that direction from here!";
			}
		} else if( inp == "search")
		{
			if(true)//Random.value < 0.5f) // 50%
			{
				if( rm.currentRoom.hiddenItems.Count > 0)
				{
					string itemFound = rm.currentRoom.hiddenItems[0];
					response = "You find a " + itemFound;
					rm.currentRoom.items.Add(itemFound);
					rm.currentRoom.hiddenItems.Remove(itemFound);
				} else
					response = "There doesn't seem to be anything else hidden in this room.";
			} else
				response = "You didn't find anything this time.";
		} else {
			response = "What?";
		}

		// add what the user entered as a 'command'
		AddLineToBuffer( "> " + inp);
		// then add our response
		AddLineToBuffer(response);
	}

}
