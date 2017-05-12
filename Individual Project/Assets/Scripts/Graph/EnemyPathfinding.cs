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
	
	public float DistanceFromPlayerUnit(GameObject enemyUnit, Unit playerUnit){
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[enemyUnit.GetComponent<Unit>().GetTileX(),
		                           enemyUnit.GetComponent<Unit>().GetTileZ()];
		
		int playerX = (int) playerUnit.transform.position.x;
		int playerZ = (int) playerUnit.transform.position.z;
		
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
		
		// remove the destination node where enemy is as this will come up as
		// an infinite distance in the cost calculation since enemy units
		// are not able to be in the same node/tile as a player unit and vice versa
		currentPath.RemoveAt(0);
		
		float cost = CalculatePathCost (currentPath);
		if(currentPath.Count == 1){
			// check to see if the enemy is adjacent to the player like the current path count suggests
			// and if not then the current path is a dead end so set cost to infinite
			if(!IsAdjacent(startingPoint, destination)){
				//Debug.Log("Enemy unit is NOT adjacent to a Player so shouldn't attack");
				cost = Mathf.Infinity;
			}
			
		}
		
		return cost;
	}
	
	public void MoveUnit(GameObject UnitToMove, int x, int z){
		// reset the selected unit's path
		UnitToMove.GetComponent<Unit>().currentPath = null;
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[UnitToMove.GetComponent<Unit>().GetTileX(),
		                           UnitToMove.GetComponent<Unit>().GetTileZ()];
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
			// no route between target and source so remove target as unreachable
			UnitToMove.GetComponent<Unit>().SetTarget(null);
			Debug.Log("No available route!");
			return;
		}
		
		List<Node> currentPath = new List<Node>();
		Node current = destination;
		float cost = 0;
		// Step through the previous chain and add to current path
		while(current != null){
			currentPath.Add(current);
			current = previousNode[current];
		}
		
		// right now currentPath describes a route from the target to the source
		// so now it gets in inverted
		currentPath.Reverse();

		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is shortened until within the movement range
		// This way the selected enemy unit will still travel towards its destination
		// whether it can be reached in one turn or not
		float MaxUnitMovement = UnitToMove.GetComponent<Unit>().movementSpeed;
		// the distance taking tile type into account
		/*float */cost = CalculatePathCost (currentPath);
		//Debug.Log("Cost for nodes to target before removal: " + cost);
		while(cost > MaxUnitMovement+1){
			Node furthestDistance = currentPath.Last();
			currentPath.Remove(furthestDistance);
			cost = CalculatePathCost (currentPath);
			//Debug.Log("Cost for nodes to target while removing: " + cost);
		}
		// has the enemy reached its target at the destination point?
		// If so, then remove the final node so that it stops in a tile
		// adjcacent to the tile the target is positioned on
		if(currentPath.Last() == destination){
			currentPath.Remove(destination);
		} else if(currentPath.Last() != previousNode[destination]){
			// the enemy unit isn't close enough to reach its target so is unable to attack it
			UnitToMove.GetComponent<Unit>().SetTarget(null);
		}
		
		// double check that the new destination is not already occupied by another unit. 
		// If it is then this MoveUnit method is called again this time knowing that landing 
		// on this tile is illegal 
		if(map.TileOccupiedByEnemy(currentPath.Last().x,currentPath.Last().z)){
			Debug.Log("Enemy destination tile occupied by another unit!");
			current = currentPath.Last();
			currentPath.Clear();
			List<Node> occupiedDestinations = new List<Node>();
			occupiedDestinations.Add(current);
			//Debug.Log(occupiedDestinations.Count);
			MoveUnit(UnitToMove, x, z, occupiedDestinations);
			return;
		}
		
		UnitToMove.GetComponent<Unit>().currentPath = currentPath;
	}
	
	public void MoveUnit(GameObject UnitToMove, int x, int z, List<Node> occupiedDestinations){
	
		Debug.Log("Searching for another path...");
		// reset the selected unit's path
		UnitToMove.GetComponent<Unit>().currentPath = null;
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[UnitToMove.GetComponent<Unit>().GetTileX(),
		                           UnitToMove.GetComponent<Unit>().GetTileZ()];
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
		
		foreach(Node occupied in occupiedDestinations){
			foreach(Node possibleU in unvisited.ToList()){
				if(possibleU == occupied){
					unvisited.Remove(possibleU);
				}
			}
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
			// no route between target and source so remove target as unreachable
			UnitToMove.GetComponent<Unit>().SetTarget(null);
			Debug.Log("No available route!");
			// the only path to the target is occupied and there is no other way
			// so find a different Player Unit to pursue
			Debug.Log("Enemy Unit is searching for a new Player target...");
			EnemyController.FindNewTarget(UnitToMove);
			return;
		}
		
		List<Node> currentPath = new List<Node>();
		Node current = destination;
		
		
		// Step through the previous chain and add to current path
		while(current != null){
			currentPath.Add(current);
			current = previousNode[current];
		}
		
		// right now currentPath describes a route from the target to the source
		// so now it gets in inverted
		currentPath.Reverse();
		
		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is shortened until within the movement range
		// This way the selected enemy unit will still travel towards its destination
		// whether it can be reached in one turn or not
		float MaxUnitMovement = UnitToMove.GetComponent<Unit>().movementSpeed;
		// calculate the cost of the current path and remove nodes
		// while the path costs more than the unit is able to move
		float cost = CalculatePathCost (currentPath);
		while(cost > MaxUnitMovement+1){
			Node furthestDistance = currentPath.Last();
			currentPath.Remove(furthestDistance);
			cost = CalculatePathCost (currentPath);
		}
		
		// if there isn't another route after removing any occupied attacking space nodes
		// then find a new target player unit
		if(currentPath.Count <= 1){
			currentPath.Clear();
			UnitToMove.GetComponent<Unit>().SetTarget(null);
			Debug.Log("Enemy Unit is searching for a new Player target...");
			EnemyController.FindNewTarget(UnitToMove);
			return;
		}
		
		// has the enemy reached its target at the destination point?
		// If so, then remove the final node so that it stops in a tile
		// adjcacent to the tile the target is positioned on
		if(currentPath.Last() == destination){
			currentPath.Remove(destination);
		} else if(currentPath.Last() != previousNode[destination]){
			// the enemy unit isn't close enough to reach its target so is unable to attack it
			UnitToMove.GetComponent<Unit>().SetTarget(null);
		}
		
		// double check that the new destination is not already occupied by another unit. 
		// If it is then this MoveUnit method is called again this time knowing that landing 
		// on this tile is illegal 
		if(map.TileOccupiedByEnemy(currentPath.Last().x,currentPath.Last().z)){
			Debug.Log("Enemy destination tile occupied by another unit!");
			current = currentPath.Last();
			currentPath.Clear();
			occupiedDestinations.Add(current);
			Debug.Log(occupiedDestinations.Count);
			MoveUnit(UnitToMove, x, z, occupiedDestinations);
			return;
		}
		
		occupiedDestinations.Clear();
		
		UnitToMove.GetComponent<Unit>().currentPath = currentPath;
	}
	
	// the distance taking tile type into account
	float CalculatePathCost (List<Node> currentPath){
		float cost = 0f;
		for (int i = 0; i < currentPath.Count; i++) {
			cost += map.TileMovementCostEnemy (currentPath [i].x, currentPath [i].z);
		}
		return cost;
	}
	
	bool IsAdjacent(Node enemyPos, Node playerPos){
		if(enemyPos.x == playerPos.x+1 || enemyPos.x == playerPos.x-1 || enemyPos.z == playerPos.z+1 || enemyPos.z == playerPos.z-1){
			return true;
		} else {
			return false;
		}
	}
	
}
