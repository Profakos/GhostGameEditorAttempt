//base imports
using UnityEngine; 

//EditorUtility
using UnityEditor;

//imports for UI
using UnityEngine.EventSystems;
using UnityEngine.UI;

//imports for lists
using System.Collections;
using System.Collections.Generic;

namespace GhostGame 
{
	public class LevelBuilder : MonoBehaviour {

		//The script that saves and loads the data
		[HideInInspector]
		public LevelLoader loaderScript;

		//The panel for changing the level name, saving and loading levels
		public GameObject levelFileEditingPanel;
		public GameObject saveConfirmation;

		//The panel for editing rooms
		public GameObject roomEditingPanel;

		protected InputField roomNameInput;
		protected InputField roomWidthInput;
		protected InputField roomHeightInput;
		protected InputField roomXInput;
		protected InputField roomYInput;
		protected Dropdown roomTypeDropdown;

		protected GameObject createRoomButton;
		protected GameObject editRoomButton;
		protected GameObject deleteRoomButton;
		protected GameObject furnitureModeButton;

		//The panel for editing furnitures
		public GameObject furniturePanel;
		protected GameObject furnitureType;
		protected Dropdown furnitureTypeDropdown;
		protected GameObject createFurnitureButton;
		protected GameObject editFurnitureButton;
		protected GameObject deleteFurnitureButton;
		protected InputField furnitureNameInput;

		//The panel for editing borders
		public GameObject borderPanel;
		protected Dropdown borderTypeDropdown;
		protected GameObject borderType;
		protected GameObject createBorderButton;
		protected GameObject editBorderButton;
		protected GameObject deleteBorderButton;
		protected InputField borderWidthInput;
		protected InputField borderHeightInput;
		protected InputField borderXInput;
		protected InputField borderYInput;

		//Level editing cursor and values
		public GameObject editingBlueprint;
		protected SpriteRenderer editingBluePrintSpriteRenderer;
		protected GameObject currentlySelectedRoom;
		protected GameObject currentlySelectedFeature;
		  
		protected InputField worldWidthInput;
		protected InputField worldHeightInput;

		//ErrorMargin due to autotiling
		private float autotileErrorMargin = 0.03f;

		//The current mode
		private PanelType currentMode;
	  
		void Awake() 
		{
			loaderScript = GetComponent<LevelLoader>();

			SetupWorldSettings ();

			SetupRoomPanel ();

			SetupFurniturePanel ();

			SetupBorderPanel ();
		}

		// Use this for initialization
		void Start () 
		{
			SwitchPanel (PanelType.LevelPanel);

			editingBlueprint.transform.position = Vector3.zero;
			editingBluePrintSpriteRenderer.size = Vector2.one;

			var worldSize = loaderScript.cameraMovement.GetWorldSize();
			worldWidthInput.text = worldSize.x.ToString();
			worldHeightInput.text = worldSize.y.ToString();

			List<Dropdown.OptionData> floorOptions = new List<Dropdown.OptionData> ();
			List<Dropdown.OptionData> furnitureOptions = new List<Dropdown.OptionData> ();
			List<Dropdown.OptionData> borderOptions = new List<Dropdown.OptionData> ();

			List<GameObject> allPrefab = new List<GameObject> ();
			allPrefab.AddRange (loaderScript.GetPrefabsByTag("Border"));
			allPrefab.AddRange (loaderScript.GetPrefabsByTag("Floor"));
			allPrefab.AddRange (loaderScript.GetPrefabsByTag("Furniture"));

			allPrefab.Sort (Sorter.SortByName);

			foreach (var prefab in allPrefab)
			{
				var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
				var data = new Dropdown.OptionData (prefab.name, spriteRenderer.sprite);

				switch (prefab.tag)
				{
					case "Floor":
						floorOptions.Add (data);
						break;
					case "Furniture":
						furnitureOptions.Add (data);
						break;
					case "Border":
						borderOptions.Add (data);
						break;
				}
			}

			roomTypeDropdown.AddOptions (floorOptions);
			furnitureTypeDropdown.AddOptions (furnitureOptions);
			borderTypeDropdown.AddOptions (borderOptions);
		}
		
		// Update is called once per frame
		void Update () 
		{
	  	
			if (Input.GetMouseButtonDown (0))
			{
				if (EventSystem.current.IsPointerOverGameObject()) //not if clicked on UI
					return;
				
				SwitchPanel (PanelType.LevelPanel);
			}

			if (Input.GetMouseButtonDown (1))
			{
				if (EventSystem.current.IsPointerOverGameObject()) //not if clicked on UI
					return;

				Vector3 screenToWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				if (currentMode == PanelType.CreateBorderPanel)
				{
					SetBlueprint (screenToWorldPosition, DataType.Border);
					return;
				}
 
				RaycastHit2D hit = Physics2D.Raycast(screenToWorldPosition, Vector2.zero);

				if (hit.collider != null)
				{
					switch (hit.collider.tag)
					{
						case "Floor":
							SelectRoom (hit.collider.gameObject);
							break;
						case "Furniture":
							SelectFurniture (hit.collider.gameObject);
							break;
						case "Border":
							SelectBorder (hit.collider.gameObject);
							break;
					}
				} 
				else
				{
					PrepareCreateRoom (screenToWorldPosition);
				}
			}
		}

