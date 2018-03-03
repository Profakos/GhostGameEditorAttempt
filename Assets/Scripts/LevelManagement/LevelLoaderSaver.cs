//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

//imports for fileoperations
using System.IO;

namespace GhostGame 
{
	public class LevelLoaderSaver : LevelLoader {

		//handles the level saving event
		public void SaveLevel()
		{ 
			if (!ValidateLevelName ()) 
			{
				return;
			}

			WorldData worldData = new WorldData ();
			worldData.CollectData (worldHolder);

			string jsonString = JsonUtility.ToJson(worldData);

			string path = GetLevelPath(); 

			using (StreamWriter sw = new StreamWriter (path))
			{
				sw.WriteLine(jsonString);
				sw.Close();
			}

		}
	}
}