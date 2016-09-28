using UnityEngine;
using System.Collections;

public class BirdAI : MonoBehaviour {

	private Rigidbody2D rb2D;
	private SpriteRenderer spriteRend;
	private float DirX;
	private float DirY;
	private float deltaDir = 0.015f;
	private float maxSpeed = 0.5f;
	private float attackCooldown;
	private bool swoop;
	private float initialYSwoop;

	public int swoopDmg = 5;
	public float droppingSpeed = 1;
	public float swoopSpeed = 5;
	public GameObject dropping;

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
		if(attackCooldown <= Time.time && transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x + 1 && transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x - 1) {
			if(Random.Range(0, 2) == 0) {
				GameObject droppingTemp = Instantiate(dropping, transform.position, Quaternion.identity) as GameObject;
				droppingTemp.GetComponent<dropping>().dir = new Vector2(0, -0.1f);
				attackCooldown = Time.time + droppingSpeed;
			}
			else {
				swoop = true;
				attackCooldown = Time.time + swoopSpeed;
				initialYSwoop = transform.position.y;
			}
		}
	}

	void FixedUpdate()
	{
		DirY = 0;
		if(swoop){
			if(attackCooldown - Time.time > 3) {
				DirY = -0.07f;
			}
			if(attackCooldown - Time.time < 1) {
				DirY = 0.07f;
			}
			if(attackCooldown - Time.time <= 0) {
				swoop = false;
				transform.position = new Vector3(transform.position.x, initialYSwoop, 0);
			}
		}

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

		rb2D.MovePosition(new Vector2(transform.position.x + DirX, transform.position.y+DirY));
	}

	void OnTriggerEnter2D(Collider2D obj)
	{
		if(obj.tag == "Player") {
			int xDir = 1;
			if(DirX < 0) {
				xDir = -1;
			}
			obj.GetComponent<Player>().hurt(swoopDmg, 0, new Vector2(xDir, 0));
		}
	}
}
