using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {
	
	static EnemyPathfinding pathfinding;
	
	public static int EnemySelector;
	
	void Start(){
		pathfinding = FindObjectOfType<EnemyPathfinding>();
		EnemySelector = 0;
	}
	
	public static void EnemyTurn(){
		EnemySelector = 0;
		if(GameController.EnemyCount.Count >= 1){
			foreach(EnemyUnit enemy in GameController.EnemyCount){
				enemy.SetHasMoved(false);
			}
			SelectNewUnit();
		}
	}
	
	public static void SelectNewUnit(){
		if(EnemySelector <= GameController.EnemyCount.Count-1){
			EnemyUnit enemyToMove = GameController.EnemyCount[EnemySelector];
			FindTarget(enemyToMove);
		}
	}
	
	static void FindTarget(EnemyUnit enemy){
		GameController.SelectedUnit = enemy.gameObject;
		Dictionary<PlayerUnit, float> distance = new Dictionary<PlayerUnit, float>();
		foreach(PlayerUnit player in GameController.PlayerCount){
			distance[player] = pathfinding.DistanceFromPlayerUnit(GameController.SelectedUnit, player);
			Debug.Log(distance[player]);
		}
		//GameController.ShowReachableTilesEnemy();
		List<KeyValuePair<PlayerUnit, float>> PlayerUnitsOrderedByDistance = new List<KeyValuePair<PlayerUnit, float>>(distance);
		PlayerUnitsOrderedByDistance.Sort(delegate(KeyValuePair<PlayerUnit, float> firstPair, KeyValuePair<PlayerUnit, float> nextPair) {
			return firstPair.Value.CompareTo(nextPair.Value);
		});

		PlayerUnit closestTarget = PlayerUnitsOrderedByDistance[0].Key;
		GameController.SelectedTarget = closestTarget.gameObject;
		int targetX = (int) closestTarget.transform.position.x;
		int targetZ = (int) closestTarget.transform.position.z;
		pathfinding.MoveUnit(GameController.SelectedUnit, targetX, targetZ);
	}
	
}
		
		
		
		
		
