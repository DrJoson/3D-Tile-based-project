using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (Unit))]
public class EnemyMouseOver : MonoBehaviour {
	
	Renderer bodyRenderer;
	
	Color normalColor;
	Color highlightColor;
	Color highlightOffset = new Color(0.2f,0.2f,0.2f,0f);
	
	void Start(){
		bodyRenderer = GetComponentInChildren<MeshRenderer>();
		normalColor = bodyRenderer.material.color;
		highlightColor = normalColor;
		highlightColor += highlightOffset;
	}
	
	void Update () {
		if(GameController.PlayerTurn && GameController.SelectedUnit){
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
		if(GameController.PlayerTurn && GameController.SelectedUnit){
			//Debug.Log("Currently selected player unit wants to attack this target!");
			bodyRenderer.material.color = normalColor;
			GameController.SelectedUnit.GetComponent<Unit>().SetTarget(this.gameObject.GetComponent<Unit>());
			bool target = true;
			GameController.MoveSelectedUnit((int)this.transform.position.x, (int)this.transform.position.z, target);
		}
	}
}
