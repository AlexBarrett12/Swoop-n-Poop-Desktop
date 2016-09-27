using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Rigidbody2D rb2D;
	private bool jump;
	private bool inAir;
	private float moveSpeed = 0.1f;
	private float downSpeed;
	private float maxDownSpeed = 0.2f;
	private RaycastHit2D hitForward;  //This checks what is infront of the player
	private RaycastHit2D hit1;  //These raycasts check what is beneath the player on either side.
	private RaycastHit2D hit2;
	private int forwardDir;
	private int collision;

	public LayerMask groundLayerMask;

	void Start () 
	{
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Update () 
	{
		collision = 1;
		forwardDir = 0;
		if(Input.GetAxis("Horizontal") > 0){
			forwardDir = 1;
		}
		if(Input.GetAxis("Horizontal") < 0) {
			forwardDir = -1;
		}
		hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f), Vector2.down, GetComponent<BoxCollider2D>().size.y * 0.25f, groundLayerMask);
		hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f), Vector2.down, GetComponent<BoxCollider2D>().size.y * 0.25f, groundLayerMask);
		hitForward = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y*0.45f), Vector3.right*forwardDir, GetComponent<BoxCollider2D>().size.x * 0.75f, groundLayerMask);

		if(!inAir) { //check if there still is ground beneath the player's feet
			if(hit1.collider == null && hit2.collider == null) {
				inAir = true;
				jump = true;
			}
		}

		if(inAir) {  //handles falling and ground collision detection
			if((hit1.collider != null && hit1.collider.tag == "Ground") || (hit2.collider != null && hit2.collider.tag == "Ground")) {
				inAir = false;
				jump = false;
				downSpeed = 0;
				if(hit1.collider != null){
					transform.position = new Vector3(transform.position.x, hit1.collider.transform.position.y + (hit1.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f), 0);
				}
				else{
					transform.position = new Vector3(transform.position.x, hit2.collider.transform.position.y + (hit2.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f), 0);
				}
			}
		}

		if(Input.GetButton("Jump") && !jump && !inAir) { //handles jumping
			jump = true;
			inAir = true;
			downSpeed = 0.2f;
		}

		if(hitForward.collider != null && hitForward.collider.tag == "Ground") {  //handles horizontal collisions
			transform.position = new Vector3(hitForward.collider.transform.position.x - (hitForward.collider.GetComponent<BoxCollider2D>().size.x*0.5f+GetComponent<BoxCollider2D>().size.x*0.5f)*forwardDir,transform.position.y,transform.position.z);
			collision = 0;
		}
	}

	void FixedUpdate()
	{
		rb2D.MovePosition(new Vector3( transform.position.x + Input.GetAxis("Horizontal")*moveSpeed*collision, transform.position.y + downSpeed, 0));
		if(inAir) {
			if(!jump) {
				downSpeed = -maxDownSpeed;
			}
			else {
				downSpeed -= 0.02f;
				if(downSpeed <= -maxDownSpeed) {
					jump = false;
					downSpeed = -maxDownSpeed;
				}
			}
		}
	}
}
