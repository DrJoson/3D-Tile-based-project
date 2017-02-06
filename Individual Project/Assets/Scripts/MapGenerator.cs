using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
	
	public TileType[] tileTypes;
	
	int[,] tiles;
	
	int mapSizeX = 10;
	int mapSizeZ = 10;
	
	public void GenerateMap(){
		GenerateMapData();
		GenerateTiles();
	}
	
	void GenerateMapData(){
		tiles = new int[mapSizeX,mapSizeZ];
		
		// defaulting all map to be made of grass tiles		
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				tiles[x,z] = 0;
			}
		}
		// setting some forest tiles
		tiles[0, 2] = 1;
		tiles[0, 3] = 1;
		tiles[1, 2] = 1;
		tiles[1, 3] = 1;
		tiles[2, 2] = 1;
		tiles[2, 3] = 1;
		
		// setting some stone tiles
		tiles[5, 2] = 2;
		tiles[5, 3] = 2;
		tiles[5, 4] = 2;
		tiles[6, 3] = 2;
		tiles[6, 4] = 2;
		
	}
	
	void GenerateTiles (){
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				TileType tt = tileTypes[tiles[x,z]];
				Instantiate(tt.tilePrefab, new Vector3(x, 0, z), Quaternion.identity);
			}
		}
	}
}