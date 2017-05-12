using UnityEngine;
using System.Collections;

public class Archer : Unit {

	// resets all of base class' values and then gives subclass some new ones
	void Reset(){
		health = 10;
		attackPower = 5;
		movementSpeed = 4;
	}
}
