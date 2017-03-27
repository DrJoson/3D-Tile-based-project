using UnityEngine;
using System.Collections;

public class UnitOptions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	}
	
	public void Open(){
		gameObject.SetActive(true);
	}
	
	
}
