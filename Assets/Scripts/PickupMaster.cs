using UnityEngine;
using System.Collections;

public class PickupMaster : MonoBehaviour {

	private int maxHealth = 10;

	public string PickupType;

	void OnTriggerEnter2D(Collider2D obj)
	{
		if(obj.tag == "Player") {
			switch (PickupType) {
				case "health":
					if(obj.GetComponent<Player>().hp + 5 > maxHealth) {
						obj.GetComponent<Player>().hp = maxHealth;
					}
					else {
						obj.GetComponent<Player>().hp += 5;
					}
					break;
			}
			Destroy(gameObject);
		}
	}
}