		//Collects the references of the various UI elements in the editorPanel
		private void SetupRoomPanel()
		{
			editingBluePrintSpriteRenderer = editingBlueprint.GetComponent<SpriteRenderer> ();

			var nameInputTransform = roomEditingPanel.transform.Find ("Name/InputField");
			if (nameInputTransform)
			{
				roomNameInput = nameInputTransform.gameObject.GetComponent<InputField> ();
			}

			var widthInputTransform = roomEditingPanel.transform.Find ("Width/InputField");
			if (widthInputTransform)
			{
				roomWidthInput = widthInputTransform.gameObject.GetComponent<InputField> ();
			}

			var heightInputTransform = roomEditingPanel.transform.Find ("Height/InputField");
			if (heightInputTransform)
			{
				roomHeightInput = heightInputTransform.gameObject.GetComponent<InputField> ();
			}

			var xInputTransform = roomEditingPanel.transform.Find ("X/InputField");
			if (xInputTransform)
			{
				roomXInput = xInputTransform.gameObject.GetComponent<InputField> ();
			}

			var yInputTransform = roomEditingPanel.transform.Find ("Y/InputField");
			if (yInputTransform)
			{
				roomYInput = yInputTransform.gameObject.GetComponent<InputField> ();
			}

			var dropdownTransform = roomEditingPanel.transform.Find ("Sprite/Dropdown");
			if (dropdownTransform)
			{
				roomTypeDropdown = dropdownTransform.gameObject.GetComponent<Dropdown> ();
			}

			var createRoomButtonTransform = roomEditingPanel.transform.Find ("CreateRoomButton");
			if (createRoomButtonTransform)
			{
				createRoomButton = createRoomButtonTransform.gameObject;
			}

			var editRoomButtonTransform = roomEditingPanel.transform.Find ("EditRoomButton");
			if (editRoomButtonTransform)
			{
				editRoomButton = editRoomButtonTransform.gameObject;
			}

			var furnitureModeButtonTransform = roomEditingPanel.transform.Find ("FurnitureModeButton");
			if (furnitureModeButtonTransform)
			{
				furnitureModeButton = furnitureModeButtonTransform.gameObject;
			}

			var deleteRoomButtonTransform = roomEditingPanel.transform.Find ("DeleteRoomButton");
			if (deleteRoomButtonTransform)
			{
				deleteRoomButton = deleteRoomButtonTransform.gameObject;
			}
		}

		//Collects the references of the various UI elements in the furniturePanel
		private void SetupFurniturePanel()
		{
			var dropdownTransform = furniturePanel.transform.Find ("Furniture/Dropdown");
			if (dropdownTransform)
			{
				furnitureTypeDropdown = dropdownTransform.gameObject.GetComponent<Dropdown> ();
			}

			var furnitureTypeTransform = furniturePanel.transform.Find ("Furniture");
			if (furnitureTypeTransform)
			{
				furnitureType = furnitureTypeTransform.gameObject;
			}

			var createFurnitureButtonTransform = furniturePanel.transform.Find ("CreateFurniture");
			if (createFurnitureButtonTransform)
			{
				createFurnitureButton = createFurnitureButtonTransform.gameObject;
			}

			var editFurnitureButtonTransform = furniturePanel.transform.Find ("EditFurniture");
			if (editFurnitureButtonTransform)
			{
				editFurnitureButton = editFurnitureButtonTransform.gameObject;
			}		

			var deleteFurnitureButtonTransform = furniturePanel.transform.Find ("DeleteFurniture");
			if (deleteFurnitureButtonTransform)
			{
				deleteFurnitureButton = deleteFurnitureButtonTransform.gameObject;
			}

			var furnitureNameInputTransform = furniturePanel.transform.Find ("Name/InputField");
			if (furnitureNameInputTransform)
			{
				furnitureNameInput = furnitureNameInputTransform.gameObject.GetComponent<InputField> ();
			}
		}

		//Sets up the connection to the world settings components, and related items
		private void SetupWorldSettings ()
		{ 
			var worldWidthInputTransform = levelFileEditingPanel.transform.Find ("Width/InputField");
			if (worldWidthInputTransform)
			{
				worldWidthInput = worldWidthInputTransform.gameObject.GetComponent<InputField> ();
			}

			var worldHeightInputTransform = levelFileEditingPanel.transform.Find ("Height/InputField");
			if (worldHeightInputTransform)
			{
				worldHeightInput = worldHeightInputTransform.gameObject.GetComponent<InputField> ();
			}
		}

		//Sets up the the references of the various UI elements of the borderPanel
		public void SetupBorderPanel()
		{
			var dropdownTransform = borderPanel.transform.Find ("Border/Dropdown");
			if (dropdownTransform)
			{
				borderTypeDropdown = dropdownTransform.gameObject.GetComponent<Dropdown> ();
			}

			var furnitureTypeTransform = borderPanel.transform.Find ("Border");
			if (furnitureTypeTransform)
			{
				borderType = furnitureTypeTransform.gameObject;
			}		

			var createBorderButtonTransform = borderPanel.transform.Find ("CreateBorder");
			if (createBorderButtonTransform)
			{
				createBorderButton = createBorderButtonTransform.gameObject;
			}

			var editBorderButtonTransform = borderPanel.transform.Find ("EditBorder");
			if (editBorderButtonTransform)
			{
				editBorderButton = editBorderButtonTransform.gameObject;
			}		

			var deleteBorderButtonTransform = borderPanel.transform.Find ("DeleteBorder");
			if (deleteBorderButtonTransform)
			{
				deleteBorderButton = deleteBorderButtonTransform.gameObject;
			}

			var xInputTransform = borderPanel.transform.Find ("X/InputField");
			if (xInputTransform)
			{
				borderXInput = xInputTransform.gameObject.GetComponent<InputField> ();
			}

			var yInputTransform = borderPanel.transform.Find ("Y/InputField");
			if (yInputTransform)
			{
				borderYInput = yInputTransform.gameObject.GetComponent<InputField> ();
			}

			var widthTransform = borderPanel.transform.Find ("Width/InputField");
			if (widthTransform)
			{
				borderWidthInput = widthTransform.gameObject.GetComponent<InputField> ();
			}

			var heightTransform = borderPanel.transform.Find ("Height/InputField");
			if (heightTransform)
			{
				borderHeightInput = heightTransform.gameObject.GetComponent<InputField> ();
			}
		}

