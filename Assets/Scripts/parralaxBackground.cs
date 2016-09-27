using UnityEngine;
using System.Collections;

public class parralaxBackground : MonoBehaviour {

	private float initialX;
	private float camInitialX;

	public float scrollRate = 1;

	void Start () 
	{
		initialX = transform.position.x;
		camInitialX = Camera.main.transform.position.x;
	}

	void FixedUpdate ()
	{
		transform.position = new Vector3(initialX+(camInitialX-Camera.main.transform.position.x)*scrollRate, transform.position.y, 0);
	}
}
