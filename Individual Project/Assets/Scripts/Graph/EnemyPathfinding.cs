using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyPathfinding : MonoBehaviour {

	Graph graph;
	Node[,] nodes;
	
	// So I can access the MapGenerator method 'TileMovementCostEnemy'
	Map map;
	
	void Start(){
		graph = GetComponent<Graph>();
		nodes = graph.nodes;
		map = GetComponent<Map>();
	}
	
	public float DistanceFromPlayerUnit(GameObject Unit, PlayerUnit player){
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[Unit.GetComponent<EnemyUnit>().GetTileX(),
		                           Unit.GetComponent<EnemyUnit>().GetTileZ()];
		
		int playerX = (int) player.transform.position.x;
		int playerZ = (int) player.transform.position.z;
		
		Node destination = nodes[playerX,playerZ];
		
		distance[startingPoint] = 0;
		previousNode[startingPoint] = null;
		
		// initialise every node to have infinity distance and to have no node linked to it
		foreach(Node node in nodes){
			if(node != startingPoint){
				distance[node] = Mathf.Infinity;
				previousNode[node] = null;
			}
			// add node to the unvisited list of nodes
			unvisited.Add(node);
		}
		// while the list of nodes is not empty check if each of them are the destination
		while(unvisited.Count > 0){
			// u will be the unvisited node with the shortest distance
			Node u = null;
			
			foreach(Node possibleU in unvisited){
				if(u == null || distance[possibleU] < distance[u]){
					u = possibleU;
				}
			}
			
			if(u == destination){
				// the destination node has been found in the graph so exit while loop
				break;
			}
			
			// node is removed from list of unvisited nodes
			unvisited.Remove(u);
			
			foreach(Node neighbour in u.neighbours){
				// float alt = distance[u] + u.DistanceTo(neighbour);
				float alt;
				// if this neighbour node is the destination then the distance doesn't matter since 
				// the selected unit can attack a target in the destination node from a neighbour node. 
				if(neighbour == destination){
					alt = 0f;
				} else {
				alt = distance[u] + map.TileMovementCostEnemy(neighbour.x, neighbour.z);
				}
				if(alt < distance[neighbour]){
					distance[neighbour] = alt;
					previousNode[neighbour] = u;
				}
			}
		}
		
		// if we get there, then either we found the shortest route
		// to our target, or there is no route at all to the target
		if(previousNode[destination] == null){
			// no route between target and source
			return Mathf.Infinity;
		}
		
		List<Node> currentPath = new List<Node>();
		Node current = destination;
		
		
		// Step through the previous chain and add to current path
		while(current != null){
			currentPath.Add(current);
			current = previousNode[current];
		}
	
		return (float)currentPath.Count;
	}
	
	public void MoveUnit(GameObject UnitToMove, int x, int z){
		// reset the selected unit's path
		UnitToMove.GetComponent<EnemyUnit>().currentPath = null;
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[UnitToMove.GetComponent<EnemyUnit>().GetTileX(),
		                           UnitToMove.GetComponent<EnemyUnit>().GetTileZ()];
		Node destination = nodes[x,z];
		
		distance[startingPoint] = 0;
		previousNode[startingPoint] = null;
		
		// initialise every node to have infinity distance and to have no node linked to it
		foreach(Node node in nodes){
			if(node != startingPoint){
				distance[node] = Mathf.Infinity;
				previousNode[node] = null;
			}
			// add node to the unvisited list of nodes
			unvisited.Add(node);
		}
		// while the list of nodes is not empty check if each of them are the destination
		while(unvisited.Count > 0){
			// u will be the unvisited node with the shortest distance
			Node u = null;
			
			foreach(Node possibleU in unvisited){
				if(u == null || distance[possibleU] < distance[u]){
					u = possibleU;
				}
			}
			
			if(u == destination){
				// the destination node has been found in the graph so exit while loop
				break;
			}
			
			// node is removed from list of unvisited nodes
			unvisited.Remove(u);
			
			foreach(Node neighbour in u.neighbours){
				// used for a simple distance not taking tile type into account
				// float alt = distance[u] + u.DistanceTo(neighbour); 
				float alt;
				// if this neighbour node is the destination and there is a targeted enemy there
				// then the distance doesn't matter since the selected unit can attack the target
				// from a neighbour node. This allows the user to select a tile 'occupied' by an
				// enemy unit in the event of that unit being its target
				if(neighbour == destination){
					alt = 0f;
				}
				else { 
					alt = distance[u] + map.TileMovementCostEnemy(neighbour.x, neighbour.z);
				}
				if(alt < distance[neighbour]){
					distance[neighbour] = alt;
					previousNode[neighbour] = u;
				}
			}
		}
		
		// if we get there, then either we found the shortest route
		// to our destination, or there is no route at all to it
		if(previousNode[destination] == null){
			// no route between target and source
			Debug.Log("No available route!");
			return;
		}
		
		List<Node> currentPath = new List<Node>();
		Node current = destination;
		
		
		// Step through the previous chain and add to current path
		while(current != null){
			currentPath.Add(current);
			current = previousNode[current];
		}
		
		/*
		// double check that the new destination adjacent to target enemy unit is not
		// already occupied by another player unit. If it is then this MoveUnit method is
		// called again this time knowing that landing on this tile is illegal
		if(map.TileOccupiedByPlayer(currentPath.ElementAt(0).x,currentPath.ElementAt(0).z)){
			Debug.Log("Attacking tile occupied by another unit!");
			current = currentPath.ElementAt(0);
			currentPath.Clear();
			List<Node> occupiedDestinations = new List<Node>();
			occupiedDestinations.Add(current);
			Debug.Log(occupiedDestinations.Count);
			MoveUnit(UnitToMove, x, z, target, occupiedDestinations);
			return;
		}*/
		
		// right now currentPath describes a route from the target to the source
		// so now it gets in inverted
		currentPath.Reverse();

		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is shortened until within the movement range
		// This way the selected enemy unit will still travel towards its destination
		// whether it can be reached in one turn or not
		float MaxUnitMovement = UnitToMove.GetComponent<EnemyUnit>().movementSpeed;
		// the distance taking tile type into account
		float cost = CalculatePathCost (currentPath);
		while(cost > MaxUnitMovement+1){
			Node furthestDistance = currentPath.Last();
			currentPath.Remove(furthestDistance);
			cost = CalculatePathCost (currentPath);
		}
		// has the enemy reached its target at the destination point?
		// If so, then remove the final node so that it stops in a tile
		// adjcacent to the tile the target is positioned on
		if(currentPath.Last() == destination){
			currentPath.Remove(destination);
		} else if(currentPath.Last() != previousNode[destination]){
			// the enemy unit isn't close enough to reach its target so is unable to attack it
			GameController.SelectedTarget = null;
		}
		UnitToMove.GetComponent<EnemyUnit>().currentPath = currentPath;
	}
	
	float CalculatePathCost (List<Node> currentPath){
		float cost = 0f;
		for (int i = 0; i < currentPath.Count; i++) {
			cost += map.TileMovementCostEnemy (currentPath [i].x, currentPath [i].z);
		}
		return cost;
	}
	
}
