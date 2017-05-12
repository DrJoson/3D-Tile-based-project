using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	private static Map mapGenerator;
	private static Pathfinding playerPathfinding;
	private static int TurnNumber;
	private static GameObject canvas;
	private static GameObject victoryLabel, loseLabel;
	private static SceneManager sceneManager;
	// used to check whether every player unit has moved
	public static List<Unit> PlayerCount = new List<Unit>();
	// as above and used to find positions of enemies when player units move
	// so that they know that they cannot pass through these enemy held tiles
	public static List<Unit> EnemyCount = new List<Unit>();
	public static int mapSizeX = 10;
	public static int mapSizeZ = 10;
	// used to create the graph used by the player and enemy pathfinding systems
	public static Graph graph;
	// used so that all other objects know when there is a unit selected and what unit it is
	public static GameObject SelectedUnit;
	public static bool MoveInProgress;
	public static Outpost OwnedOutpost;
	public static List<GameObject> reachableIndicators = new List<GameObject>();
	public static bool PlayerTurn;
	int levelNumber = 0;
	
	// Use this for initialization
	void Start () {
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
		// find the scene manager
		sceneManager = GameObject.FindObjectOfType<SceneManager>();
		if(!sceneManager){
			Debug.LogError("No scene manager found!");
		}
		// find the canvas
		canvas = GameObject.Find("Game Canvas");
		
		SelectedUnit = null;
		PlayerTurn = true;
		TurnNumber = 1;
	}
	
	void OnLevelWasLoaded(int level){
		levelNumber = level;
		// clear the player and enemy counts
		GameController.PlayerCount.Clear();
		GameController.EnemyCount.Clear();
		// find map generator
		mapGenerator = FindObjectOfType<Map>();
		if(mapGenerator){
			mapGenerator.GenerateMap(mapSizeX, mapSizeZ, level);
		} else {
			Debug.Log("No map generator found");
			return;
		}
		if(level == 5){
			OwnedOutpost.PlayerOwned = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(SelectedUnit && SelectedUnit.GetComponent<Unit>().GetHasMoved()){
			EndTurn();
		}
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
	
	public void EndTurn(){
		if(PlayerTurn){
			Debug.Log(PlayerCount.Count);
			if(EnemyCount.Count <= 0){
				PlayerVictory();
				return;
			}
			
			foreach(Unit playerUnit in PlayerCount){
				// if one of the player units has claimed enemy outpost
				// then the player is victorious
				if(OwnedOutpost != null){
					if(!OwnedOutpost.PlayerOwned){
						if(playerUnit.transform.position == OwnedOutpost.transform.position){
							PlayerVictory();
							return;
						}
					}
				}
				if(!playerUnit.GetHasMoved()){
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
			
			foreach(Unit enemyUnit in EnemyCount){
				if(!enemyUnit.GetHasMoved()){
					return;
				}
			}
			
			PlayerTurn = true;
			
			foreach(Unit playerUnit in PlayerCount){
				playerUnit.SetHasMoved(false);
				playerUnit.SetDefending(false);
			}
			
			// find the text used to display whose turn it is (player/enemy)
			// and update the information. Now that it is the player's turn
			// again the turn number value also increments
			Transform turn = canvas.transform.Find("Turn");
			Text turnText = turn.GetComponent<Text>();
			turnText.text = "Player Phase";
			turnText.color = Color.blue;
			
			// in level 5, if the player survives for 10 turns then they automatically win
			if(levelNumber == 5 && TurnNumber > 9){
				PlayerVictory();
			}
			
			Transform turnNo = canvas.transform.Find("TurnNo#");
			Text turnNoText = turnNo.GetComponent<Text>();
			TurnNumber++;
			turnNoText.text = "Turn " + TurnNumber.ToString();

			
			Debug.Log("Enemy turn has ended");
			return;
		}
	}
	
	void PlayerVictory (){
		victoryLabel.SetActive(true);
		Debug.Log("You are victorious!");
		Invoke("LoadNextScene", 3);
	}
	
	
	void PlayerLose (){
		loseLabel.SetActive(true);
		Invoke("LoadLoseScene", 3);
	}
	
	void LoadNextScene(){
		sceneManager.LoadNextLevel();
	}
	
	void LoadLoseScene(){
		sceneManager.LoadScene("03b Lose");
	}
	
}
