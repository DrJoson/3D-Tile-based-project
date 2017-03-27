using UnityEngine;
using System.Collections;

public class UnitMouseOver : MonoBehaviour {

	bool UnitHasMoved;
	
	Renderer bodyRenderer;

	Color normalColor;
	Color highlightColor;
	Color highlightOffset = new Color(0.2f,0.2f,0.2f,0f);
	
	void Start(){
		bodyRenderer = GetComponentInChildren<MeshRenderer>();
		UnitHasMoved = GetComponent<PlayerUnit>().GetHasMoved();
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
		UnitHasMoved = GetComponent<PlayerUnit>().GetHasMoved();
		if(GameController.PlayerTurn && !GameController.SelectedUnit && !UnitHasMoved){
			GameController.SelectedUnit = this.gameObject;
			Debug.Log("Unit selected");
			GameController.ShowReachableTiles();
		} else {
			Debug.Log("This unit cannot be selected");
		}
	}

}
