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
	private RaycastHit2D hitForward1;  //This checks what is infront of the player
	private RaycastHit2D hitForward2;
	private RaycastHit2D hit1;  //These raycasts check what is beneath the player on either side.
	private RaycastHit2D hit2;
	private RaycastHit2D slopeHit;
	private int forwardDir;
	private int collision;
	private Transform mainCamTransform;
	private bool yCorrected;
	private float knockBackTime;
	private float knockBackX;
	private int raycastVerticalFlip = 1;
	private float angleX;
	private float angleY;

	public int hp = 10;
	public float invulnTime;
	public LayerMask groundLayerMask;
	public LayerMask slopeLayerMask;

	void Start () 
	{
		rb2D = GetComponent<Rigidbody2D>();
		mainCamTransform = Camera.main.transform;
	}

	void Update () 
	{
		Color vertColor = Color.yellow;
		collision = 1;
		forwardDir = 0;
		yCorrected = false;
		if((Input.GetAxis("Horizontal") > 0 && knockBackTime <= Time.time) || knockBackX > 0){
			forwardDir = 1;
		}
		if((Input.GetAxis("Horizontal") < 0 && knockBackTime <= Time.time) || knockBackX < 0) {
			forwardDir = -1;
		}

		raycastVerticalFlip = 1;
		if (downSpeed > 0) {
			raycastVerticalFlip = -1;
			vertColor = Color.green;
		}

		hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y * 0.5f)*raycastVerticalFlip), -Vector2.up*raycastVerticalFlip, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);
		hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y * 0.5f)*raycastVerticalFlip), -Vector2.up*raycastVerticalFlip, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);

		if (downSpeed <= 0) {
			slopeHit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f)), -Vector2.up, GetComponent<BoxCollider2D> ().size.y * 0.01f, slopeLayerMask);
		} else {
			slopeHit = new RaycastHit2D();
		}

		hitForward1 = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - GetComponent<BoxCollider2D> ().size.y * 0.4f), Vector3.right * forwardDir, GetComponent<BoxCollider2D> ().size.x * 0.55f, groundLayerMask);
		hitForward2 = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + GetComponent<BoxCollider2D> ().size.y * 0.4f), Vector3.right * forwardDir, GetComponent<BoxCollider2D> ().size.x * 0.55f, groundLayerMask);

		angleX = 1;
		angleY = 0;
		if (slopeHit.collider != null) {
			angleX = Mathf.Cos (slopeHit.collider.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad);
			angleY = Mathf.Sin (slopeHit.collider.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad);
			//transform.eulerAngles = new Vector3(0,0,slopeHit.collider.gameObject.transform.eulerAngles.z);
		}
		//Debug.Log ("AngleX: "+angleX);

		//Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.4f), new Vector3(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.4f, 0) + Vector3.right * forwardDir * GetComponent<BoxCollider2D>().size.x * 0.55f+Vector3.right*0.01f, Color.blue, 1);
		//Debug.DrawLine(new Vector2(transform.position.x, transform.position.y + GetComponent<BoxCollider2D>().size.y * 0.4f), new Vector3(transform.position.x, transform.position.y + GetComponent<BoxCollider2D>().size.y * 0.4f, 0) + Vector3.right * forwardDir * GetComponent<BoxCollider2D>().size.x * 0.55f+Vector3.right*0.01f, Color.blue, 1);

		Debug.DrawLine(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y * 0.5f)*raycastVerticalFlip), new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f*raycastVerticalFlip) - Vector2.up*raycastVerticalFlip * GetComponent<BoxCollider2D>().size.y * 0.01f, vertColor, 1);
		Debug.DrawLine(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y * 0.5f)*raycastVerticalFlip), new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y * 0.5f*raycastVerticalFlip) - Vector2.up*raycastVerticalFlip * GetComponent<BoxCollider2D>().size.y * 0.01f, vertColor, 1);
		Debug.DrawLine(new Vector3 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f), 0), new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f), 0) -Vector3.up*GetComponent<BoxCollider2D> ().size.y * 0.01f, Color.cyan, 1);

		if(!inAir) { //check if there still is ground beneath the player's feet
			if(hit1.collider == null && hit2.collider == null) {
				inAir = true;
				jump = true;
			}
		}

		if(inAir) {  //handles falling and ground collision detection
			if((hit1.collider != null && hit1.collider.tag == "Ground") || (hit2.collider != null && hit2.collider.tag == "Ground") || (slopeHit.collider != null && slopeHit.collider.tag == "Ground")) {
				if(downSpeed <= 0){
					inAir = false;
					if(hit1.collider != null){
						hit1.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
					}
					else if(hit2.collider != null){
						hit2.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
					}
				}
				jump = false;
				downSpeed = 0;
				//Debug.Log("AngleX: "+angleX+" AngleY: "+angleY);
				if(angleX == 1){
					if(hit1.collider != null){
						transform.position = new Vector3(transform.position.x, hit1.collider.transform.position.y + (hit1.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f)*raycastVerticalFlip, 0);
					}
					else {
						transform.position = new Vector3(transform.position.x, hit2.collider.transform.position.y + (hit2.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f)*raycastVerticalFlip, 0);
					}
				}
				else{
					transform.position = new Vector3(transform.position.x, Mathf.Tan(slopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad)*(transform.position.x - slopeHit.collider.transform.position.x)+ slopeHit.collider.transform.position.y + (slopeHit.collider.GetComponent<BoxCollider2D>().size.y*0.5f+GetComponent<BoxCollider2D>().size.y*0.5f/Mathf.Cos (slopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad)), 0);
				}
				yCorrected = true;
			}
		}

		if(Input.GetButtonDown("Jump") && !jump && !inAir) { //handles jumping
			jump = true;
			inAir = true;
			downSpeed = 0.2f;
			hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y + (GetComponent<BoxCollider2D>().size.y * 0.5f+0.01f)), Vector2.up, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);
			hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f, transform.position.y + (GetComponent<BoxCollider2D>().size.y * 0.5f+0.01f)), Vector2.up, GetComponent<BoxCollider2D>().size.y * 0.01f, groundLayerMask);
			if(hit1.collider != null || hit2.collider != null){
				downSpeed = 0;
				inAir = false;
				jump = false;
			}
		}

		if(knockBackTime > Time.time) {
			collision = 0;
		}
		if(((hitForward1.collider != null && hitForward1.collider.tag == "Ground") || (hitForward2.collider != null && hitForward2.collider.tag == "Ground")) && !yCorrected) {  //handles horizontal collisions
			if(hitForward1.collider != null){
				transform.position = new Vector3(hitForward1.collider.transform.position.x - (hitForward1.collider.GetComponent<BoxCollider2D>().size.x*0.5f+GetComponent<BoxCollider2D>().size.x*0.5f)*forwardDir,transform.position.y,transform.position.z);
			}
			else{
				transform.position = new Vector3(hitForward2.collider.transform.position.x - (hitForward2.collider.GetComponent<BoxCollider2D>().size.x*0.5f+GetComponent<BoxCollider2D>().size.x*0.5f)*forwardDir,transform.position.y,transform.position.z);
			}
			collision = 0;
		}
		GameObject.FindGameObjectWithTag("HP").GetComponent<Text>().text = "HP: " + hp;
	}

	void FixedUpdate()
	{
		if(knockBackTime <= Time.time && knockBackX != 0) {
			knockBackX = 0;
		}

		rb2D.MovePosition(new Vector3(transform.position.x + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleX, transform.position.y + downSpeed + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleY, 0));
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
