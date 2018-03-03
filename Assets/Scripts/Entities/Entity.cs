//base imports
using UnityEngine;  

public class Entity : MonoBehaviour {

	public int sortingOffset = 0;
	public int precision = -10;

	[HideInInspector]
	public string prefabName;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	void Awake () 
	{

		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	//updates the draw order of the sprite
	void LateUpdate () 
	{
		spriteRenderer.sortingOrder = (int)(gameObject.transform.position.y * precision) + sortingOffset;
	}
}
