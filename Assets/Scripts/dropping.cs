using UnityEngine;
using System.Collections;

public class dropping : MonoBehaviour {

	private Rigidbody2D rb2D;

	public Vector2 dir;
	public int dropDmg = 1;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if(transform.position.y < -5) {
			Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{
		rb2D.MovePosition(new Vector2(transform.position.x + dir.x, transform.position.y + dir.y));
	}

	void OnTriggerEnter2D(Collider2D obj)
	{
		if(obj.tag == "Player") {
			GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().hurt(dropDmg, 0.1f, new Vector2(dir.x, 0));
			Destroy(gameObject);
		}

		if(obj.tag == "Ground") {
			Destroy(gameObject);
		}
	}
}
