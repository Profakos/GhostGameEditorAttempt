//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

namespace GhostGame 
{
	//stores references to specific prefabs
	public class PrefabCache : MonoBehaviour 
	{
		public string tagType;
		
		public List<GameObject> prefabList = new List<GameObject> ();

		void Awake()
		{
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}