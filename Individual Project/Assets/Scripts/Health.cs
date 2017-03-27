using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float health = 10;
	
	public void loseHealth(float damage){
		health -= damage;
		if(health <= 0){
			if(this.GetComponent<EnemyUnit>()){
				GameController.EnemyCount.Remove(this.GetComponentInParent<EnemyUnit>());
			}
			if(this.GetComponent<PlayerUnit>()){
				GameController.PlayerCount.Remove(this.GetComponentInParent<PlayerUnit>());
			}
			Destroy(gameObject);
		}
	}
}
