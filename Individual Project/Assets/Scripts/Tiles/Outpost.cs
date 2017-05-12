using UnityEngine;
using System.Collections;

public class Outpost : MonoBehaviour {
	
	public bool PlayerOwned = false;
	
	// Use this for initialization
	void Start () {
		GameController.OwnedOutpost = this;
	}
	
}
