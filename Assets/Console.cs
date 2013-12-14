﻿using UnityEngine;
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
			response = rm.currentRoom.description + "\n" + rm.currentRoom.GetExitOptions();
		} else if( inp == "north" && rm.currentRoom.exits.ContainsKey("north") )
		{
			rm.currentRoom = rm.currentRoom.exits["north"];
			response = rm.currentRoom.description + "\n" + rm.currentRoom.GetExitOptions();
		} else {
			response = "What?";
		}

		// add what the user entered as a 'command'
		AddLineToBuffer( "> " + inp);
		// then add our response
		AddLineToBuffer(response);

	}

}
