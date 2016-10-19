using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private Rigidbody2D rb2d;

	public Vector2 shootDir;
	public float fireSpeed = 1;
	public int dmg = 1;

	void Start()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		int neg = 1;
		if (shootDir.y < 0) {
			neg = -1;
		}
		transform.eulerAngles = new Vector3(0, 0, Mathf.Acos(shootDir.x)*Mathf.Rad2Deg*neg);
	}

	void FixedUpdate()
	{
		//transform.eulerAngles = new Vector3(0, 0, Mathf.Acos(shootDir.x)*Mathf.Rad2Deg);
		rb2d.MovePosition (new Vector2(transform.position.x, transform.position.y) + shootDir*fireSpeed);
		if (transform.position.y > 50) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D obj){
		if (obj.tag == "Enemy") {
			obj.GetComponent<BirdAI>().hurt(dmg);
			Destroy(gameObject);
		}
		if (obj.tag == "Ground") {
			Destroy (gameObject);
		}
	}
}
