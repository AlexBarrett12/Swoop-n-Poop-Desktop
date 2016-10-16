using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickupMaster : MonoBehaviour {

	private Rigidbody2D rb2D;
	private int maxHealth = 10;
	private bool dontDestroy;  //stops the gameobject from being destroyed when the player walks over this item.
	private bool pickup;  //handles pickup reliability
	private bool reset;  //resets the pickup bool
	private float maxDownSpeed = 0.2f;
	private RaycastHit2D xCast;  //Casts a ray in the x direction of motion.
	private RaycastHit2D yCast;  //Casts a ray in the y direction of motion

	public string PickupType;  //handles how the player interacts with this pickupable item
	public string weaponName;  //specfies the type of weapon that this item is, if applicable.
	public bool thrown;  //handles whether the object has been thrown or not.
	public Vector2 throwDir;
	public int ammo;  //ammo left in this weapon. For instance a gun might have 0 bullets left.
	public int dmg = 5;
	public LayerMask groundMask;

	void Start()
	{
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if(reset) {
			pickup = false;
			reset = false;
		}
		if(Input.GetButtonDown("PickupItem") && !thrown) {
			pickup = true;
		}
		if(thrown) {
			xCast = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, throwDir.x, groundMask);
			yCast = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, throwDir.y, groundMask);

			if(xCast.collider != null) {
				int reversePositioning = 1;
				if(throwDir.x > 0) {
					reversePositioning = -1;
				}
				throwDir = new Vector2(0, throwDir.y);
				transform.position = new Vector3(xCast.collider.transform.position.x + reversePositioning*(xCast.collider.GetComponent<BoxCollider2D>().size.x*0.5f*xCast.collider.transform.localScale.x + GetComponent<BoxCollider2D>().size.x*0.5f*transform.localScale.x), transform.position.y, 0);
			}
			if(yCast.collider != null){
				if(throwDir.y < 0) {
					thrown = false;
				}
				throwDir = Vector2.zero;
			}
		}
	}

	void FixedUpdate()
	{
		reset = true;
		if(PickupType == "weapon" && thrown) {
			rb2D.MovePosition(new Vector2(transform.position.x + throwDir.x*0.2f, transform.position.y + throwDir.y));
			throwDir = new Vector2(throwDir.x, throwDir.y -0.01f);
			if(throwDir.y >= maxDownSpeed) {
				throwDir.y = maxDownSpeed;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D obj)
	{
		if(obj.tag == "Player") {
			dontDestroy = false;
			switch (PickupType) {
				case "health":
					if(obj.GetComponent<Player>().hp == maxHealth) {
						dontDestroy = true;
						GameObject.FindGameObjectWithTag("PickupText").GetComponent<Text>().text = "Already at Max Health";
					}
					else if(obj.GetComponent<Player>().hp + 5 > maxHealth) {
						obj.GetComponent<Player>().hp = maxHealth;
					}
					else {
						obj.GetComponent<Player>().hp += 5;
					}
					break;
				default:
					dontDestroy = true;
					break;
			}
			if(!dontDestroy) {
				Destroy(gameObject);
			}
		}
		if(obj.tag == "Enemy" && thrown) {
			obj.GetComponent<BirdAI>().hurt(dmg);
		}
	}

	void OnTriggerStay2D(Collider2D obj)
	{
		if(obj.tag == "Player" && !thrown) {
			dontDestroy = false;
			switch (PickupType) {
				case "weapon":
					if(pickup) {
						if(obj.GetComponent<Player>().currentWeapon != "") {
							obj.GetComponent<Player>().dropWeapon();
						}
						obj.GetComponent<Player>().pickupWeapon(weaponName, ammo, transform.localScale);
						GameObject.FindGameObjectWithTag("PickupText").GetComponent<Text>().text = "";
					}
					else {
						GameObject.FindGameObjectWithTag("PickupText").GetComponent<Text>().text = "Press E";
						dontDestroy = true;
					}
					break;
				default:
					dontDestroy = true;
					break;
			}
			if(!dontDestroy) {
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerExit2D(Collider2D obj)
	{
		if(obj.tag == "Player" && !thrown) {
			GameObject.FindGameObjectWithTag("PickupText").GetComponent<Text>().text = "";
		}
	}
}
