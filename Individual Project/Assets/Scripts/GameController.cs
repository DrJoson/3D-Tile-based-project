using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	public static GameObject SelectedUnit;
	PlayerUnit playerUnit;
	public static bool PlayerTurn;
	
//	private static int TurnNumber;

	// Use this for initialization
	void Start () {
		SelectedUnit = null;
		PlayerTurn = true;
//		TurnNumber = 1;
		
	
	}
	
	// Update is called once per frame
	void Update () {
		// Deselects a unit if one is was selected
		if(SelectedUnit != null && Input.GetMouseButtonDown(1)){
			SelectedUnit = null;
			Debug.Log("Unit deselected");
		}
	}
	
	public void ReceivePlayerUnitInformation(PlayerUnit receivedUnit){
		playerUnit = receivedUnit;
		Debug.Log("Player unit information received by game controller");
	}
	
	public void CompleteMovement(){
		if(playerUnit){
		playerUnit.Moved();
		} else {
			Debug.LogError("No player unit assigned to game controller");
		}
	}
}
