using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Pathfinding : MonoBehaviour {
	
	Graph graph;
	Node[,] nodes;
	
	// So I can access the MapGenerator method 'TileMovementCost'
	Map map;
	
	void Start(){
		graph = GetComponent<Graph>();
		nodes = graph.nodes;
		map = GetComponent<Map>();
	}
	
	public bool PathFound(GameObject Unit, int x, int z){
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[Unit.GetComponent<PlayerUnit>().GetTileX(),
		                           Unit.GetComponent<PlayerUnit>().GetTileZ()];
		
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
				// float alt = distance[u] + u.DistanceTo(neighbour);
				float alt = distance[u] + map.TileMovementCost(neighbour.x, neighbour.z);
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
			return false;
		}
		
		List<Node> currentPath = new List<Node>();
		Node current = destination;
		
		
		// Step through the previous chain and add to current path
		while(current != null){
			currentPath.Add(current);
			current = previousNode[current];
		}
		
		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is cleared and the unit is not moved
		float MaxUnitMovement = Unit.GetComponent<PlayerUnit>().movementSpeed;
		// the distance without taking tile type into account
		/*		if(currentPath.Count > MaxUnitMovement+1){
			Debug.Log("This tile is beyond this unit's movement range");
			currentPath.Clear();
			return;
		} */
		// the distance taking tile type into account
		float cost = 0f;
		for(int i = 0; i < currentPath.Count; i++){
			cost += map.TileMovementCost(currentPath[i].x, currentPath[i].z);
		}
		if(cost > MaxUnitMovement+1){
			currentPath.Clear();
			return false;
		}

		//Debug.Log("This tile is reachable");
		return true;
	}
	
	public void MoveUnit(GameObject UnitToMove, int x, int z, bool target){
		// reset the selected unit's path
		UnitToMove.GetComponent<PlayerUnit>().currentPath = null;
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[UnitToMove.GetComponent<PlayerUnit>().GetTileX(),
								   UnitToMove.GetComponent<PlayerUnit>().GetTileZ()];
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
				if(neighbour == destination && target){
					alt = 0f;
				}
				else { 
					alt = distance[u] + map.TileMovementCost(neighbour.x, neighbour.z);
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

		// if the selected player unit's destination is also an enemy target
		// remove the node they are positioned at so that the selected unit's
		// destination becomes the node adjacent to the enemy target
		if(target){
			currentPath.RemoveAt(0);
		}
		
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
		}
		
		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is cleared and the unit is not moved
		float MaxUnitMovement = UnitToMove.GetComponent<PlayerUnit>().movementSpeed;
		// the distance without taking tile type into account
/*		if(currentPath.Count > MaxUnitMovement+1){
			Debug.Log("This tile is beyond this unit's movement range");
			currentPath.Clear();
			return;
		} */
		// the distance taking tile type into account
		float cost = 0f;
		for(int i = 0; i < currentPath.Count; i++){
			cost += map.TileMovementCost(currentPath[i].x, currentPath[i].z);
		}
		if(cost > MaxUnitMovement+1){
			Debug.Log("This tile is beyond this unit's movement range");
			GameController.SelectedTarget = null;
			currentPath.Clear();
			return;
		}
		
		// right now currentPath describes a route from the target to the source
		// so now it gets in inverted
		
		currentPath.Reverse();
		
		GameController.RemoveTileIndicators();

		UnitToMove.GetComponent<PlayerUnit>().currentPath = currentPath;
	}
	
	public void MoveUnit(GameObject UnitToMove, int x, int z, bool target, List<Node> occupiedDestinations){
	
		Debug.Log("Searching for another path...");
		// reset the selected unit's path
		UnitToMove.GetComponent<PlayerUnit>().currentPath = null;
		
		Dictionary<Node, float> distance = new Dictionary<Node, float>();
		Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
		
		List<Node> unvisited = new List<Node>();
		
		Node startingPoint = nodes[UnitToMove.GetComponent<PlayerUnit>().GetTileX(),
		                           UnitToMove.GetComponent<PlayerUnit>().GetTileZ()];
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
				if(neighbour == destination && target){
					alt = 0f;
				}
				else { 
					alt = distance[u] + map.TileMovementCost(neighbour.x, neighbour.z);
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
		
		// if the selected player unit's destination is also an enemy target
		// remove the node they are positioned at so that the selected unit's
		// destination becomes the node adjacent to the enemy target
		if(target){
			currentPath.RemoveAt(0);
		}
		
		// double check that the new destination adjacent to target enemy unit is not
		// already occupied by another player unit. If it is then this MoveUnit method is
		// called again this time knowing that landing on this tile is illegal
		if(map.TileOccupiedByPlayer(currentPath.ElementAt(0).x,currentPath.ElementAt(0).z)){
			Debug.Log("Attacking tile occupied by another unit!");
			current = currentPath.ElementAt(0);
			currentPath.Clear();
			occupiedDestinations.Add(current);
			Debug.Log(occupiedDestinations.Count);
			MoveUnit(UnitToMove, x, z, target, occupiedDestinations);
			return;
		}
		
		// Checking whether this distance is within this selected unit's movement range
		// If it is not then the path is cleared and the unit is not moved
		float MaxUnitMovement = UnitToMove.GetComponent<PlayerUnit>().movementSpeed;
		// the distance taking tile type into account
		float cost = 0f;
		for(int i = 0; i < currentPath.Count; i++){
			cost += map.TileMovementCost(currentPath[i].x, currentPath[i].z);
		}
		if(cost > MaxUnitMovement+1){
			Debug.Log("This tile is beyond this unit's movement range");
			GameController.SelectedTarget = null;
			currentPath.Clear();
			return;
		}
		
		// right now currentPath describes a route from the target to the source
		// so now it gets in inverted
		
		currentPath.Reverse();
		occupiedDestinations.Clear();
		
		GameController.RemoveTileIndicators();
		
		UnitToMove.GetComponent<PlayerUnit>().currentPath = currentPath;
	}
	
	// used by the player unit to draw visual interpretation of current path
	public Vector3 TileToWorldCoordinates(int x, int z){
		return new Vector3(x,0,z);
	}
		
			
}