		//Updates the world imputs after loading
		public void UpdateWorldInputs(float x, float y)
		{
			worldHeightInput.text = y.ToString ();
			worldWidthInput.text = x.ToString ();
		}

		//Called by changing the world width value in the editor panel
		public void SetWorldWidth(string x)
		{
			float newX = 0f;

			if (!float.TryParse (x, out newX))
				return;

			loaderScript.SetWorldWidth (newX);
		}

		//Called by changing the world height value in the editor panel
		public void SetWorldHeight(string y)
		{
			float newY = 0f;

			if (!float.TryParse (y, out newY))
				return;

			loaderScript.SetWorldHeight (newY);
		}

		//Called by cancelling saving the level
		public void OpenLevelPanel()
		{
			SwitchPanel (PanelType.LevelPanel);
		}

		//Called by pressing the Save button
		public void SaveConfirmation()
		{
			saveConfirmation.SetActive (true);
		}

		//Resets and shows the room creating panel, with all fields empty
		private void PrepareCreateRoom(Vector2 position)
		{
			SwitchPanel (PanelType.CreateRoomPanel);

			var blueprintPosition = new Vector3 (Mathf.Round (position.x), Mathf.Round (position.y), 0);

			editingBlueprint.transform.position = blueprintPosition;

			var blueprintSize = Vector2.one;
			editingBluePrintSpriteRenderer.size =  blueprintSize;

			if(roomNameInput) 
			{ 
				roomNameInput.text = string.Empty; 
			}

			if (roomHeightInput)
			{
				roomHeightInput.text = blueprintSize.x.ToString();
			}

			if (roomWidthInput)
			{
				roomWidthInput.text = blueprintSize.y.ToString();
			}

			if (roomXInput)
			{
				roomXInput.text = blueprintPosition.x.ToString ();
			}

			if (roomYInput)
			{
				roomYInput.text = blueprintPosition.y.ToString ();
			}

			if (roomTypeDropdown)
			{
				roomTypeDropdown.value = 0;
				roomTypeDropdown.RefreshShownValue ();
			} 
		}
			
		//Called by changing the room width value in the editor panel
		public void SetRoomWidth(string x)
		{
			var blueprintSize = editingBluePrintSpriteRenderer.size;
			if (!float.TryParse (x, out blueprintSize.x))
				return;

			editingBluePrintSpriteRenderer.size = blueprintSize;
		}

		//Called by changing the room height value in the editor panel
		public void SetRoomHeight(string y)
		{
			var blueprintSize = editingBluePrintSpriteRenderer.size;
			if (!float.TryParse (y, out blueprintSize.y))
				return;

			editingBluePrintSpriteRenderer.size = blueprintSize;
		}

		//Called by changing the room's x coordinate in the editor panel
		public void SetRoomX(string x)
		{
			var blueprintPosition = editingBlueprint.transform.position;
			if (!float.TryParse (x, out blueprintPosition.x))
				return; 

			if(!IsBlueprintPositionValid(DataType.Floor, new Vector2(blueprintPosition.x - editingBlueprint.transform.position.x, 0)))
			{ 
				blueprintPosition = editingBlueprint.transform.position;

				roomXInput.text = blueprintPosition.x.ToString();
				roomYInput.text = blueprintPosition.y.ToString();

				return;
			}

			editingBlueprint.transform.position = blueprintPosition;
		}

		//Called by changing the room's Y coordinate in the editor panel
		public void SetRoomY(string y)
		{
			var blueprintPosition = editingBlueprint.transform.position;
			if (!float.TryParse (y, out blueprintPosition.y))
				return;

			if(!IsBlueprintPositionValid(DataType.Floor,  new Vector2(0, blueprintPosition.y- editingBlueprint.transform.position.y)))
			{ 
				blueprintPosition = editingBlueprint.transform.position;

				roomXInput.text = blueprintPosition.x.ToString();
				roomYInput.text = blueprintPosition.y.ToString();

				return;
			}

			editingBlueprint.transform.position = blueprintPosition;
		}

		//Sets up the room editor panel based on the room we rightclicked on, filling out the editor panel with its data
		private void SelectRoom(GameObject hitObject)
		{ 
			SwitchPanel (PanelType.EditRoomPanel);

			var hitSpriteRenderer = hitObject.GetComponent<SpriteRenderer> ();

			currentlySelectedRoom = hitObject;
			roomNameInput.text = hitObject.name;
			roomWidthInput.text = hitSpriteRenderer.size.x.ToString();
			roomHeightInput.text = hitSpriteRenderer.size.y.ToString();
			roomXInput.text = hitObject.transform.position.x.ToString();
			roomYInput.text = hitObject.transform.position.y.ToString();

			var spriteName = hitSpriteRenderer.sprite.name;
			var dropdownValue = roomTypeDropdown.options.FindIndex (m => m.text == spriteName);
			roomTypeDropdown.value = dropdownValue >= 0 ? dropdownValue : 0;
			roomTypeDropdown.RefreshShownValue (); 

			editingBlueprint.transform.position = hitObject.transform.position;
			editingBluePrintSpriteRenderer.size = hitSpriteRenderer.size;
		}

