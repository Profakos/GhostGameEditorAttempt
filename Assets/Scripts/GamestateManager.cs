//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;


namespace GhostGame {
	public class GamestateManager : MonoBehaviour {

		//singleton instance
		public static GamestateManager instance = null;

		//loads the world
		private LevelLoader levelLoader;

		// Use this for initialization
		void Awake () 
		{

			if (instance == null) 
			{
				instance = this;
			} else if (instance != this) 
			{
				Destroy (gameObject);
			}

			DontDestroyOnLoad (gameObject); 

			levelLoader = gameObject.GetComponent<LevelLoader> (); 
		}

		// Use this for initialization
		void Start () 
		{ 
			if (levelLoader != null)
			{
				levelLoader.LoadLevel ();
				levelLoader.ProcessLevel ();
			}		
		}
		
		// Update is called once per frame
		void Update () 
		{
		
		}
	}	
}