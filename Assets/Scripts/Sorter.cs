//base imports
using UnityEngine; 

//imports for lists
using System.Collections;
using System.Collections.Generic;

namespace GhostGame
{
	public static class Sorter
	{

		public static int SortByName(GameObject x, GameObject y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0; //equals
				}

				return -1; //y is greater

			} 
			else
			{
				if (y == null)
				{
					return 1; //x is greater
				}

				return x.name.CompareTo(y.name);

			}
		}
	}
}

