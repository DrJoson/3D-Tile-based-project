using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {
	
	static EnemyPathfinding pathfinding;
	
	static int newTargetNumber;
	
	public static int EnemySelector;
	
	void Start(){
		pathfinding = FindObjectOfType<EnemyPathfinding>();
		newTargetNumber = 0;
		EnemySelector = 0;
	}
	
	public static void EnemyTurn(){
		EnemySelector = 0;
		if(GameController.EnemyCount.Count >= 1){
			foreach(Unit enemy in GameController.EnemyCount){
				enemy.SetHasMoved(false);
			}
			SelectNewUnit();
		}
	}
	
	public static void SelectNewUnit(){
		newTargetNumber = 0;
		if(EnemySelector <= GameController.EnemyCount.Count-1){
			Unit enemyToMove = GameController.EnemyCount[EnemySelector];
			//Debug.Log("Enemy Unit selected");
			
			FindTarget(enemyToMove.gameObject);
		}
	}
	
	static void FindTarget(GameObject enemyUnit){
		GameController.SelectedUnit = enemyUnit;
		// finding the distance between the selected Enemy Unit and all Player Units
		Dictionary<Unit, float> distance = new Dictionary<Unit, float>();
		foreach(Unit playerUnit in GameController.PlayerCount){
			distance[playerUnit] = pathfinding.DistanceFromPlayerUnit(GameController.SelectedUnit, playerUnit);
			//Debug.Log(GameController.SelectedUnit + " distance from " + playerUnit + " is " + distance[playerUnit]);
			
		}
		// organize the Player Units by their distance from the selected Enemy Unit
		List<KeyValuePair<Unit, float>> PlayerUnitsOrderedByDistance = new List<KeyValuePair<Unit, float>>(distance);
		PlayerUnitsOrderedByDistance.Sort(delegate(KeyValuePair<Unit, float> firstPair, KeyValuePair<Unit, float> nextPair) {
			return firstPair.Value.CompareTo(nextPair.Value);
		});

		// set the closest Player Unit as the Enemy Unit's target
		Unit closestTarget = PlayerUnitsOrderedByDistance[0].Key;
		enemyUnit.GetComponent<Unit>().SetTarget(closestTarget);
		int targetX = (int) closestTarget.transform.position.x;
		int targetZ = (int) closestTarget.transform.position.z;
		pathfinding.MoveUnit(GameController.SelectedUnit, targetX, targetZ);
	}
	
	public static void FindNewTarget(GameObject enemyUnit){
		Dictionary<Unit, float> distance = new Dictionary<Unit, float>();
		foreach(Unit playerUnit in GameController.PlayerCount){
			distance[playerUnit] = pathfinding.DistanceFromPlayerUnit(GameController.SelectedUnit, playerUnit);
			//Debug.Log(GameController.SelectedUnit + " distance from " + playerUnit + " is " + distance[playerUnit]);
			
		}
		// organize the player units by their distance from the selected enemy unit
		List<KeyValuePair<Unit, float>> PlayerUnitsOrderedByDistance = new List<KeyValuePair<Unit, float>>(distance);
		PlayerUnitsOrderedByDistance.Sort(delegate(KeyValuePair<Unit, float> firstPair, KeyValuePair<Unit, float> nextPair) {
			return firstPair.Value.CompareTo(nextPair.Value);
		});
		
		newTargetNumber++;
		
		
		// if every possible target has been iterated through and none
		// are reachable, then the Enemy Unit does nothing
		if(newTargetNumber >= GameController.PlayerCount.Count){
			enemyUnit.GetComponent<Unit>().SetHasMoved(true);
			return;
		}
		
		// set the closest player unit as the enemy unit's target
		Unit closestTarget = PlayerUnitsOrderedByDistance[newTargetNumber].Key;
		enemyUnit.GetComponent<Unit>().SetTarget(closestTarget);
		int targetX = (int) closestTarget.transform.position.x;
		int targetZ = (int) closestTarget.transform.position.z;
		pathfinding.MoveUnit(GameController.SelectedUnit, targetX, targetZ);
	}
	
}
		
		
		
		
		
