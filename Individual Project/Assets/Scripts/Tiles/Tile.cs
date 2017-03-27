using UnityEngine;
using System.Collections;

// for using in the inspector
[System.Serializable]
public class Tile {
	
	public string name;
	public GameObject tilePrefab;
	public bool passable = true;
	public float movementCost = 1;

}

