//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

//Declare a hotspot of an object
//For example, a "computer","north" would mean a mortal could walk here, and use the computer to the north of this tile
namespace GhostGame 
{
	public class HotSpot
	{
		Entity entity;
		Direction direction;

		public HotSpot(Entity entity, Direction direction)
		{
			this.entity = entity;
			this.direction = direction;
		}
			
	}
}