		//Sets up the furniture editor panel based on the room we rightclicked on, filling out the editor panel with its data
		private void SelectFurniture(GameObject hitObject)
		{
			SwitchPanel (PanelType.EditFurniturePanel);		

			var hitBoxCollider = hitObject.GetComponent<BoxCollider2D> ();

			currentlySelectedFeature = hitObject;
			if (hitObject.transform.parent != null)
			{
				currentlySelectedRoom = hitObject.transform.parent.gameObject;
			}

			furnitureNameInput.text = currentlySelectedFeature.transform.name;

			editingBlueprint.transform.position = hitObject.transform.position;
			editingBluePrintSpriteRenderer.size = hitBoxCollider.size;

		}

		//Called by pressing the move north button in the editor panel
		public void MoveRoomNorth(string value)
		{ 
			MoveBlueprint (Direction.North, value, DataType.Floor);
		}

		//Called by pressing the move east button in the editor panel
		public void MoveRoomEast(string value)
		{
			MoveBlueprint (Direction.East, value, DataType.Floor);
		}

		//Called by pressing the move south button in the editor panel
		public void MoveRoomSouth(string value)
		{
			MoveBlueprint (Direction.South, value, DataType.Floor);
		}

		//Called by pressing the move west button in the editor panel
		public void MoveRoomWest(string value)
		{
			MoveBlueprint (Direction.West, value, DataType.Floor);
		}

		//Called by pressing the move north button in the editor panel
		public void MoveFurnitureNorth(string value)
		{ 
			MoveBlueprint (Direction.North, value, DataType.Furniture);
		}

		//Called by pressing the move east button in the editor panel
		public void MoveFurnitureEast(string value)
		{
			MoveBlueprint (Direction.East, value, DataType.Furniture);
		}

		//Called by pressing the move south button in the editor panel
		public void MoveFurnitureSouth(string value)
		{
			MoveBlueprint (Direction.South, value, DataType.Furniture);
		}

		//Called by pressing the move west button in the editor panel
		public void MoveFurnitureWest(string value)
		{
			MoveBlueprint (Direction.West, value, DataType.Furniture);
		}

		//Called by pressing the move north button in the editor panel
		public void MoveBorderNorth(string value)
		{ 
			MoveBlueprint (Direction.North, value, DataType.Border);
		}

		//Called by pressing the move east button in the editor panel
		public void MoveBorderEast(string value)
		{
			MoveBlueprint (Direction.East, value, DataType.Border);
		}

		//Called by pressing the move south button in the editor panel
		public void MoveBorderSouth(string value)
		{
			MoveBlueprint (Direction.South, value, DataType.Border);
		}

		//Called by pressing the move west button in the editor panel
		public void MoveBorderWest(string value)
		{
			MoveBlueprint (Direction.West, value, DataType.Border);
		}

		//Moves the location of the blueprints cursor
		public void MoveBlueprint(Direction dir, string value, DataType dataType)
		{
			float parsedValue = 0f;

			if (!float.TryParse (value, out parsedValue))
			{
				return;
			}

			MoveBlueprint (dir, parsedValue, dataType);
		}
	 
		//Moves the location of the blueprint cursor
		private void MoveBlueprint(Direction dir, float value, DataType dataType)
		{
			Vector2 offSet = Vector2.zero;

			switch (dir)
			{
				case Direction.North: 
					offSet.y += value;
					break;

				case Direction.East:
					offSet.x += value;
					break;

				case Direction.South: 
					offSet.y -= value;
					break;

				case Direction.West:
					offSet.x -= value;
					break;

				default:
					return; 
			}

			if(currentMode != PanelType.CreateFurniturePanel && !IsBlueprintPositionValid(dataType, offSet))
			{ 
				return;
			}

			var blueprintPosition = editingBlueprint.transform.position;

			blueprintPosition.x += offSet.x;
			blueprintPosition.y += offSet.y;

			if (DataType.Floor == dataType)
			{
				roomXInput.text = blueprintPosition.x.ToString ();
				roomYInput.text = blueprintPosition.y.ToString ();
			} else if (DataType.Border == dataType)
			{
				borderXInput.text = blueprintPosition.x.ToString ();
				borderYInput.text = blueprintPosition.y.ToString ();
			}
	 
			editingBlueprint.transform.position = blueprintPosition;
		}

		//checks if the new area of the blueprint is acceptable
		//offset is if you want to check if the cursor can be moved before you move it
		//otherwise it checks if the current place is okay or not
		public bool IsBlueprintPositionValid(DataType dT, Vector2 offset)
		{
			switch(dT)
			{
				case DataType.Floor:
					if (currentlySelectedRoom == null)
					{
						if (CheckOverlap (DataType.Floor, offset))
						{
							EditorUtility.DisplayDialog ("Error", "Overlap with an existing room", "OK");
							return false;
						};					
					} 
					else
					{
						if (CheckOverlap (currentlySelectedRoom, offset))
						{
							EditorUtility.DisplayDialog ("Error", "Overlap with an existing room", "OK");
							return false;
						};
					}


					break;

				case DataType.Furniture:

					if (!IsFurnitureInRoom (offset))
					{
						EditorUtility.DisplayDialog ("Error", "Furniture would be out of bounds.", "OK");
						return false;
					}

					if (currentlySelectedFeature == null)
					{
						if (CheckOverlap (DataType.Furniture, offset))
						{
							EditorUtility.DisplayDialog ("Error", "Overlap with an existing furniture", "OK");
							return false;
						};					
					} 
					else
					{
						if (CheckOverlap (currentlySelectedFeature, offset))
						{
							EditorUtility.DisplayDialog ("Error", "Overlap with an existing furniture", "OK");
							return false;
						};							
					}

					break;
			}

			return true;
		}

