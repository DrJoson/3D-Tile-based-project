using UnityEngine;
using System.Collections;

public class TileMouseOver : MonoBehaviour {

	GameController gameController;

	public Color highlightColor;
	Color normalColor;

	void Start(){
		gameController = FindObjectOfType<GameController>();
		normalColor = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.color = normalColor;
		if(GameController.SelectedUnit){
			MouseOverTile ();
		}
	}
	
	void MouseOverTile ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		// output information from the raycast
		RaycastHit hitInfo;
		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			renderer.material.color = highlightColor;
		}
		else {
			renderer.material.color = normalColor;
		}
	}
	
	void OnMouseDown (){
		if(GameController.SelectedUnit){
			GameObject UnitToMove = GameController.SelectedUnit;
			// Finds out from Tile script whether or not the selected tile is passable
			bool tilePassable = GetComponent<Tile>().Passable;
			// If it is the selected unit moves to the selected tile's position
			if(tilePassable){
				UnitToMove.transform.position = transform.position;
				// The unit ends its movement and cannot be selected until next turn
				gameController.CompleteMovement();
				// The unit is then unselected
				GameController.SelectedUnit = null;
				renderer.material.color = normalColor;
			} else {
				print("This tile is not passable");
			}
		}
	}
}
