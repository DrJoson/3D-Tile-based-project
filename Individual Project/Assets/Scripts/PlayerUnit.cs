using UnityEngine;
using System.Collections;

public class PlayerUnit : MonoBehaviour {

	bool HasMoved;

	// Use this for initialization
	void Start () {
		HasMoved = false;
	}
	
	public void Moved(){
		HasMoved = true;
		Debug.Log("Player unit has finished its movement");
	}
	
	public bool GetHasMoved(){
		return HasMoved;
	}
	
	
}
