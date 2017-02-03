using UnityEngine;
using System.Collections;

public class UnitMouseOver : MonoBehaviour {

	GameController gameController;
	bool UnitHasMoved;
	
	Renderer bodyRenderer;

	public Color highlightColor;
	Color normalColor;
	
	void Start(){
		gameController = FindObjectOfType<GameController>();
		bodyRenderer = GetComponentInChildren<MeshRenderer>();
		UnitHasMoved = GetComponent<PlayerUnit>().GetHasMoved();
		normalColor = bodyRenderer.material.color;
	}
	
	// Update is called once per frame
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
			gameController.ReceivePlayerUnitInformation(GetComponent<PlayerUnit>());
		} else {
		Debug.Log("This unit cannot be selected");
		}
	}

}
