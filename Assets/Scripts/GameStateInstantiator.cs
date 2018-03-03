//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

namespace GhostGame 
{
	public class GameStateInstantiator : MonoBehaviour {
		
		//GameManager prefab to instantiate.
		public GameObject gameManager;          

		void Awake ()
		{
			//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
			if (GamestateManager.instance == null)

				//Instantiate gameManager prefab
				Instantiate(gameManager); 
		}
	} 

}