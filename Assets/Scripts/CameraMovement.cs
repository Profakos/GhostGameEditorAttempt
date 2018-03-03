//base imports
using UnityEngine;  

namespace GhostGame 
{
	public class CameraMovement : MonoBehaviour {

		float xRadius = 20f;
		float yRadius = 20f;

		float speed = 40;

		//Gets the size of the world border in a vector
		public Vector2 GetWorldSize()
		{
			return new Vector2( xRadius * 2f, yRadius * 2f);
		}	

		//Sets the size of the world border in a vector
		public void SetWorldSize(Vector2 vector)
		{
			SetWorldSize (vector.x, vector.y);
		}

		//Sets the width of the world border
		public void SetWorldWidth(float x)
		{
			SetWorldSize (x, yRadius * 2f);
		}

		//Sets the size of the height border
		public void SetWorldHeight(float y)
		{
			SetWorldSize (xRadius * 2f, y);
		}

		//Sets the size of the world border in a vector
		public void SetWorldSize(float width, float height)
		{
			xRadius = width * 0.5f;
			yRadius = height * 0.5f;
		}

		// Use this for initialization
		void Start () 
		{
		
		}
		
		// Update is called once per frame
		void Update () 
		{

			if (Camera.current == null)
				return;

			float xAxisValue = Input.GetAxis ("Horizontal");
			float yAxisValue = Input.GetAxis ("Vertical");

			Vector2 cameraPosition = Camera.current.transform.position;

			if (cameraPosition.x + xAxisValue > xRadius)
			{
				xAxisValue = xRadius - cameraPosition.x;
			}


			if (cameraPosition.x + xAxisValue < (-xRadius) )
			{ 
				xAxisValue = - (xRadius + cameraPosition.x);
			}


			if (cameraPosition.y + yAxisValue > yRadius)
			{
				yAxisValue = yRadius - cameraPosition.y;
			}

			if (cameraPosition.y + yAxisValue < -yRadius)
			{
				yAxisValue =  - (yRadius + cameraPosition.y);
			}

			var translation = speed * Time.deltaTime;

			Camera.current.transform.Translate (new Vector2 (xAxisValue * translation, yAxisValue * translation));

		}
	}
}