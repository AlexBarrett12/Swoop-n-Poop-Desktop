using UnityEngine;
using System.Collections;

public class FloorSpawner : MonoBehaviour {

	public GameObject groundTile;
	public float incrementX = 1;
	public float incrementY;
	public float startX;
	public float startY;
	public int count = 30;

	void Awake () 
	{
		float h;
		for(int i = 0; i < count; i++) {
			//h = 0;
			//if(Random.Range(0, 2) == 0) {
				//h = 0.5f;
			//}
			Instantiate(groundTile, new Vector3((startX), startY, 0), Quaternion.identity);
			startX += incrementX*0.5f;
			startY += incrementY*0.5f;
		}
	}
}
