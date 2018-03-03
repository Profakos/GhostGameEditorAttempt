//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

namespace GhostGame 
{

	public class Floor : MonoBehaviour {

		//behavioral data
		public bool outdoors = false;

		//dimension data
		public NavGrid navGrid;

		public SpriteRenderer sprite;

		void Awake(){
			sprite = GetComponent<SpriteRenderer> ();
		}
			
		// Use this for initialization
		void Start () {
			
		}
				
		// Update is called once per frame
		void Update () {
		}

		public void SetupGrid()
		{
			int width = (int) sprite.size.x;
			int height = (int) sprite.size.y;
			navGrid = new NavGrid (width, height);

		}
			
	}
}