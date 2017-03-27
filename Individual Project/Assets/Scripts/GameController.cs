using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	private static Map mapGenerator;
	private static Pathfinding playerPathfinding;
	private static int TurnNumber;
	private static GameObject canvas;
	private static GameObject victoryLabel, loseLabel;
	//public static UnitOptions popupwindow;
	// used to check whether every player unit has moved
	//public static PlayerUnit[] UnitCount;
	public static List<PlayerUnit> PlayerCount = new List<PlayerUnit>();
	// as above and used to find positions of enemies when player units move
	// so that they know that they cannot pass through these enemy held tiles
	//public static EnemyUnit[] EnemyCount;
	public static List<EnemyUnit> EnemyCount = new List<EnemyUnit>();
	public static int mapSizeX = 10;
	public static int mapSizeZ = 10;
	// used to create the graph used by the player and enemy pathfinding systems
	public static Graph graph;
	// used so that all other objects know when there is a unit selected and what unit it is
	public static GameObject SelectedUnit;
	public static GameObject SelectedTarget;
	public static List<GameObject> reachableIndicators = new List<GameObject>();
	public static bool PlayerTurn;

	// Use this for initialization
	void Start () {
		// find map generator
		mapGenerator = FindObjectOfType<Map>();
		if(mapGenerator){
			mapGenerator.GenerateMap(mapSizeX, mapSizeZ);
		} else {
			Debug.LogError("No map generator found!");
		}
		// find graph
		graph = FindObjectOfType<Graph>();
		if(graph){
			graph.GenerateGraph(mapSizeX, mapSizeZ);
		} else {
			Debug.LogError("No graph generator found!");
		}
		// find pathfinding system
		playerPathfinding = FindObjectOfType<Pathfinding>();
		if(!playerPathfinding){
			Debug.LogError("No player pathfinding system found!");
		}
		// find the victory label
		victoryLabel = GameObject.Find("Victory");
		if(!victoryLabel){
			Debug.LogError("No victory label found!");
		}
		victoryLabel.SetActive(false);
		// find the lose label
		loseLabel = GameObject.Find("Defeated");
		if(!loseLabel){
			Debug.LogError("No lose label found!");
		}
		loseLabel.SetActive(false);
		
		// find unit options popup window
		/*popupwindow = FindObjectOfType<UnitOptions>();
		if(!graph){
			Debug.LogError("No pop-up window found!");
		}*/
		canvas = GameObject.Find("Canvas");
		
		SelectedUnit = null;
		PlayerTurn = true;
		TurnNumber = 1;
	
	}
	
	// Update is called once per frame
	void Update () {
		// Deselects a unit if one is was selected
		if(SelectedUnit && Input.GetMouseButtonDown(1)){
			RemoveTileIndicators();
			SelectedUnit = null;
			Debug.Log("Unit deselected");
		}
	}

	public static void ShowReachableTiles(){
		mapGenerator.HighlightReachableTiles();
	}
	
	/*public static void ShowReachableTilesEnemy(){
		mapGenerator.HighlightReachableTilesEnemy();
	}*/
	
	public static void RemoveTileIndicators(){
		reachableIndicators.ForEach(DestroyObject);
		reachableIndicators.Clear();
	}

	public static void MoveSelectedUnit(int receivedTilePosX, int receivedTilePosZ, bool target){
		if(SelectedUnit){
			playerPathfinding.MoveUnit(SelectedUnit, receivedTilePosX, receivedTilePosZ, target);
		} else {
			Debug.LogError("No player unit assigned to game controller");
		}
	}
	
	public static void EndTurn(){
		if(PlayerTurn){
		
			if(EnemyCount.Count <= 0){
				PlayerVictory();
				return;
			}
			
			foreach(PlayerUnit player in PlayerCount){
				if(!player.GetHasMoved()){
					return;
				}
			}
			
			PlayerTurn = false;
		
			// find the text used to display whose turn it is (player/enemy)
			// and update the information
			Transform turn = canvas.transform.Find("Turn");
			Text turnText = turn.GetComponent<Text>();
			turnText.text = "Enemy Phase";
			turnText.color = Color.red;
		
			Debug.Log("Player turn has ended");
			EnemyController.EnemyTurn();
			return;
		} 
		else if(!PlayerTurn){
		
			if(PlayerCount.Count <= 0){
				PlayerLose();
				return;
			}
			
			EnemyController.EnemySelector++;
			EnemyController.SelectNewUnit();
			
			foreach(EnemyUnit enemy in EnemyCount){
				if(!enemy.GetHasMoved()){
					return;
				}
			}
			
			PlayerTurn = true;
			
			foreach(PlayerUnit player in PlayerCount){
				player.SetHasMoved(false);
			}
			
			// find the text used to display whose turn it is (player/enemy)
			// and update the information. Now that it is the player's turn
			// again the turn number value also increments
			Transform turn = canvas.transform.Find("Turn");
			Text turnText = turn.GetComponent<Text>();
			turnText.text = "Player Phase";
			turnText.color = Color.blue;
			
			Transform turnNo = canvas.transform.Find("TurnNo#");
			Text turnNoText = turnNo.GetComponent<Text>();
			TurnNumber++;
			turnNoText.text = "Turn " + TurnNumber.ToString();
			
			Debug.Log("Enemy turn has ended");
			return;
		}
	}
	
	static void PlayerVictory (){
		victoryLabel.SetActive(true);
	}
	
	
	static void PlayerLose (){
		loseLabel.SetActive(true);
	}
}
