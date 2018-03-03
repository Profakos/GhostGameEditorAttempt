//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

//Used for navigating a room, by creating a grid based on the floor objects
//The grid will be navigated through A* pathfinding
namespace GhostGame 
{
	public class NavGrid
	{
		public int width;
		public int height;
		public NavNode[,] nodeArray;

		public NavGrid (int width, int height)
		{
			this.width = width;
			this.height = height;

			nodeArray = new NavNode [width , height];

			for (int col = 0; col < width; col++)
			{
				for (int row = 0; row < height; row++)
				{
					nodeArray [col, row] = new NavNode ();
				}
			}
		}

		public void AddHotSpot(int col, int row, Entity entity, Direction dir)
		{
			if (col > nodeArray.GetLength (0) || col < 0 || row > nodeArray.GetLength (0) || row < 0)
			{
				return;
			}

			nodeArray [col, row].AddHotSpot(entity, dir);
		}
	}
}