		//Sets the blueprint's position directly, to 0.5 unit precision
		private void SetBlueprint (Vector3 position, DataType dT) {
				var x = position.x;
				var y = position.y;

				x = Mathf.Floor (x * 2f) * 0.5f;
				y = Mathf.Floor (y * 2f) * 0.5f;

				editingBlueprint.transform.position = new Vector2(x,y);

			if (dT == DataType.Border)
			{
				borderXInput.text = x.ToString ();
				borderYInput.text = y.ToString ();
			}
		}

		//Called by the CreateRoom button
		//Spawns a room based on the selected Floor prefab, if all conditions are acceptable
		public void CreateRoom()
		{
			if (string.IsNullOrEmpty (roomNameInput.text))
			{
				EditorUtility.DisplayDialog ("Error", "Room name is empty", "OK"); 
				return;
			}

			if (loaderScript == null)
			{
				EditorUtility.DisplayDialog ("Error", "Can not find loading script", "OK"); 
				return;
			}

			GameObject prefab = loaderScript.GetPrefabsByTag("Floor").Find (m => m.name == roomTypeDropdown.captionText.text); 
	 
			if (prefab == null)
			{
				EditorUtility.DisplayDialog ("Error", "No prefab found for room dropdown", "OK");
				return;
			}

			if (!IsBlueprintPositionValid(DataType.Floor, Vector2.zero))
			{
				return;
			}

			if (!IsNameUnique (roomNameInput.text, DataType.Floor))
			{
				EditorUtility.DisplayDialog ("Error", "Room name is not unique", "OK");
				return;
			}

			var blueprintPosition = editingBlueprint.transform.position;

			Vector3 spawnPosition = new Vector3 ((blueprintPosition.x), (blueprintPosition.y), 0);

			GameObject newRoom = GameObject.Instantiate (prefab, spawnPosition, Quaternion.identity, loaderScript.worldHolder.transform);

			AttachToWorldHolder (newRoom);

			newRoom.name = roomNameInput.text;

			Entity entity = newRoom.GetComponent<Entity> ();

			entity.prefabName = prefab.name;

			SpriteRenderer roomSpriteRenderer = newRoom.GetComponent<SpriteRenderer> ();

			roomSpriteRenderer.sprite = roomTypeDropdown.captionImage.sprite;

			roomSpriteRenderer.size = editingBluePrintSpriteRenderer.size;

			SwitchPanel (PanelType.LevelPanel);
		}
	 
		//Attaches game object to a specific layer of the worldHolder
		public void AttachToWorldHolder(GameObject gO)
		{
			var layerName = LayerMask.LayerToName (gO.layer);

			var layerObject = loaderScript.worldHolder.transform.Find (layerName);

			if (layerObject != null)
			{
				gO.transform.parent = layerObject.transform;
			}
		}
			
		//Edits the selected room instance's variables. Called by the EditRoom button
		public void EditRoom()
		{
			if (currentlySelectedRoom == null)
			{
				EditorUtility.DisplayDialog ("Error", "No room selected to edit", "OK");
				return;
			}

			if (string.IsNullOrEmpty (roomNameInput.text))
			{
				EditorUtility.DisplayDialog ("Error", "Room name is empty", "OK");
				return;
			}

			if(!IsBlueprintPositionValid(DataType.Floor, Vector2.zero))
			{
				return;
			}

			if (!IsNameUnique (roomNameInput.text, currentlySelectedRoom))
			{
				EditorUtility.DisplayDialog ("Error", "Room name is not unique", "OK");
				return;
			}

			SpriteRenderer roomSpriteRenderer = currentlySelectedRoom.GetComponent<SpriteRenderer> ();

			if (currentlySelectedRoom.transform.childCount != 0 && (editingBluePrintSpriteRenderer.size.x != roomSpriteRenderer.size.x || editingBluePrintSpriteRenderer.size.y != roomSpriteRenderer.size.y))
			{
				EditorUtility.DisplayDialog ("Error", "Can not resize room with children", "OK");
				return;
			}

			currentlySelectedRoom.transform.position = new Vector3 ((editingBlueprint.transform.position.x), (editingBlueprint.transform.position.y), 0);

			currentlySelectedRoom.name = roomNameInput.text;
	   
			roomSpriteRenderer.size = editingBluePrintSpriteRenderer.size;
		}

		//called by DeleteRoomButton, deletes a room
		public void DeleteRoom()
		{
			Destroy (currentlySelectedRoom);
			currentlySelectedRoom = null;

			SwitchPanel(PanelType.LevelPanel);
		}

		//Tests if the selected object overlaps with any other of the same tag
		//Call this one if the gameObject parameter is going to be null, as you are not editing an existing item
		//but comparing to the blueprint
		private bool CheckOverlap(DataType dataType, Vector2 offset)
		{
			var tag = string.Empty;

			switch (dataType)
			{
				case DataType.Floor: 
					tag = "Floor";
					break;
				case DataType.Furniture:
					tag = "Furniture";
					break;
			}

			return CheckOverlap (null, tag, offset);
		}

		//Tests if the selected object overlaps with any other of the same tag
		//Call this one if the gameObject is not going to be null
		private bool CheckOverlap(GameObject gO, Vector2 offset)
		{
			return CheckOverlap (gO, gO.tag, offset);
		}

