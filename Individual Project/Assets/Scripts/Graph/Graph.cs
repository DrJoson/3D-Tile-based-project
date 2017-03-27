using UnityEngine;
using System.Collections;

public class Graph : MonoBehaviour {

	int mapSizeX;
	int mapSizeZ;
	
	public Node[,] nodes;
	
	public void GenerateGraph (int sizeX, int sizeZ){
		
		mapSizeX = sizeX;
		mapSizeZ = sizeZ;
		
		// initialise array of Nodes
		nodes = new Node[mapSizeX,mapSizeZ];
		
		// initialise a Node for every tile in the graph
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				nodes[x,z] = new Node();
				nodes[x,z].x = x;
				nodes[x,z].z = z;
			}
		}
		
		// add neighbours to each of the initialised Nodes
		for(int x=0; x < mapSizeX; x++){
			for(int z=0; z < mapSizeZ; z++){
				// adds neighbours to the current Node. They can be added north, east, 
				// south and west but only if there are tiles in those directions
				if(z < mapSizeZ-1){
					nodes[x,z].neighbours.Add(nodes[x,z+1]);
				}
				if(x < mapSizeX-1){
					nodes[x,z].neighbours.Add(nodes[x+1,z]);
				}
				if(z > 0){
					nodes[x,z].neighbours.Add(nodes[x,z-1]);
				}
				if(x > 0){
					nodes[x,z].neighbours.Add(nodes[x-1,z]);
				}
			}
		}
	}
}
