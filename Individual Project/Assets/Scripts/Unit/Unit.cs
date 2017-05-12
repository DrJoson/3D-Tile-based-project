using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	
	// default unit stats
	public float health = 10;
	public float attackPower = 5;
	public float movementSpeed = 4;
	
	GameController gameController; // used in order to use the EndTurn method
	Unit target;       // used to get the position and health of this unit's target
	Pathfinding graph; // used to change position of unit
	Map map;  		   // used to find cost of moving from node unit is currently positioned at to next node in path
	int tileX, tileZ;  // position of the unit in terms of tiles
	bool HasMoved;     // each unit can only move once per turn
	bool isDefending;  // if a unit stays in one tile it defends itself taking less damage when attacked
	
	// the path the unit will follow
	public List<Node> currentPath = null;
	
	void Start () {
		// put this unit into either a list of player units or enemy units
		if(this.tag == "Player"){
			GameController.PlayerCount.Add(this);
		} else if(this.tag == "Enemy"){
			GameController.EnemyCount.Add(this);
		}
		gameController = FindObjectOfType<GameController>();
		graph = FindObjectOfType<Pathfinding>();
		map = FindObjectOfType<Map>();
		
		tileX = (int)transform.position.x;
		tileZ = (int)transform.position.z;
		HasMoved = false;
		isDefending = false;
	}
	
	void Update (){
		if(!HasMoved){
			if(currentPath != null){
				GameController.MoveInProgress = true;
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
				else if(target){
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
		
		if(this.tag == "Player"){
			remainingMovement -= map.TileMovementCost(currentPath[1].x, currentPath[1].z);
		}
		if(this.tag == "Enemy"){
			remainingMovement -= map.TileMovementCostEnemy(currentPath[1].x, currentPath[1].z);
		}
		
		// move unit to the position of the next node
		Vector3 nextPosition = graph.TileToWorldCoordinates(currentPath[1].x, currentPath[1].z);
		transform.position = nextPosition;
		
		// update the unit's tileX and tileZ parameters
		//tileX = (int)transform.position.x;
		tileX = (int)nextPosition.x;
		//tileZ = (int)transform.position.z;
		tileZ = (int)nextPosition.z;
		
		// remove the first node that the unit started on
		currentPath.RemoveAt(0);
		
		if(currentPath.Count == 1){
			// destination node reached so reset path
			currentPath = null;
			
			Debug.Log ("Unit has finished its movement");
			
			if(target){
				AttackTarget();
			}
			CompleteMovement();
		}
	}
	
	void AttackTarget(){
		Health targetsHealth = target.GetComponent<Health>();
		if(targetsHealth){
			float damageToInflict = attackPower;
			// if the target is defending then the damage inflicted is halved
			if(target.GetIsDefending()){
				damageToInflict = damageToInflict / 2;
			}
			targetsHealth.LoseHealth(damageToInflict);
			Debug.Log("This unit attacked its target " + target.tag +" unit!");
		} else {
			Debug.LogError("Targets health component couldn't be found!");
		}
		target = null;
		CompleteMovement();
		
	}
	
	void CompleteMovement (){
		currentPath = null;
		HasMoved = true;
		GameController.MoveInProgress = false;
		GameController.SelectedUnit = null;
		gameController.EndTurn ();
	}	
	
	
	public int GetTileX(){
		return tileX;
	}
	
	public int GetTileZ(){
		return tileZ;
	}
	
	public void SetTarget(Unit unit){
		target = unit;
	}
	
	public bool GetHasMoved(){
		return HasMoved;
	}
	
	public void SetHasMoved(bool receivedBool){
		HasMoved = receivedBool;
	}
	
	public bool GetIsDefending(){
		return isDefending;
	}
	
	public void SetDefending(bool receivedBool){
		isDefending = receivedBool;
	}
	
	
}
