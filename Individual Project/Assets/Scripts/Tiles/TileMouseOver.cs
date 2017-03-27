using UnityEngine;
using System.Collections;

public class TileMouseOver : MonoBehaviour {

	void OnMouseDown (){
		if(GameController.PlayerTurn && GameController.SelectedUnit){ 
			if(this.transform.position != GameController.SelectedUnit.transform.position){
				// since a tile has been clicked on and not an enemy, the game controller is told that there is no 
				// enemy target at this position for the pathfinding to consider
				bool target = false;
				GameController.MoveSelectedUnit((int)this.transform.position.x, (int)this.transform.position.z, target);
			}
			else {
				Debug.Log("Already in this space");
			}
		}
	}
}
