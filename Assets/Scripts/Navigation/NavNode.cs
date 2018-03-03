//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

//A collection of hotspots
namespace GhostGame
{
	public class NavNode
	{
		public List<HotSpot> hotSpotList = new List<HotSpot> ();

		public NavNode ()
		{
			
		}

		public void AddHotSpot(Entity entity, Direction dir)
		{
			hotSpotList.Add(new HotSpot (entity, dir));
		}
	}
}

