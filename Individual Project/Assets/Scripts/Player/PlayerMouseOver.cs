using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (Unit))]
public class PlayerMouseOver : MonoBehaviour {

	bool UnitHasMoved;
	
	Renderer bodyRenderer;
	
	GameController gc;
	Unit thisUnit;

	Color normalColor;
	Color highlightColor;
	Color highlightOffset = new Color(0.2f,0.2f,0.2f,0f);
	
	void Start(){
		gc = FindObjectOfType<GameController>();
		bodyRenderer = GetComponentInChildren<MeshRenderer>();
		thisUnit = GetComponent<Unit>();
		UnitHasMoved = thisUnit.GetHasMoved();
		normalColor = bodyRenderer.material.color;
		highlightColor = normalColor;
		highlightColor += highlightOffset;
	}
	
	void Update () {
		if(GameController.PlayerTurn && !GameController.SelectedUnit && !UnitHasMoved){
			MouseOverUnit();	
		}
	}

	void MouseOverUnit ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		// output information from the raycast
		RaycastHit hitInfo;
		if (collider.Raycast (ray, out hitInfo, Mathf.Infinity)) {
			bodyRenderer.material.color = highlightColor;
		}
		else {
			bodyRenderer.material.color = normalColor;
		}
	}
	
	void OnMouseDown(){
		if(GameController.PlayerTurn){
			UnitHasMoved = GetComponent<Unit>().GetHasMoved();
			if(!GameController.SelectedUnit && !UnitHasMoved){
				GameController.SelectedUnit = this.gameObject;
				Debug.Log("Unit selected");
				GameController.ShowReachableTiles();
			} 
			/*
			// deselecting player unit if it is already selected
			else if(GameController.SelectedUnit == this.gameObject){
				GameController.RemoveTileIndicators();
				GameController.SelectedUnit = null;
				Debug.Log("Unit deselected");
			}
			*/
			// staying and defending position
			else if(GameController.SelectedUnit == this.gameObject){
				GameController.RemoveTileIndicators();
				thisUnit.SetHasMoved(true);
				thisUnit.SetDefending(true);
				GameController.SelectedUnit = null;
				gc.EndTurn();
				Debug.Log("Unit defending position!");
			}
		}
	}

}
