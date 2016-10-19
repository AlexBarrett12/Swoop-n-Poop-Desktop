using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D obj)
	{
		if (obj.tag == "Player") {
			if(GameObject.FindGameObjectWithTag("GameManager") != null){
				GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().endLevel();
			}
			else{
				Application.LoadLevel(Application.loadedLevel+1);
			}
		}
	}
}
