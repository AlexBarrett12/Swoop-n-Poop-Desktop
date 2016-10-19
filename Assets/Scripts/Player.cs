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
	private RaycastHit2D slopeHit;  //This calculates the gradient of the slope beneath the player if there is one.
	private RaycastHit2D futureSlopeHit;  //Currently unused. This predicts where the player will be next frame and adjusts the player accordingly.
	private int forwardDir;  //Determines which direction the player is travelling in, used for horizontal collision.
	private int collision;  //Stops the player if there is a collision
	private Transform mainCamTransform;
	private bool yCorrected;  //Allows vertical collision to take place before horizontal collision if there is any vertical collision.
	private float knockBackTime;
	private float knockBackX;  //Knocks back the player ta this rate.
	private int raycastVerticalFlip = 1;  //Flips vertical raycast vertically.
	private float angleX;  //AngleX and AngleY are used to calculate how that player moves up a slope.
	private float angleY;
	private float initialJumpTime;  //Testing variables used to check if frame independence exists.
	private float finalJumpTime;
	private float slopeHitExtender = 1; //This extended the slopeHit raycast if there was no hit.

	private float attackCooldown;

	public int ammo;
	//public int hp = 10;  //moved to saveStuff.hp
	public float invulnTime;
	public LayerMask groundLayerMask;
	public LayerMask slopeLayerMask;
	public string currentWeapon;
	public GameObject holdingWeapon;
	public static StuffToSave saveStuff = new StuffToSave();  //Holds everything that the player might save, i.e. score, upgrades, lives e.c.t
	public float yMovement;  //Used by birds to calculate where the player will be when it swoops
	public GameObject[] weapons = new GameObject[2];  //Array of all the weapons used by the player. 0 for stone, 1 for gun e.t.c;
	public GameObject[] bullets = new GameObject[2];

	void Start ()
	{
		rb2D = GetComponent<Rigidbody2D> ();
		mainCamTransform = Camera.main.transform;
		//SaveLoad.Save();
		//SaveLoad.clearSave ();
		saveStuff = SaveLoad.Load();
	}

	void Update () 
	{
		Color vertColor = Color.yellow;
		collision = 1;
		forwardDir = 0;
		yCorrected = false;
		slopeHitExtender = 1;
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

		hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f)*raycastVerticalFlip), -Vector2.up*raycastVerticalFlip, GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.01f, groundLayerMask);
		hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.4f, transform.position.y - (GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f)*raycastVerticalFlip), -Vector2.up*raycastVerticalFlip, GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.01f, groundLayerMask);

		if (downSpeed <= 0) {
			slopeHit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.5f)), -Vector2.up, GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.01f, slopeLayerMask);
		} else {
			slopeHit = new RaycastHit2D();
			futureSlopeHit = new RaycastHit2D();
		}

		hitForward1 = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.4f), Vector3.right * forwardDir, GetComponent<BoxCollider2D> ().size.x * transform.localScale.x * 0.55f, groundLayerMask);
		hitForward2 = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y + GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.4f), Vector3.right * forwardDir, GetComponent<BoxCollider2D> ().size.x * transform.localScale.x * 0.55f, groundLayerMask);

		if(slopeHit.collider == null && downSpeed == 0) {  //This double checks if there actually isn't a slope beneath the player, in case the player has interpolated the slope too far. Would work better with a future slop check but I couldn't get it working.
			slopeHitExtender = 5;
			slopeHit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.5f)), -Vector2.up, GetComponent<BoxCollider2D> ().size.y * transform.localScale.y * 0.01f*slopeHitExtender, slopeLayerMask);
			if(slopeHit.collider != null && slopeHit.collider.transform.eulerAngles.z == 0) {
				slopeHit = new RaycastHit2D();
			}
		}

		angleX = 1;
		angleY = 0;
		if (slopeHit.collider != null) {
			angleX = Mathf.Cos (slopeHit.collider.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad);
			angleY = Mathf.Sin (slopeHit.collider.gameObject.transform.eulerAngles.z*Mathf.Deg2Rad);
			//transform.eulerAngles = new Vector3(0,0,slopeHit.collider.gameObject.transform.eulerAngles.z);
		}

		//Debug.Log ("AngleX: "+angleX);

		//Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.4f), new Vector3(transform.position.x, transform.position.y - GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.4f, 0) + Vector3.right * forwardDir * GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.55f+Vector3.right*0.01f, Color.blue, 1);
		//Debug.DrawLine(new Vector2(transform.position.x, transform.position.y + GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.4f), new Vector3(transform.position.x, transform.position.y + GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.4f, 0) + Vector3.right * forwardDir * GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.55f+Vector3.right*0.01f, Color.blue, 1);

		//Debug.DrawLine(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f*transform.localScale.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f)*raycastVerticalFlip), new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f*raycastVerticalFlip) - Vector2.up*raycastVerticalFlip * GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.01f, vertColor, 1);
		//Debug.DrawLine(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f*transform.localScale.x, transform.position.y - (GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f)*raycastVerticalFlip), new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x*transform.localScale.x * 0.4f, transform.position.y - GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.5f*raycastVerticalFlip) - Vector2.up*raycastVerticalFlip * GetComponent<BoxCollider2D>().size.y*transform.localScale.y * 0.01f, vertColor, 1);

		//Debug.DrawLine(new Vector3 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0), new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0) -Vector3.up*GetComponent<BoxCollider2D> ().size.y * 0.01f * transform.localScale.y*slopeHitExtender, Color.cyan, 1);
		//Debug.DrawLine(transform.position, new Vector3(0.01f, 0, 0) + transform.position, Color.red, 2);

		if(!inAir) { //check if there still is ground beneath the player's feet
			if(hit1.collider == null && hit2.collider == null && slopeHit.collider == null) {
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
					else if(hit2.collider != null) {
						hit2.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
					}
				}
				jump = false;
				downSpeed = 0;
				finalJumpTime = Time.time;
				//Debug.Log("Time: "+(-initialJumpTime+finalJumpTime));
				//Debug.Log("AngleX: "+angleX+" AngleY: "+angleY);
				if(angleX == 1){
					if(hit1.collider != null){
						transform.position = new Vector3(transform.position.x, hit1.collider.transform.position.y + (hit1.collider.GetComponent<BoxCollider2D>().size.y*0.5f*hit1.collider.transform.localScale.y+GetComponent<BoxCollider2D>().size.y*0.5f*transform.localScale.y)*raycastVerticalFlip, 0);
					}
					else if(hit2.collider != null) {
						transform.position = new Vector3(transform.position.x, hit2.collider.transform.position.y + (hit2.collider.GetComponent<BoxCollider2D>().size.y*0.5f*hit2.collider.transform.localScale.y+GetComponent<BoxCollider2D>().size.y*0.5f*transform.localScale.y)*raycastVerticalFlip, 0);
					}
				}
				else{
					transform.position = new Vector3(transform.position.x, Mathf.Tan(slopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad)*(transform.position.x - slopeHit.collider.transform.position.x)+ slopeHit.collider.transform.position.y + GetComponent<BoxCollider2D>().size.y*0.5f*transform.localScale.y + (slopeHit.collider.GetComponent<BoxCollider2D>().size.y*0.5f*slopeHit.transform.localScale.y/Mathf.Cos (slopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad)), 0);
				}
				yCorrected = true;
			}
		}

		if(Input.GetButtonDown("Jump") && !jump && !inAir) { //handles jumping
			initialJumpTime = Time.time;
			jump = true;
			inAir = true;
			downSpeed = 0.2f;
			hit1 = Physics2D.Raycast(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().size.x * 0.4f*transform.localScale.x, transform.position.y + (GetComponent<BoxCollider2D>().size.y* transform.localScale.y * 0.5f+0.01f)), Vector2.up, GetComponent<BoxCollider2D>().size.y * transform.localScale.y * 0.01f, groundLayerMask);
			hit2 = Physics2D.Raycast(new Vector2(transform.position.x - GetComponent<BoxCollider2D>().size.x * 0.4f*transform.localScale.x, transform.position.y + (GetComponent<BoxCollider2D>().size.y* transform.localScale.y * 0.5f+0.01f)), Vector2.up, GetComponent<BoxCollider2D>().size.y * transform.localScale.y * 0.01f, groundLayerMask);
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
				transform.position = new Vector3(hitForward1.collider.transform.position.x - (hitForward1.collider.GetComponent<BoxCollider2D>().size.x*0.5f*hitForward1.collider.transform.localScale.x+GetComponent<BoxCollider2D>().size.x*0.5f*transform.localScale.x)*forwardDir,transform.position.y,transform.position.z);
			}
			else{
				transform.position = new Vector3(hitForward2.collider.transform.position.x - (hitForward2.collider.GetComponent<BoxCollider2D>().size.x*0.5f*hitForward2.collider.transform.localScale.x+GetComponent<BoxCollider2D>().size.x*0.5f*transform.localScale.x)*forwardDir,transform.position.y,transform.position.z);
			}
			collision = 0;
		}
		GameObject.FindGameObjectWithTag("HP").GetComponent<Text>().text = "HP: " + Player.saveStuff.hp;

		//Debug.DrawLine(new Vector3 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0), new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0) -Vector3.up*GetComponent<BoxCollider2D> ().size.y * 0.01f * transform.localScale.y, Color.yellow, 1);
		//Debug.DrawLine(transform.position, new Vector3(0.01f, 0, 0) + transform.position, Color.blue, 2);

		//Debug.DrawLine(new Vector3 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0), new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0) -Vector3.up*GetComponent<BoxCollider2D> ().size.y * 0.01f * transform.localScale.y*slopeHitExtender, Color.cyan, 1);
		//Debug.DrawLine(transform.position, new Vector3(0.01f, 0, 0) + transform.position, Color.green, 2);

		if(Input.GetButtonDown("Fire1") && currentWeapon != "") {
			fireMethod();
		}

		if(Input.GetKeyDown(KeyCode.F5)) {
			SaveLoad.Save();
		}
		if(Input.GetKeyDown(KeyCode.F6)) {
			saveStuff = SaveLoad.Load();
		}
	}

	void FixedUpdate()
	{
		if(knockBackTime <= Time.time && knockBackX != 0) {
			knockBackX = 0;
		}

		rb2D.MovePosition(new Vector2(transform.position.x + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleX, transform.position.y + downSpeed + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleY));

		yMovement = (Input.GetAxis ("Horizontal") * moveSpeed * collision + knockBackX) * angleY;

		if(slopeHit.collider != null) {
			futureSlopeHit = Physics2D.Raycast(new Vector2(transform.position.x + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleX, transform.position.y - (GetComponent<BoxCollider2D>().size.y * transform.localScale.y * 0.5f)), -Vector2.up, GetComponent<BoxCollider2D>().size.y * transform.localScale.y * 0.05f, slopeLayerMask);
		}
		else {
			futureSlopeHit = new RaycastHit2D();
		}

		//Adjusts the player accordingly based on the next slope if there is one.
		if(futureSlopeHit.collider != null && futureSlopeHit.collider.transform.eulerAngles.z != slopeHit.collider.transform.eulerAngles.z) {
			rb2D.MovePosition(new Vector2(transform.position.x + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleX, Mathf.Tan(futureSlopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad)*(transform.position.x + (Input.GetAxis("Horizontal")*moveSpeed*collision+knockBackX)*angleX - futureSlopeHit.collider.transform.position.x)+ futureSlopeHit.collider.transform.position.y + GetComponent<BoxCollider2D>().size.y*0.5f*transform.localScale.y + (futureSlopeHit.collider.GetComponent<BoxCollider2D>().size.y*0.5f*futureSlopeHit.transform.localScale.y/Mathf.Cos (futureSlopeHit.collider.transform.eulerAngles.z*Mathf.Deg2Rad))));
		}

		mainCamTransform.GetComponent<Rigidbody2D>().MovePosition(new Vector2(mainCamTransform.position.x + (transform.position.x - mainCamTransform.position.x)*(transform.position.x - mainCamTransform.position.x)*(transform.position.x - mainCamTransform.position.x), mainCamTransform.position.y + (transform.position.y - mainCamTransform.position.y+2)*(transform.position.y - mainCamTransform.position.y+2)*(transform.position.y - mainCamTransform.position.y+2)));
		if(inAir) {
			if(!jump) {
				downSpeed = -maxDownSpeed;
			}
			else {
				downSpeed -= 0.015f;
				if(downSpeed <= -maxDownSpeed) {
					jump = false;
					downSpeed = -maxDownSpeed;
				}
			}
		}
		Debug.DrawLine(new Vector3 (transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0), new Vector3(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D> ().size.y * 0.5f * transform.localScale.y), 0) -Vector3.up*GetComponent<BoxCollider2D> ().size.y * 0.01f * transform.localScale.y, Color.yellow, 1);
		Debug.DrawLine(transform.position, new Vector3(0.01f, 0, 0) + transform.position, Color.blue, 2);
	}

	public void hurt(int dmg, float deltaInvuln, Vector2 dir)
	{
		if(invulnTime <= Time.time) {
			saveStuff.hp -= dmg;
			invulnTime += deltaInvuln;
			knockBackX = dir.x*0.15f;
			if(knockBackX != 0) {
				knockBackTime = Time.time + 0.2f;
			}
			if(saveStuff.hp <= 0){
				if(GameObject.FindGameObjectWithTag("GameManager")!= null){

				}
				else{
					Application.LoadLevel(Application.loadedLevel);
				}
			}
		}
	}

	public void dropWeapon()
	{
		holdingWeapon.GetComponent<SpriteRenderer>().sprite = null;
		holdingWeapon.transform.localScale = new Vector3(1, 1, 1);
		(Instantiate(weaponGameObject(currentWeapon), transform.position, Quaternion.identity) as GameObject).GetComponent<PickupMaster>().ammo = ammo;
		currentWeapon = null;
		ammo = 0;
	}

	public void pickupWeapon(string weapon, int weaponAmmo, Vector3 scale)
	{
		currentWeapon = weapon;
		ammo = weaponAmmo;
		holdingWeapon.GetComponent<SpriteRenderer>().sprite = weaponGameObject(weapon).GetComponent<SpriteRenderer>().sprite;
		holdingWeapon.transform.localScale = scale;
	}

	private GameObject weaponGameObject(string weapon)
	{
		switch (weapon) {
		case "stone":
			return weapons [0];
		case "gun":
			return weapons [1];
		}
		return null;
	}

	private void fireMethod()
	{
		switch (currentWeapon) {
		case "stone":
			throwWeapon (currentWeapon, 1);
			break;
		case "gun":
			if (ammo == 0) {
				throwWeapon (currentWeapon, ammo);
			}
			else{
				float xDiff = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x);
				float yDiff = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y);
				GameObject tempBull = (Instantiate(getBulletGO(currentWeapon), transform.position, Quaternion.identity) as GameObject);
				tempBull.AddComponent<Bullet>();
				tempBull.GetComponent<Bullet>().shootDir = new Vector2(xDiff/Mathf.Sqrt(xDiff*xDiff+yDiff*yDiff), yDiff/Mathf.Sqrt(xDiff*xDiff+yDiff*yDiff));
				tempBull.GetComponent<Bullet>().fireSpeed = 0.4f;
				tempBull.GetComponent<Bullet>().dmg = 5;
			}
			break;
		}
		ammo--;
	}

	private void throwWeapon(string weapon, int amm)
	{
		GameObject tempThrown = Instantiate(weaponGameObject(weapon), holdingWeapon.transform.position, Quaternion.identity) as GameObject;
		tempThrown.GetComponent<PickupMaster> ().thrown = true;
		tempThrown.GetComponent<PickupMaster> ().ammo = amm;
		float xDiff = (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - holdingWeapon.transform.position.x);
		float yDiff = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - holdingWeapon.transform.position.y);
		tempThrown.GetComponent<PickupMaster>().throwDir = new Vector2(xDiff/Mathf.Sqrt(xDiff*xDiff+yDiff*yDiff), yDiff/Mathf.Sqrt(xDiff*xDiff+yDiff*yDiff));
		currentWeapon = "";
		holdingWeapon.GetComponent<SpriteRenderer>().sprite = null;
	}

	private GameObject getBulletGO(string weapon)
	{
		switch (weapon) {
		case "gun":
			return bullets[1];
		}
		return null;
	}
}
