using UnityEngine;
using System.Collections;

public class Warrior : Unit {

	// resets all of base class' values and then gives subclass some new ones
	void Reset(){
		health = 12;
		attackPower = 5;
		movementSpeed = 3;
	}
	
}
