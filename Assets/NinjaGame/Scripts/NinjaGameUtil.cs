using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class NinjaGameUtil : MonoBehaviour {
	public List<String> levelsList;

	string LEVEL_PATH  ="NinjaGame/Levels";
		
	// Use this for initialization
	void Start () {
		levelsList = FindLevels();
		
	}
	
	
	List<String> FindLevels()
	{
		
		string[] levels;
			//Get all the scene files in the directory Assets/Levels
		levels=Directory.GetFiles(Application.dataPath + "/" + LEVEL_PATH +"/","*.unity");
		foreach(var level in levels)
		{
			if (levelsList!=null)
				levelsList.Add(level);
			Debug.Log(level);
		}
		return levelsList;
	}
	
}
