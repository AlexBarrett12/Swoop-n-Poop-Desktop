using UnityEngine;
using System.Collections;

public class FloorSpawner : MonoBehaviour {

	public GameObject groundTile;

	void Awake () 
	{
		float h;
		for(int i = 0; i < 30; i++) {
			h = 0;
			if(Random.Range(0, 2) == 0) {
				h = 0.5f;
			}
			Instantiate(groundTile, new Vector3((i - 5) * 0.5f, -0.5f, 0), Quaternion.identity);
			Instantiate(groundTile, new Vector3((i + 25) * 0.5f, -0.5f+h, 0), Quaternion.identity);
		}
	}
}
