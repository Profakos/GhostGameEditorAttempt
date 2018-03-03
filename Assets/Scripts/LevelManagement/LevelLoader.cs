//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

//imports for fileoperations
using System.IO;

namespace GhostGame 
{		
	public class LevelLoader : MonoBehaviour {
	 
		//Dictionary of prefablists
		Dictionary<string, List<GameObject>> prefabDictionary = new Dictionary<string, List<GameObject>>();

		//hierarchy
		public GameObject worldHolder;

		public CameraMovement cameraMovement;

		public GameObject worldBorder;
		public SpriteRenderer worldBorderSpriteRenderer;

		//level data
		protected string levelName = "TestLevel";
		protected string folderName = "Levels";

		void Awake () 
		{
			var currentCamera = Camera.main;

			if (currentCamera)
			{
				cameraMovement = currentCamera.gameObject.GetComponent<CameraMovement> ();
			}

			var prefabCacheArray = gameObject.GetComponentsInChildren<PrefabCache> ();

			foreach (var prefabCache in prefabCacheArray)
			{
				if (!prefabDictionary.ContainsKey (prefabCache.tagType))
				{
					prefabDictionary.Add (prefabCache.tagType, new List<GameObject>());
				}

				prefabDictionary [prefabCache.tagType].AddRange (prefabCache.prefabList);
			}

			//
			foreach(PrefabCache go in prefabCacheArray)
			{
				Destroy(go.gameObject);
			}
		}
		
		// Use this for initialization
		void Start () 
		{
			SetupWorldHolder ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			
		}

		//Gets a prefab list from the prefab dictionary based on tag
		public List<GameObject> GetPrefabsByTag(string tag)
		{
			if (!prefabDictionary.ContainsKey (tag) || prefabDictionary [tag] == null)
			{
				return new List<GameObject>();
			}

			return prefabDictionary [tag];

		}

		//Sets up the navigational grid of every room
		private void SetupGrids()
		{
			var rooms = GameObject.FindGameObjectsWithTag ("Floor");

			foreach (var room in rooms)
			{
				Floor floorScript = room.GetComponent<Floor> ();

				if (floorScript == null)
				{
					continue;
				}

				floorScript.SetupGrid ();
			}
		}

		//tidies up the world hierarchy
		private void SetupWorldHolder()
		{
			if (worldHolder != null) 
			{
				return;	
			}

			worldHolder = new GameObject();
			worldHolder.name = "WorldHolder";

			if (worldHolder == null)
			{
				worldHolder = new GameObject();
				worldHolder.name = "WorldHolder";
			}

			if (GameObject.Find ("Floor0") == null)
			{
				GameObject floor0 = new GameObject ("Floor0");
				floor0.transform.parent = worldHolder.transform;
			}

			if (GameObject.Find ("Floor1") == null)
			{
				GameObject floor0 = new GameObject ("Floor1");
				floor0.transform.parent = worldHolder.transform;
			}		

			if (GameObject.Find ("Floor2") == null)
			{
				GameObject floor0 = new GameObject ("Floor2");
				floor0.transform.parent = worldHolder.transform;
			}		

			if (GameObject.Find ("Floor3") == null)
			{
				GameObject floor0 = new GameObject ("Floor3");
				floor0.transform.parent = worldHolder.transform;
			}

			if (worldBorder != null)
			{
				worldBorderSpriteRenderer = worldBorder.GetComponent<SpriteRenderer>();

				worldBorderSpriteRenderer.size = cameraMovement.GetWorldSize ();
			}
		}
			
		//handles the room renaming event
		public void SetLevelName(string name)
		{
			levelName = name;
		}

		//sets the world's width
		public void SetWorldWidth(float newX)
		{
			cameraMovement.SetWorldWidth (newX);

			if (worldBorderSpriteRenderer == null)
			{
				return;
			}

			worldBorderSpriteRenderer.size = cameraMovement.GetWorldSize ();

		}

		//sets the world's height
		public void SetWorldHeight(float newY)
		{	
			cameraMovement.SetWorldHeight (newY);

			if (worldBorderSpriteRenderer == null)
			{
				return;
			}

			worldBorderSpriteRenderer.size = cameraMovement.GetWorldSize ();
		}

		//formats the level's path
		protected string GetLevelPath()
		{
			return string.Format ("{0}/{1}/{2}.json", Application.dataPath, folderName, levelName);
		}

		//checks if the level's name is valid for a filename
		protected bool ValidateLevelName()
		{
			if (string.IsNullOrEmpty (levelName)) 
			{
				Debug.Log ("No level name given");
				return false;
			}

			foreach (char invalidChar in Path.GetInvalidFileNameChars()) 
			{
				if (levelName.Contains(invalidChar.ToString())) 
				{
					Debug.Log ("Invalid level name");
					return false;
				}
			}

			return true;
		}

		//handles the level loading event
		public void LoadLevel()
		{
			if (!ValidateLevelName ()) 
			{
				return;
			}
	  
			ClearLevel ();

			string path = GetLevelPath();

			string jsonString = string.Empty;

			using (StreamReader sr = new StreamReader(path)) 
			{
				string line; 
				while ((line = sr.ReadLine()) != null) 
				{
					jsonString += line;
				}
			}

			WorldData loadedData = JsonUtility.FromJson<WorldData>(jsonString);

			if (loadedData == null)
				return;

			SetupWorldHolder ();

			loadedData.LoadWorld (prefabDictionary, worldHolder);	
		}

		//sets up the technical data, like navigation
		public void ProcessLevel()
		{
			SetupGrids ();
		}

		//empties the level of gameobjects
		public void ClearLevel()
		{
			ClearLevelByTag ("Border");
			ClearLevelByTag ("Furniture");
			ClearLevelByTag ("Floor");
		}

		//empties the level of gameobjects with a specific tag
		public void ClearLevelByTag(string tag)
		{
			GameObject[] gameObjects;

			gameObjects = GameObject.FindGameObjectsWithTag(tag);
			foreach (var gO in gameObjects)
			{
				gO.transform.parent = null;
				Destroy (gO);
			}
		}
 
	}
}