		//Tests if the selected object overlaps with any other of the same tag
		//Tag can be overriden if needed, such as when gO is null
		private bool CheckOverlap(GameObject gO, string tag, Vector2 offset)
		{
			var tagToCheck = gO == null ? tag : gO.tag;

			if (string.IsNullOrEmpty (tagToCheck))
			{
				return false;
			}

			var position = Vector2.zero;
			position.x = editingBlueprint.transform.position.x + offset.x;
			position.y = editingBlueprint.transform.position.y + offset.y;

			var size = editingBluePrintSpriteRenderer.size;

			Vector2 pointA = new Vector2 (position.x + size.x * 0.5f - autotileErrorMargin, position.y + size.y * 0.5f - autotileErrorMargin );
			Vector2 pointB = new Vector2 (position.x - size.x * 0.5f + autotileErrorMargin, position.y - size.y * 0.5f + autotileErrorMargin );
	 
			var colliders = Physics2D.OverlapAreaAll (pointA, pointB);

			if (colliders.Length == 0)
			{
				return false;
			}

			foreach (var c in colliders)
			{
				if (c.tag != tagToCheck)
				{
					continue;
				}

				if (c.gameObject == gO)
				{
					continue;
				}

				return true;
			} 

			return false;
		}
	 
		//Checks if the object's name is unique within a specific tag
		//Call this one if the gameObject is going to be null
		private bool IsNameUnique(string newName, DataType type)
		{
			var tag = string.Empty;

			switch (type)
			{
				case DataType.Floor: 
					tag = "Floor";
					break;
				case DataType.Furniture:
					tag = "Furniture";
						break;
			}

			return IsNameUnique (newName, null, tag);
		}	

		//Checks if the object's name is unique within a specific tag
		//Call this one if the gameObject is going to be not null, because you are editing an existing item
		private bool IsNameUnique(string newName, GameObject gO)
		{
			return IsNameUnique (newName, gO, gO.tag);
		}

		//Checks if the object's name is unique within a specific tag
		//Tag can be overriden if needed, such as when gO is null
		private bool IsNameUnique(string newName, GameObject gO, string tag)
		{
			var tagToCheck = gO == null ? tag : gO.tag;

			if (string.IsNullOrEmpty (tagToCheck))
			{
				return false;
			}

			List<GameObject> rooms = new List<GameObject> ();
			rooms.AddRange( GameObject.FindGameObjectsWithTag (tagToCheck));

			return !rooms.Find (m => m != gO && m.name == newName);
		}

		//Called by Back to room button, returning from the furniture menu to the selected room
		public void BackToRoom()
		{
			currentlySelectedFeature = null;
			SelectRoom (currentlySelectedRoom);
		}

		//Called by the Furniture Mode button
		public void OpenFurniturePanel()
		{
			SwitchPanel (PanelType.CreateFurniturePanel);
			PrepareCreateFurniture ();
		}

		//Resets and shows the furniture panel. Public, because it is also called by the furniture type dropdown
		//Resets and updates the blueprint with the sprite size of the selected prefab
		public void PrepareCreateFurniture()
		{
			if (currentlySelectedRoom == null) //if we are directly selecting an item, room selection is handled elsewhere
			{
				return;
			}

			currentlySelectedFeature = null;
			furnitureNameInput.text = string.Empty;

			var roomSpriteRenderer = currentlySelectedRoom.GetComponent<SpriteRenderer> ();
			var roomPosition = currentlySelectedRoom.transform.position;

			var bottomLeftRoom = new Vector2(roomPosition.x - roomSpriteRenderer.size.x * 0.5f, roomPosition.y - roomSpriteRenderer.size.y * 0.5f);

			GameObject prefab = loaderScript.GetPrefabsByTag("Furniture").Find (m => m.name == furnitureTypeDropdown.captionText.text);
			var boxCollider = prefab.GetComponent<BoxCollider2D> ();

			var blueprintSize = boxCollider.size;
			var blueprintPosition = bottomLeftRoom;

			blueprintPosition.x += blueprintSize.x * 0.5f;
			blueprintPosition.y += blueprintSize.y * 0.5f;

			editingBlueprint.transform.position = blueprintPosition;
			editingBluePrintSpriteRenderer.size = blueprintSize;
		}

		//Called by the CreateFurniture button
		public void CreateFurniture()
		{
			if (!CreateFurniture (currentlySelectedRoom))
			{
				return;
			}

			PrepareCreateFurniture ();
		}

		//Creates the selected furniture prefab
		private bool CreateFurniture(GameObject room)
		{
			GameObject prefab;

			prefab = loaderScript.GetPrefabsByTag("Furniture").Find (m => m.name == furnitureTypeDropdown.captionText.text); 	

			if (prefab == null)
			{
				EditorUtility.DisplayDialog ("Error", "No prefab found for furniture dropdown", "OK");
				return false;
			}	

			if(!IsBlueprintPositionValid(DataType.Furniture, Vector2.zero))
			{
				return false;
			}

			if (string.IsNullOrEmpty (furnitureNameInput.text))
			{
				EditorUtility.DisplayDialog ("Error", "Furniture name is empty", "OK");
				return false;
			}

			if (!IsNameUnique (furnitureNameInput.text, DataType.Furniture))
			{
				EditorUtility.DisplayDialog ("Error", "Furniture name is not unique", "OK");
				return false;
			}
				
			GameObject newFurniture = GameObject.Instantiate (prefab, editingBlueprint.transform.position, Quaternion.identity, loaderScript.worldHolder.transform);

			newFurniture.transform.parent = room.transform;

			newFurniture.name = furnitureNameInput.text;

			Entity entity = newFurniture.GetComponent<Entity> ();

			entity.prefabName = prefab.name;

			furnitureNameInput.text = string.Empty;

			return true;
		}

