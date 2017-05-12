using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Tile[] tileTypes;
	public GameObject reachableIndicator;
	public int[,] tiles;
	
	Pathfinding path;

	public int mapSizeX;
	public int mapSizeZ;

	void Start(){
		path = FindObjectOfType<Pathfinding>();
	}

	public void GenerateMap(int sizeX, int sizeZ, int level){
		mapSizeX = sizeX;
		mapSizeZ = sizeZ;
		GenerateMapData(level);
		GenerateTiles();
	}
	
	void GenerateMapData(int level){
		tiles = new int[mapSizeX,mapSizeZ];
		
		// defaulting all map to be made of grass tiles		
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				tiles[x,z] = 0;
			}
		}
		
		if(level == 3){
		// tutorial stage
		
		// setting some forest tiles		
		tiles[0,1] = 1;
		tiles[0,2] = 1;
		tiles[0,3] = 1;
		tiles[0,4] = 1;
		tiles[0,5] = 1;
		tiles[0,6] = 1;
		tiles[0,9] = 1;
		tiles[1,1] = 1;
		tiles[1,2] = 1;
		tiles[1,3] = 1;
		tiles[1,4] = 1;
		tiles[1,9] = 1;
		tiles[2,9] = 1;
		tiles[2,2] = 1;
		tiles[2,3] = 1;
		tiles[3,2] = 1;
		tiles[3,3] = 1;
		
		// setting some stone tiles
		tiles[5,3] = 2;
		tiles[5,2] = 2;
		tiles[6,2] = 2;
		tiles[8,1] = 2;
		tiles[8,5] = 2;
		tiles[8,6] = 2;
		tiles[9,1] = 2;
		tiles[9,5] = 2;
		tiles[9,6] = 2;
		}
		
		if(level == 4){ 
		// claim outpost stage
		
		// setting some forest tiles
		tiles[0,4] = 1;
		tiles[0,5] = 1;
		tiles[0,6] = 1;
		tiles[1,4] = 1;
		tiles[1,5] = 1;
		tiles[1,6] = 1;
		tiles[2,4] = 1;
		tiles[2,5] = 1;
		tiles[2,6] = 1;
		tiles[3,5] = 1;
		tiles[3,6] = 1;
		// setting some stone tiles
		tiles[8,1] = 2;
		tiles[3,2] = 2;
		tiles[4,2] = 2;
		tiles[5,2] = 2;
		tiles[8,2] = 2;
		tiles[4,3] = 2;
		tiles[5,3] = 2;
		tiles[7,3] = 2;
		tiles[7,4] = 2;
		tiles[5,4] = 2;
		tiles[7,5] = 2;
		tiles[8,3] = 2;
		
		// setting up outpost tile
		tiles[1,8] = 3;
		}
	
		if(level == 5){
		// survive attack stage
		
		// setting some stone tiles
		tiles[3,1] = 2;
		tiles[3,3] = 2;
		tiles[4,1] = 2;
		tiles[4,3] = 2;
		tiles[5,0] = 2;
		tiles[5,1] = 2;
		tiles[5,3] = 2;
		tiles[5,4] = 2;
		tiles[6,0] = 2;
		tiles[6,4] = 2;
		tiles[6,5] = 2;
		tiles[6,6] = 2;
		tiles[7,0] = 2;
		tiles[8,0] = 2;
		tiles[8,4] = 2;
		tiles[8,5] = 2;
		tiles[8,6] = 2;
		tiles[9,0] = 2;
		tiles[9,1] = 2;
		tiles[9,2] = 2;
		tiles[9,3] = 2;
		tiles[9,4] = 2;
		
		}
		
	}	
	
	void GenerateTiles (){
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				Tile tileArray = tileTypes[tiles[x,z]];
				// create a tile at the current map position
				GameObject tile = Instantiate(tileArray.tilePrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
				// set the tile to be a child of the Tile Map game object. This just keeps the Unity hierarchy easier on the eyes
				tile.transform.parent = GameObject.Find("Tile Map").transform;
			}
		}
	}

	public void HighlightReachableTiles(){
		for(int x = 0; x < mapSizeX; x++){
			for(int z = 0; z < mapSizeZ; z++){
				if(path.PathFound(GameController.SelectedUnit, x, z)){
					GameObject reachableTile = Instantiate(reachableIndicator, new Vector3(x, 0.1f, z), Quaternion.identity) as GameObject;
					GameController.reachableIndicators.Add(reachableTile);
					reachableTile.transform.parent = GameObject.Find("Highlighted Tiles").transform;
				}
			}
		}
	}
	
	public bool TilePassable(int x, int z){
		return tileTypes[tiles[x,z]].passable;
	}
	
	public bool TileOccupiedByEnemy(int x, int z){
		bool occupied = false;
		foreach(Unit enemyUnit in GameController.EnemyCount){
			//Vector3 EnemyPosition = enemyUnit.transform.position;
			Vector3 EnemyPosition = new Vector3(enemyUnit.GetTileX(), 0, enemyUnit.GetTileZ());
			if(path.TileToWorldCoordinates(x,z) == EnemyPosition){
 				if(EnemyPosition != GameController.SelectedUnit.transform.position){
				//if(EnemyPosition != MovingUnit.transform.position){
					occupied = true;
					return occupied;
				}
			}
		}
		return occupied;
	}
	
	public bool TileOccupiedByPlayer(int x, int z){
		bool occupied = false;
		foreach(Unit playerUnit in GameController.PlayerCount){
			//Vector3 PlayerPosition = playerUnit.transform.position;
			Vector3 PlayerPosition = new Vector3(playerUnit.GetTileX(), 0, playerUnit.GetTileZ());
			if(path.TileToWorldCoordinates(x,z) == PlayerPosition){
				if(PlayerPosition != GameController.SelectedUnit.transform.position){
					occupied = true;
					return occupied;
				}
			}
		}
		return occupied;
	}
	
	public float TileMovementCost(int x, int z){
		Tile tileToMoveTo = tileTypes[tiles[x,z]];
		if(TilePassable(x,z) == false || TileOccupiedByEnemy(x,z) == true){
			return Mathf.Infinity;
		}
		return tileToMoveTo.movementCost;
	}
	
	// for enemy units to use
	public float TileMovementCostEnemy(int x, int z){
		Tile tileToMoveTo = tileTypes[tiles[x,z]];
		if(TilePassable(x,z) == false || TileOccupiedByPlayer(x,z) == true){
			return Mathf.Infinity;
		}
		return tileToMoveTo.movementCost;
	}
	
	
}