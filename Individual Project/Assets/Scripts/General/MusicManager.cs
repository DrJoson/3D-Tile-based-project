using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip[] levelMusicChangeArray;
	
	private AudioSource audioSource;

	void Awake (){
		DontDestroyOnLoad(gameObject);
		//Debug.Log("Not destroying on load: " + name);
	}

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		audioSource.volume = PlayerPrefsManager.GetMasterVolume();
	}
	
	void OnLevelWasLoaded(int level){
		AudioClip thisLevelMusic = levelMusicChangeArray[level];
		
		if(thisLevelMusic) { // if theres some music attached to the array element
			Debug.Log ("Playing clip: " + thisLevelMusic);
			audioSource.clip = thisLevelMusic;
			audioSource.loop = true;
			audioSource.Play ();
		}
	}
	
	public void SetVolume(float newVolume){
		audioSource.volume = newVolume;
	}
}
