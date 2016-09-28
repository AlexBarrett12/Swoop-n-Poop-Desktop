using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	private Transform mainCamTransform;
	private bool yCorrected;
	private float knockBackTime;
	private float knockBackX;

	public int hp = 10;
	public float invulnTime;
	public LayerMask groundLayerMask;

	void Start () 
	{
		rb2D = GetComponent<Rigidbody2D>();
		mainCamTransform = Camera.main.transform;
	}

	void Update () 
	{
		collision = 1;
		forwardDir = 0;
		yCorrected = false;
		if((Input.GetAxis("Horizontal") > 0 && knockBackTime <= Time.time) || knockBackX > 0){
			forwardDir = 1;
		}
		if((Input.GetAxis("Horizontal") < 0 && knockBackTime <= Time.time) || knockBackX < 0) {
			forwardDir = -1;
		}
		hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f), Vector2.down, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);
		hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f), Vector2.down, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);
		hitForward = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y*0.4f), Vector3.right*forwardDir, GetComponent<BoxCollider2D>().size.x * 0.55f, groundLayerMask);

		//Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.4f), new Vector3(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.4f, 0) + Vector3.right * forwardDir * GetComponent<BoxCollider2D>().size.x * 0.55f, Color.yellow);

		if(!inAir) { //check if there still is ground beneath the player's feet
			if(hit1.collider == null && hit2.collider == null) {
				inAir = true;
				jump = true;
			}
		}

		if(inAir) {  //handles falling and ground collision detection
			if(downSpeed <= 0 && ((hit1.collider != null && hit1.collider.tag == "Ground") || (hit2.collider != null && hit2.collider.tag == "Ground"))) {
				inAir = false;
				jump = false;
				downSpeed = 0;
				if(hit1.collider != null){
					transform.position = new Vector3(transform.position.x, hit1.collider.transform.position.y + (hit1.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f), 0);
				}
				else{
					transform.position = new Vector3(transform.position.x, hit2.collider.transform.position.y + (hit2.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f), 0);
				}
				yCorrected = true;
			}
		}

		if(Input.GetButton("Jump") && !jump && !inAir) { //handles jumping
			jump = true;
			inAir = true;
			downSpeed = 0.2f;
		}

		if(knockBackTime > Time.time) {
			collision = 0;
		}
		if(hitForward.collider != null && hitForward.collider.tag == "Ground" && !yCorrected) {  //handles horizontal collisions
			transform.position = new Vector3(hitForward.collider.transform.position.x - (hitForward.collider.GetComponent<BoxCollider2D>().size.x*0.5f+GetComponent<BoxCollider2D>().size.x*0.5f)*forwardDir,transform.position.y,transform.position.z);
			collision = 0;
		}
		GameObject.FindGameObjectWithTag("HP").GetComponent<Text>().text = "HP: " + hp;
	}

	void FixedUpdate()
	{
		if(knockBackTime <= Time.time && knockBackX != 0) {
			knockBackX = 0;
		}
		rb2D.MovePosition(new Vector3(transform.position.x + Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX, transform.position.y + downSpeed, 0));
		mainCamTransform.GetComponent<Rigidbody2D>().MovePosition(new Vector2(mainCamTransform.position.x + (transform.position.x - mainCamTransform.position.x)*(transform.position.x - mainCamTransform.position.x)*(transform.position.x - mainCamTransform.position.x), mainCamTransform.position.y + (transform.position.y - mainCamTransform.position.y)*(transform.position.y - mainCamTransform.position.y)*(transform.position.y - mainCamTransform.position.y)));
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
	public void hurt(int dmg, float deltaInvuln, Vector2 dir)
	{
		if(invulnTime <= Time.time) {
			hp -= dmg;
			invulnTime += deltaInvuln;
			knockBackX = dir.x*0.15f;
			knockBackTime = Time.time + 0.2f;
		}
	}
}
