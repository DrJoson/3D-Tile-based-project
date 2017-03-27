using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	public float autoLoadNextLevelAfter;
	
	void Start(){
		if(autoLoadNextLevelAfter <= 0){
			Debug.Log ("Level auto load disabled, use positive number in inspector to use");
		} else {
			Invoke("LoadNextLevel", autoLoadNextLevelAfter);
		}
	}

	public void loadScene(string name){
		Debug.Log("Scene load requested: " + name);
		Application.LoadLevel(name);
	}
	
	public void quitGame(){
		Debug.Log("Player wants to quit the game");
		Application.Quit();
	}
	
	public void LoadNextLevel(){
		Application.LoadLevel(Application.loadedLevel + 1);
	}
	
}
