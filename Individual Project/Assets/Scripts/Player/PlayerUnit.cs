using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUnit : MonoBehaviour {

	//public float health = 10;
	public float attackPower = 5;
	public float movementSpeed = 4;
	
	Pathfinding graph; // used to change position of unit
	Map map;  		   // used to find cost of moving from node unit is currently positioned at to next node in path
	//UnitOptions unitPopup;
	int tileX, tileZ;  // position of the unit in terms of tiles
	bool HasMoved;     // each unit can only move once per turn

	public List<Node> currentPath = null;
	
	void Start () {
		GameController.PlayerCount.Add(this);
		graph = FindObjectOfType<Pathfinding>();
		map = FindObjectOfType<Map>();
		//unitPopup = FindObjectOfType<UnitOptions>();
		//unitPopup = GameObject.FindWithTag("Popup");
		//unitPopup = GameObject.Find("Pop-up Canvas");
		
		tileX = (int)transform.position.x;
		tileZ = (int)transform.position.z;
		HasMoved = false;
	}
	
	void Update (){
		if(!HasMoved){
			if(currentPath != null){
				if(currentPath.Count >= 2){
					Vector3 nextPosition = graph.TileToWorldCoordinates(currentPath[1].x, currentPath[1].z);
					if(Vector3.Distance(transform.position, nextPosition) < 0.1f){
						// continue along path
						Move();
					}
					// change transform of unit
					float step = movementSpeed * Time.deltaTime;
					transform.position = Vector3.Lerp(transform.position, nextPosition, step);
				} 
				else if(GameController.SelectedTarget){
					AttackTarget();
				}
			}
		}
	}
	
	public void Move(){
		
		float remainingMovement = movementSpeed;
		//while(remainingMovement > 0){
		if(remainingMovement <= 0){
			return;
		}
		if(currentPath == null){
			return;
		}
		remainingMovement -= map.TileMovementCost(currentPath[1].x, currentPath[1].z);
		    
		// move unit to the position of the next node
		Vector3 nextPosition = graph.TileToWorldCoordinates(currentPath[1].x, currentPath[1].z);
		//float step = movementSpeed * Time.deltaTime;
		transform.position = nextPosition;
			
		//transform.position = Vector3.MoveTowards(transform.position, nextPosition, step); 
		//transform.position = Vector3.Lerp(transform.position, nextPosition, movementSpeed * Time.deltaTime);
		//StartCoroutine(MoveOverTime(this.gameObject, nextPosition, movementSpeed));


		// update the unit's tileX and tileZ parameters
		tileX = (int)transform.position.x;
		tileZ = (int)transform.position.z;
			
		// remove the first node that the unit started on
		currentPath.RemoveAt(0);
		    
		if(currentPath.Count == 1){
			// destination node reached so reset path
			currentPath = null;
			
			//DisplayUnitOptions();
			
			Debug.Log ("Player unit has finished its movement");
			
			if(GameController.SelectedTarget){
				AttackTarget();
			}
			CompleteMovement();
		}
	}
	
	/*
	// will the unit simply wait at destination or attack adjacent enemy, etc
	void DisplayUnitOptions(){
		//unitPopup.SetActive(true);
		unitPopup.Open();
	}*/
	
	void AttackTarget(){
		Health targetsHealth = GameController.SelectedTarget.GetComponent<Health>();
		if(targetsHealth){
			targetsHealth.loseHealth(attackPower);
			Debug.Log("Player Unit attacked the target enemy unit!");
		} else {
			Debug.LogError("Targets health component couldn't be found!");
		}
		GameController.SelectedTarget = null;
		CompleteMovement();

	}

	void CompleteMovement (){
		currentPath = null;
		HasMoved = true;
		GameController.SelectedUnit = null;
		GameController.EndTurn ();
	}	
	
	
	public int GetTileX(){
		return tileX;
	}
	
	public int GetTileZ(){
		return tileZ;
	}
	
	public bool GetHasMoved(){
		return HasMoved;
	}
	
	public void SetHasMoved(bool receivedBool){
		HasMoved = receivedBool;
	}
	
	
}
