//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;


namespace GhostGame 
{
	[System.Serializable]
	public class WorldData
	{
		public List<DataListItem> dataList;

		public WorldSettings worldSettings;

		public WorldData()
		{
			dataList = new List<DataListItem> ();
			worldSettings = new WorldSettings ();
		}
	 
		//starts collecting the saveable gameobjects
		public void CollectData(GameObject worldHolder)
		{
			CollectWorldSettings ();

			CollectType (DataType.Floor, worldHolder);
			CollectType (DataType.Furniture, worldHolder);
			CollectType (DataType.Border, worldHolder);
		}

		//Collects the world's settings
		private void CollectWorldSettings()
		{
			var mainCamera = GameObject.Find ("Main Camera");

			if (mainCamera == null)
			{
				worldSettings.width = 40f;
				worldSettings.height = 40f;
				return;
			}

			var cameraMovement = mainCamera.GetComponent<CameraMovement> ();

			if (cameraMovement == null)
			{
				worldSettings.width = 40f;
				worldSettings.height = 40f;
				return;			
			}

			var worldSize = cameraMovement.GetWorldSize ();

			worldSettings.width = worldSize.x;
			worldSettings.height = worldSize.y;
		}

		//collects and saves gameobject of a specific tag
		private void CollectType(DataType type, GameObject worldHolder)
		{
			string tag = string.Empty;

			switch (type)
			{
				case DataType.Floor:
					tag = "Floor";
					break;
				case DataType.Border: 
					tag = "Border";
					break;
				case DataType.Furniture:
					tag = "Furniture";
					break;
				default:
					return;
			}

			if (string.IsNullOrEmpty (tag))
				return;

			GameObject[] gameObjects;
			gameObjects = GameObject.FindGameObjectsWithTag(tag);

			foreach (GameObject gO in gameObjects)
			{
				var position = gO.transform.position;
				var spriteRenderer = gO.GetComponent<SpriteRenderer> ();

				var entity = gO.GetComponent<Entity> ();

				if (entity == null)
					continue;

				var parentName = GetParentPath (gO.transform.parent, worldHolder.transform);

				var item = new DataListItem (type, entity.prefabName , gO.name, position, gO.layer, spriteRenderer, parentName);
				dataList.Add (item);
			} 
		}

		public string GetParentPath (Transform tf, Transform topTransform)
		{
			if (tf.parent == null || tf.parent == topTransform)
			{
				return tf.name;
			}

			var parentPath = GetParentPath (tf.parent, topTransform);

			return parentPath + "/" + tf.name;
		}

		//creates the world from data
		public void LoadWorld(Dictionary<string,List<GameObject>> prefabDictionary, GameObject worldHolder)
		{
			LoadWorldSettings (worldHolder);

			LoadType (DataType.Floor, prefabDictionary, worldHolder);
			LoadType (DataType.Furniture, prefabDictionary, worldHolder);
			LoadType (DataType.Border, prefabDictionary, worldHolder);  
		}

		//loads the world's settings
		public void LoadWorldSettings(GameObject worldHolder)
		{
			if (worldSettings == null)
			{
				return;
			}

			var mainCamera = GameObject.Find ("Main Camera");

			if (mainCamera == null)
			{
				return;
			}

			var cameraMovement = mainCamera.GetComponent<CameraMovement> ();

			if (cameraMovement == null)
			{
				return;			
			}

			cameraMovement.SetWorldSize (worldSettings.width, worldSettings.height);

			var borderTransform = worldHolder.transform.Find ("WorldBorder");

			if (borderTransform != null)
			{
				var borderSpriteRenderer = borderTransform.GetComponent<SpriteRenderer> ();
				borderSpriteRenderer.size = cameraMovement.GetWorldSize ();
			}

			var levelEditor = GameObject.Find ("LevelEditorManager");
			if (levelEditor == null)
			{
				return;
			}

			var levelBuilderScript = levelEditor.GetComponent<LevelBuilder> ();
			if (levelBuilderScript == null)
			{
				return;
			}

			levelBuilderScript.UpdateWorldInputs (worldSettings.width, worldSettings.height);

		}

		//creates objects of a specific type from data
		private void LoadType(DataType type, Dictionary<string,List<GameObject>> prefabDictionary, GameObject worldHolder)
		{
			var itemWithType = dataList.FindAll (m => m.type == type);

			string tag = DataTypeToTag(type);

			if (string.IsNullOrEmpty (tag))
				return;

			if (!prefabDictionary.ContainsKey (tag) || prefabDictionary[tag] == null)
			{
				return;
			}

			var prefabList = prefabDictionary [tag];
			
			foreach (DataListItem item in itemWithType)
			{
				var prefab = prefabList.Find (m => m.name == item.prefabName);

				if (prefab == null)
					continue;

				var go = GameObject.Instantiate (prefab, new Vector2(item.x, item.y), Quaternion.identity);

				go.layer = item.layer;

				go.name = item.name;

				var entity = go.GetComponent<Entity> ();

				entity.prefabName = item.prefabName;
				 
				var spriteRenderer = go.GetComponent<SpriteRenderer> ();

				spriteRenderer.size = new Vector2 (item.spriteWidth, item.spriteHeight);

				if (string.IsNullOrEmpty (item.parentName))
				{ 
					go.transform.SetParent (worldHolder.transform);				
				} 
				else
				{
					var parentTransform = worldHolder.transform.Find (item.parentName);

					if (parentTransform != null)
					{
						go.transform.parent = parentTransform;
					}
				}
			}
		}

		//Converts dataType to gameObject tag
		private string DataTypeToTag(DataType type)
		{
			switch (type)
			{
				case DataType.Floor:
					return "Floor";
				case DataType.Border: 
					return "Border";
				case DataType.Furniture:
					return "Furniture";
				default:
					return string.Empty;
			}		
		}
	}

	[System.Serializable]
	public class DataListItem
	{
		public DataType type;

		public string prefabName;

		public float x;
		public float y;
		public float z;

		public string name;

		public float spriteWidth;
		public float spriteHeight;

		public int layer;

		public string parentName;

		public DataListItem(DataType type, string prefabName, string name, Vector3 position, int layer, SpriteRenderer spriteRenderer, string parentName)
		{
			this.type = type;
			this.prefabName = prefabName;

			this.name = name;
			this.x = position.x;
			this.y = position.y;
			this.z = position.z;

			this.layer = layer;

			this.spriteWidth = spriteRenderer.size.x;
			this.spriteHeight = spriteRenderer.size.y;

			this.parentName = parentName;
		}
	}

	[System.Serializable]
	public class WorldSettings
	{
		public float width;
		public float height;
	}

	[System.Serializable]
	public enum DataType
	{
		Floor,
		Furniture,
		Border
	}

}