		//Called by the Edit Furniture button, edits the data of the currently selected furniture
		public void EditFurniture()
		{	
			if (currentlySelectedFeature == null)
			{
				EditorUtility.DisplayDialog ("Error", "No furniture selected to edit", "OK");
				return;
			}

			if(!IsBlueprintPositionValid(DataType.Furniture, Vector2.zero))
			{
				return;
			}

			if (string.IsNullOrEmpty (furnitureNameInput.text))
			{
				EditorUtility.DisplayDialog ("Error", "Furniture name is empty", "OK");
				return;
			}

			if (!IsNameUnique (furnitureNameInput.text, currentlySelectedFeature))
			{
				EditorUtility.DisplayDialog ("Error", "Furniture name is not unique", "OK");
				return;
			}

			var blueprintPosition = editingBlueprint.transform.position;

			currentlySelectedFeature.transform.position = new Vector3 ((blueprintPosition.x), (blueprintPosition.y), 0);

			currentlySelectedFeature.name = furnitureNameInput.text;

			return;
		}

		//Called by the delete furniture button
		public void DeleteFurniture()
		{
			if (currentlySelectedFeature != null)
			{
				Destroy (currentlySelectedFeature);
				currentlySelectedFeature = null;
			}

			BackToRoom ();
		}

		//Checks if the furniture's bounds fall within the room. Offset is for checking a new location.
		private bool IsFurnitureInRoom(Vector2 furnitureOffset)
		{
			if (currentlySelectedRoom == null)
			{
				EditorUtility.DisplayDialog ("Error", "Parent room not found.", "OK");
				return false;
			}

			var roomSpriteRenderer = currentlySelectedRoom.GetComponent<SpriteRenderer> ();
			var roomPosition = currentlySelectedRoom.transform.position;
	 
			var topLeftRoom = new Vector2(roomPosition.x - roomSpriteRenderer.size.x * 0.5f, roomPosition.y + roomSpriteRenderer.size.y * 0.5f);
			var bottomRightRoom = new Vector2(roomPosition.x + roomSpriteRenderer.size.x * 0.5f, roomPosition.y - roomSpriteRenderer.size.y * 0.5f);

			var blueprintPosition = editingBlueprint.transform.position;
			var blueprintSize = editingBluePrintSpriteRenderer.size;

			var topLeftBlueprint = new Vector2(blueprintPosition.x - blueprintSize.x * 0.5f + furnitureOffset.x, blueprintPosition.y + blueprintSize.y * 0.5f + furnitureOffset.y);
			var bottomRightBlueprint = new Vector2(blueprintPosition.x + blueprintSize.x * 0.5f + furnitureOffset.x, blueprintPosition.y - blueprintSize.y * 0.5f + furnitureOffset.y);

			if (topLeftRoom.x <= topLeftBlueprint.x && topLeftRoom.y >= topLeftBlueprint.y
			   && bottomRightRoom.x >= bottomRightBlueprint.x && bottomRightRoom.y <= bottomRightBlueprint.y)
			{
				return true;
			}

			return false;
		}

		//Called by changing the border width value in the editor panel
		public void SetBorderWidth(string x)
		{
			var blueprintSize = editingBluePrintSpriteRenderer.size;
			if (!float.TryParse (x, out blueprintSize.x))
				return;

			editingBluePrintSpriteRenderer.size = blueprintSize;
		}

		//Called by changing the border height value in the editor panel
		public void SetBorderHeight(string y)
		{
			var blueprintSize = editingBluePrintSpriteRenderer.size;
			if (!float.TryParse (y, out blueprintSize.y))
				return;

			editingBluePrintSpriteRenderer.size = blueprintSize;
		}

		//Called by changing the border's x coordinate in the editor panel
		public void SetBorderX(string x)
		{
			var blueprintPosition = editingBlueprint.transform.position;
			if (!float.TryParse (x, out blueprintPosition.x))
				return; 

			if(!IsBlueprintPositionValid(DataType.Border, Vector2.zero))
			{ 
				blueprintPosition = editingBlueprint.transform.position;

				borderXInput.text = blueprintPosition.x.ToString();
				borderYInput.text = blueprintPosition.y.ToString();

				return;
			}

			editingBlueprint.transform.position = blueprintPosition;
		}

		//Called by changing the border's Y coordinate in the editor panel
		public void SetBorderY(string y)
		{
			var blueprintPosition = editingBlueprint.transform.position;
			if (!float.TryParse (y, out blueprintPosition.y))
				return;

			if(!IsBlueprintPositionValid(DataType.Border, Vector2.zero))
			{ 
				blueprintPosition = editingBlueprint.transform.position;

				borderXInput.text = blueprintPosition.x.ToString();
				borderXInput.text = blueprintPosition.y.ToString();

				return;
			}

			editingBlueprint.transform.position = blueprintPosition;
		}

		//Opens up the border panel, called by the Wall Mode button
		public void OpenBorderPanel()
		{
			SwitchPanel (PanelType.CreateBorderPanel);
			PrepareCreateBorder (Vector2.zero);			
		}

		public void PrepareCreateBorder()
		{
			PrepareCreateBorder (editingBlueprint.transform.position);
		}

		//Resets the border creation panel
		public void PrepareCreateBorder(Vector2 position)
		{
			GameObject prefab = loaderScript.GetPrefabsByTag("Border").Find (m => m.name == borderTypeDropdown.captionText.text);
			var boxCollider = prefab.GetComponent<BoxCollider2D> ();
 
			var blueprintSize = boxCollider.size;
			var blueprintPosition = Vector2.zero;

			//blueprintPosition.x += blueprintSize.x * 0.5f;
			//blueprintPosition.y += blueprintSize.y * 0.5f; 

			borderXInput.text = blueprintPosition.x.ToString();
			borderYInput.text = blueprintPosition.y.ToString();
			borderHeightInput.text = blueprintSize.y.ToString();
			borderWidthInput.text = blueprintSize.x.ToString();
 
			editingBlueprint.transform.position = blueprintPosition;
			editingBluePrintSpriteRenderer.size =  blueprintSize;	
		}

