using UnityEngine;
using System.Collections;

public class BirdAI : MonoBehaviour {

	private Rigidbody2D rb2D;
	private SpriteRenderer spriteRend;
	private float DirX;
	private float deltaDir = 0.015f;
	private float maxSpeed = 0.5f;

	void Start () 
	{
		rb2D = GetComponent<Rigidbody2D>();
		spriteRend = GetComponent<SpriteRenderer>();
	}

	void Update () 
	{
		if(DirX > 0) {
			spriteRend.flipX = true;
		}
		if(DirX < 0) {
			spriteRend.flipX = false;
		}
	}

	void FixedUpdate()
	{
		if(transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x){
			DirX -= deltaDir;
			if(DirX < -maxSpeed) {
				DirX = -maxSpeed;
			}
		}

		if(transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x){
			DirX += deltaDir;
			if(DirX > maxSpeed) {
				DirX = maxSpeed;
			}
		}

		rb2D.MovePosition(new Vector2(transform.position.x + DirX, transform.position.y));
	}
}
