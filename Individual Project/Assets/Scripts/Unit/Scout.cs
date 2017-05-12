using UnityEngine;
using System.Collections;

public class Scout : Unit {
	
	// resets all of base class' values and then gives subclass some new ones
	void Reset(){
		health = 8;
		attackPower = 3;
		movementSpeed = 5;
	}
	
}