		//Called by the Create border button, attempts to spawn the selected border prefab  
		public void CreateBorder()
		{
			if (loaderScript == null)
			{
				EditorUtility.DisplayDialog ("Error", "Can not find loading script", "OK"); 
				return;
			}

			GameObject prefab = loaderScript.GetPrefabsByTag("Border").Find (m => m.name == borderTypeDropdown.captionText.text); 

			if (prefab == null)
			{
				EditorUtility.DisplayDialog ("Error", "No prefab found for room dropdown", "OK");
				return;
			}	

			var blueprintPosition = editingBlueprint.transform.position;

			Vector3 spawnPosition = new Vector3 ((blueprintPosition.x), (blueprintPosition.y), 0);

			GameObject newBorder = GameObject.Instantiate (prefab, spawnPosition, Quaternion.identity, loaderScript.worldHolder.transform);

			SpriteRenderer sprite = newBorder.GetComponent<SpriteRenderer> ();

			sprite.size = editingBluePrintSpriteRenderer.size;

			Entity entity = newBorder.GetComponent<Entity> ();

			entity.prefabName = prefab.name;

			AttachToWorldHolder (newBorder);

			SwitchPanel (PanelType.LevelPanel);			

		}

		//Called by clicking on a border item
		public void SelectBorder(GameObject gO)
		{
			SwitchPanel (PanelType.EditBorderPanel);

			var sprite = gO.GetComponent<SpriteRenderer> ();

			var blueprintSize = sprite.size;
			var blueprintPosition = gO.transform.position;

			borderXInput.text = blueprintPosition.x.ToString();
			borderYInput.text = blueprintPosition.y.ToString();
			borderHeightInput.text = blueprintSize.y.ToString();
			borderWidthInput.text = blueprintSize.x.ToString();

			editingBlueprint.transform.position = blueprintPosition;
			editingBluePrintSpriteRenderer.size =  blueprintSize;	

			currentlySelectedFeature = gO;
		}

		//Called by the Edit Border button
		public void EditBorder()
		{			
			SpriteRenderer spriteRenderer = currentlySelectedFeature.GetComponent<SpriteRenderer> ();

			spriteRenderer.size = editingBluePrintSpriteRenderer.size;

			currentlySelectedFeature.transform.position = new Vector3 ((editingBlueprint.transform.position.x), (editingBlueprint.transform.position.y), 0);
  
		}

		//Calld by the delete border button
		public void DeleteBorder()
		{
			Destroy (currentlySelectedFeature);
			currentlySelectedFeature = null;
			SwitchPanel (PanelType.LevelPanel);	
		}

		//Swaps the displayed panel and its components
		private void SwitchPanel(PanelType pT)
		{
			currentMode = pT;

			saveConfirmation.SetActive (false);

			levelFileEditingPanel.SetActive (pT == PanelType.LevelPanel);

			var showEditingBlueprint = false;

			if (pT == PanelType.CreateRoomPanel)
			{ 
				roomEditingPanel.SetActive (true);

				createRoomButton.SetActive (true);
				editRoomButton.SetActive (false);
				deleteRoomButton.SetActive (false);
				furnitureModeButton.SetActive (false);
				roomTypeDropdown.gameObject.SetActive (true);

				showEditingBlueprint = true;
			} 
			else if (pT == PanelType.EditRoomPanel)
			{ 
				roomEditingPanel.SetActive (true);

				createRoomButton.SetActive (false);
				editRoomButton.SetActive (true);
				deleteRoomButton.SetActive (true);
				furnitureModeButton.SetActive (true);
				roomTypeDropdown.gameObject.SetActive (false);

				showEditingBlueprint = true;
			}
			else
			{
				roomEditingPanel.SetActive (false);
			}

			if (pT == PanelType.CreateFurniturePanel)
			{
				furniturePanel.SetActive (true); 
				showEditingBlueprint = true;
				createFurnitureButton.SetActive (true);
				editFurnitureButton.SetActive (false);
				deleteFurnitureButton.SetActive (false);
				furnitureType.SetActive (true);
				showEditingBlueprint = true;
			} 
			else if (pT == PanelType.EditFurniturePanel)
			{
				furniturePanel.SetActive (true);
				createFurnitureButton.SetActive (false);
				editFurnitureButton.SetActive (true);
				deleteFurnitureButton.SetActive (true);
				furnitureType.SetActive (false);
				showEditingBlueprint = true;
			}
			else
			{
				furniturePanel.SetActive (false);
			}

			if (pT == PanelType.CreateBorderPanel)
			{
				borderPanel.SetActive (true);
				borderType.SetActive (true);
				createBorderButton.SetActive (true);
				editBorderButton.SetActive (false);
				showEditingBlueprint = true;
			}
			else if (pT == PanelType.EditBorderPanel)
			{
				borderPanel.SetActive(true);
				borderType.SetActive (false);
				createBorderButton.SetActive (false);
				editBorderButton.SetActive (true);
				showEditingBlueprint = true;
			}
			else
			{
				borderPanel.SetActive (false);
			}

			editingBlueprint.SetActive (showEditingBlueprint);
		}

		//Enum list of the panel types
		private enum PanelType
		{
			LevelPanel,
			CreateRoomPanel,
			EditRoomPanel,
			CreateFurniturePanel,
			EditFurniturePanel,
			CreateBorderPanel,
			EditBorderPanel
		}

	}
}