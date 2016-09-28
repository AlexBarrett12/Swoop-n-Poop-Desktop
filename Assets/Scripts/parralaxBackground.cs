using UnityEngine;
using System.Collections;

public class parralaxBackground : MonoBehaviour {

	private float initialX;
	private float camInitialX;

	public float scrollRate = 1;
	public bool loop;  //borked right now :/
	public int numInRow;

	void Start () 
	{
		initialX = transform.position.x;
		camInitialX = Camera.main.transform.position.x;
	}

	void FixedUpdate ()
	{
		/*if(loop && transform.position.x < Camera.main.transform.position.x - (9f + GetComponent<SpriteRenderer>().sprite.bounds.max.x*transform.localScale.x)) {
			transform.position = new Vector3(Camera.main.transform.position.x + 8.999999f + (GetComponent<SpriteRenderer>().sprite.bounds.max.x*transform.localScale.x), transform.position.y, 0);
			initialX = transform.position.x;
			Debug.Log("ForwardLoopedX");
		}
		if(loop && transform.position.x > Camera.main.transform.position.x + (9f + GetComponent<SpriteRenderer>().sprite.bounds.max.x*transform.localScale.x)) {
			transform.position = new Vector3(Camera.main.transform.position.x - 8.999999f + (GetComponent<SpriteRenderer>().sprite.bounds.max.x*transform.localScale.x), transform.position.y, 0);
			initialX = transform.position.x;
			Debug.Log("BackLoopedX");
		}*/
		transform.position = new Vector3(initialX+(camInitialX-Camera.main.transform.position.x)*scrollRate, transform.position.y, 0);
	}
}
