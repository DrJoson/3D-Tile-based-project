using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent (typeof (Unit))]
public class Health : MonoBehaviour {

	private Unit thisUnit;
	private GameObject healthBar;
	private Slider healthSlider;
	private Canvas canvas;
	private float maxHealth;
	private float currentHealth;
	
	public GameObject healthBarPrefab;
	public float healthBarOffset = -0.25f;
	
	void Start(){
		thisUnit = GetComponentInParent<Unit>();
		maxHealth = GetComponentInParent<Unit>().health;
		canvas = FindObjectOfType<Canvas>();
		healthBar = Instantiate(healthBarPrefab) as GameObject;
		healthBar.transform.SetParent(canvas.transform, false);
		healthSlider = healthBar.GetComponentInChildren<Slider>();
		currentHealth = maxHealth;
	}
	
	void Update(){
		healthSlider.value = currentHealth/(float)maxHealth;
		Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y  + healthBarOffset, transform.position.z);
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
		healthBar.transform.position = new Vector3(screenPosition.x, screenPosition.y, screenPosition.z);
	}
	
	public void LoseHealth(float damage){
		currentHealth -= damage;
		healthSlider.value = currentHealth;
		if(currentHealth <= 0){
			if(this.tag == "Enemy"){
				GameController.EnemyCount.Remove(thisUnit);
			}
			if(this.tag == "Player"){
				GameController.PlayerCount.Remove(thisUnit);
			}
			Destroy(healthBar);
			Destroy(gameObject);
		}
	}
}
