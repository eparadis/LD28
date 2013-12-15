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

		//AddLineToBuffer("123456789 123456789 123456789 1234567890");
		//AddLineToBuffer("This is a very long line that should be wrapped several times and it'll be interesting to see how it works. Hopefully it works very well and there are no issues with various punctuation and other elements.");
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
	
	// return a string with some of the spaces replaced with newlines such that no line is longer than columns
	string WordWrap( string orig)
	{
		int columns = 40; // we could use information from the font guiText is using and Screen.width to determine the ACTUAL number of columns

		// we might not need to do anything
		if( orig.Length <= columns )
			return orig;

		int prevSpace = 0;
		int lineRemaining = columns;
		char[] arr = orig.ToCharArray();
		for( int i=0; i<arr.Length; i+=1)
		{
			// if we find a space, and theres enough of the original line left to require more wrapping
			if(arr[i] == ' ' && (arr.Length - i ) > columns)
			{
				if(lineRemaining <= 0)
				{
					arr[prevSpace] = '\n';
					lineRemaining = columns - (i-prevSpace);
					prevSpace = i;
				} else
				{
					prevSpace = i;
				}
			}

			lineRemaining -= 1;
		}

		return new string(arr);
	}

	// from psuedocode on wikipedia 'word wrap'
	string BetterWordWrap( string orig)
	{
		int columns = 40;
		char[] orig_arr = orig.ToCharArray();
		int index = 0;

		//SpaceLeft := LineWidth
		int spaceLeft = columns;
		//for each Word in Text
		foreach( string word in orig.Split(' ') )
		{
			//if (Width(Word) + SpaceWidth) > SpaceLeft
			if( word.Length + 1 > spaceLeft)
			{
				//insert line break before Word in Text
				orig_arr[index-1] = '\n';
				//SpaceLeft := LineWidth - Width(Word)
				spaceLeft = columns - word.Length;
			} else {
				//SpaceLeft := SpaceLeft - (Width(Word) + SpaceWidth)
				spaceLeft = spaceLeft - (word.Length + 1);
			}
			index += word.Length + 1; // the length of the word plus the whitespace (newline or space)
		}
		return new string( orig_arr);
	}

	void AddLineToBuffer( string t)
	{
		t = BetterWordWrap(t);
		foreach( string sp in t.Split ('\n'))
		{
			scrollBuffer.Enqueue( sp );
		}

		//check to see if we've got too much scroll back and drop a line
		while( scrollBuffer.Count > maxBufferLength )
		{
			scrollBuffer.Dequeue();
		}

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
		string firstWord = inp.Split(' ')[0];

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
			//if(Random.value < 0.5f) // 50% chance of finding hidden stuff
			//{
				if( rm.currentRoom.hiddenItems.Count > 0)
				{
					string itemFound = rm.currentRoom.hiddenItems[0];
					response = "You find a " + itemFound;
					rm.currentRoom.items.Add(itemFound);
					rm.currentRoom.hiddenItems.Remove(itemFound);
				} else
					response = "There doesn't seem to be anything else hidden in this room.";
			//} else
			//	response = "You didn't find anything this time.";
		} else if( firstWord == "get")
		{
			string attemptedItem = inp.Substring( firstWord.Length + 1);
			if( rm.currentRoom.items.Contains(attemptedItem))
			{
				rm.currentRoom.items.Remove(attemptedItem);
				response = "You picked up the " + attemptedItem + ", but it immediately turned to dust because there is no inventory system yet!";
			} else {
				response = "There is no " + attemptedItem + " in this room.";
			}
		} else {
			response = "What?";
		}

		// add what the user entered as a 'command'
		AddLineToBuffer( "> " + inp);
		// then add our response
		AddLineToBuffer(response);
	}

}
