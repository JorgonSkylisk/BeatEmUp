﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour 
{
	//public EasyJoystick joystick;

	public float maxSpeed = 4;
	public float jumpForce = 1000;
	public float minHeight, maxHeight;
	public int maxHealth = 10;
	public string playerName;
	public Sprite playerImage;
	public AudioClip collisionSound,jumpSound,healthItem,deadSound;
	public Weapon weapon;
	public int damageCount;

	private int currentHealth;
	private float currentSpeed;
	private Rigidbody rb;
	private Animator anim;
	private Transform groundCheck;
	public bool onGround;
	public bool isDead = false;
	public bool facingRight = true;
	private bool jump = false; 
	private AudioSource audioS;
	public bool holdingWeapon = false;
	private float weaponAttackTime;
	public bool highDamage;
	public bool canAttack = true;


	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		groundCheck = gameObject.transform.Find ("GroundCheck");
		currentSpeed = maxSpeed;
		currentHealth = maxHealth;
		audioS = GetComponent<AudioSource>();
		//joystick = GameObject.FindObjectOfType<EasyJoystick>();
		//EasyButton.On_ButtonDown += HandleOn_ButtonDown;
	}
	
	//void OnEnable()
	//{
		//EasyButton.On_ButtonDown += HandleOn_ButtonDown;
	//}

	//void HandleOn_ButtonDown(string buttonName)
	//{
		//if (buttonName == "AttackButton") 
		//{
			//Attack();
		//}
		//else if(buttonName == "JumpButton")
		//{
		//	Jump ();
		//}
	//}

	void Update () 
	{
		HoldEnemy ();

		onGround = Physics.Linecast (transform.position,groundCheck.position,1<< LayerMask.NameToLayer("Ground"));

		anim.SetBool ("OnGround", onGround);
		anim.SetBool ("Dead", isDead);
		anim.SetBool ("Weapon",holdingWeapon);

		if (Input.GetButtonDown("Jump"))//地面攻击不能跳
		{
			Jump ();
		}

		if (!holdingWeapon) 
		{
			if (Input.GetKeyDown (KeyCode.J)&&canAttack /*|| CrossPlatformInputManager.GetButtonDown ("Attack")*/ ) 
			{
				Attack ();
			}
		}
		else if (holdingWeapon) 
		{
			if (Input.GetKey (KeyCode.J)/*|| CrossPlatformInputManager.GetButton("Attack")*/) 
			{
				if(weaponAttackTime>0.3f)
				{
					weaponAttackTime = 0;
					Attack ();
				}
				else
				{
					weaponAttackTime += Time.deltaTime;
				}
			}
		}
	}

	float h;
	float z;
	private void FixedUpdate()
	{
		if (damageCount >= 3) 
		{
			highDamage = true;
			anim.SetTrigger("HighDamage");
			damageCount = 0;
			Invoke("NotHighDamage",0.05f);
			SetHoldingWeaponToFalse();
			FindObjectOfType<Weapon>().gameObject.GetComponent<SpriteRenderer>().sprite = null;
		}

		if (highDamage) 
		{
			if(!isDead)
			{
				if(facingRight)
				{
					rb.AddForce (new Vector3(-1.5f,2.5f,0),ForceMode.Impulse);
				}
				else
				{
					rb.AddForce (new Vector3(1.5f,2.5f,0),ForceMode.Impulse);
				}
			}
		}

		if (!isDead) 
		{
			//float h = Input.GetAxis ("Horizontal");
			//float z = Input.GetAxis ("Vertical");
			//float h = joystick.JoystickAxis.x;
			//float z = joystick.JoystickAxis.y;
			if(!highDamage&&onGround&&
			   !anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
			{
				h = CrossPlatformInputManager.GetAxis ("Horizontal");
				z = CrossPlatformInputManager.GetAxis ("Vertical");
			}

			if(anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2"))
			{
				rb.velocity = Vector3.zero;
			}

			if(!onGround)
			{
				z=0;
			}
			if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Damage")&&!highDamage&&onGround)
			{
				rb.velocity = new Vector3(h*currentSpeed, rb.velocity.y, z*currentSpeed);
			}

			if(onGround)
			{
				anim.SetFloat ("Speed",Mathf.Abs (rb.velocity.magnitude));
			}

			if(h>0&&!facingRight&&!highDamage&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
			{
				Flip();
			}
			else if(h<0&&facingRight&&!highDamage&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
			{
				Flip ();
			}

			if(jump)
			{
				jump = false;
				rb.AddForce (Vector3.up * jumpForce);
				//PlaySong (jumpSound);
			}
			float minWidth = Camera.main.ScreenToWorldPoint(new Vector3(0,0,10)).x;
			float maxWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0,10)).x;
			rb.position = new Vector3(Mathf.Clamp(rb.position.x,minWidth+1,maxWidth-1),
				                       rb.position.y,
				                       Mathf.Clamp(rb.position.z,minHeight,maxHeight));
		}
	}

	public void Flip()
	{
		if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("DefaultAttack"))
		{
			facingRight = !facingRight;

			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;
		}
	}

	void ZeroSpeed()
	{
		currentSpeed = 0;
	}

	void ResetSpeed()
	{
		currentSpeed = maxSpeed;
	}

	void WeaponSpeed()
	{
		currentSpeed = 0.5f * maxSpeed;
	}

	void PlayerRespawn()
	{
		if (FindObjectOfType<GameManager> ().lives > 0) 
		{
			isDead = false;
			UIManager.instance.UpdateLives ();
			currentHealth = maxHealth;
			UIManager.instance.UpdateHealth (currentHealth);
			anim.Rebind ();
			float minWidth = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 10)).x;
			transform.position = new Vector3 (minWidth, 10, -4);
		}
		else 
		{
			UIManager.instance.UpdateDisplayMessage("Game Over");
			Destroy(FindObjectOfType<GameManager>().gameObject,2f);
			Invoke ("LoadScene",2f);
		}
	}
		

	public void TookDamage(int damage)
	{
		if(!isDead)
		{
			if (!onGround) {
				damageCount += 4;
			} else {
				damageCount+=1;
			}

			currentHealth -= damage;
			anim.SetTrigger("HitDamage");
			UIManager.instance.UpdateHealth(currentHealth);
			PlaySong (collisionSound);
			if(currentHealth<=0)
			{
				damageCount = 0; 
				isDead = true;
				FindObjectOfType<GameManager>().lives -- ;
				holdingWeapon = false;
				FindObjectOfType<Weapon>().gameObject.GetComponent<SpriteRenderer>().sprite = null;

				PlaySong(deadSound);
				if(facingRight)
				{
					rb.AddForce(new Vector3(-3,5,0),ForceMode.Impulse);
				}
				else
				{
					rb.AddForce(new Vector3(3,5,0),ForceMode.Impulse);
				}
			}
		}
	}

	public void PlaySong(AudioClip clip)
	{
		audioS.clip = clip;
		audioS.Play ();
	}

	private void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Health Item"))
		{
			//if(Input.GetKeyDown(KeyCode.E))
			//{
				Destroy (other.gameObject);
				anim.SetTrigger ("Catching");
				PlaySong(healthItem);
				currentHealth = maxHealth;
				UIManager.instance.UpdateHealth(currentHealth);
			//}
		}

		if(other.CompareTag("Weapon"))
		{
			if(/*Input.GetKeyDown (KeyCode.E)&&*/!anim.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
			{
				anim.SetTrigger("Catching");
				holdingWeapon = true;
				WeaponItem weaponItem = other.GetComponent<PickableWeapon>().weapon;
				weapon.ActivateWeapon(weaponItem.sprite,weaponItem.color,weaponItem.durability,weaponItem.damage);
				Destroy (other.gameObject);
			}
		}
	}

	void Attack()
	{
		anim.SetTrigger ("Attack");
	}

	void Jump()
	{
		if(onGround &&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")&&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1 0")&&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")&&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")&&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1")&&
		   !anim.GetCurrentAnimatorStateInfo(0).IsName ("HighDamage2")&&
		   holdingWeapon == false)
		{
			jump = true;
		}
	}
	
	void PlayAttackSound()
	{
		audioS.clip = jumpSound;
		audioS.Play ();
	}

	Ray ray;
	RaycastHit hit;

	float rayDis; 

	void HoldEnemy()
	{
		rayDis = GetComponent<CapsuleCollider> ().radius +1f;
		ray.origin = transform.position + transform.up * 1f;

		if (facingRight)
		{
			ray.direction = transform.right;
			//Debug.DrawRay(transform.position+ transform.up * 1f,transform.right*rayDis,Color.blue);
		} 
		else if(!facingRight)
		{
			ray.direction = -transform.right;
			//Debug.DrawRay(transform.position+ transform.up * 1f,-transform.right*rayDis,Color.blue);
		}


		if (Physics.Raycast (ray, out hit, rayDis, 1 << LayerMask.NameToLayer ("Enemy"))&&onGround && 
		    !anim.GetCurrentAnimatorStateInfo(0).IsName("Damage")&&
		    !anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1")&&
		    !anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&
		    !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")&&
		    !anim.GetCurrentAnimatorStateInfo(0).IsName("Voadera")&&
			!holdingWeapon
		    ) 
		{
			//Debug.Log ("Hold");
			Debug.DrawRay(transform.position+ transform.up * 1f,ray.direction*rayDis,Color.blue);
			Enemy holdEnemy = hit.collider.gameObject.GetComponent<Enemy> ();
			if(holdEnemy.tag != "Boss")
			{
				if (holdEnemy != null) {
					//Debug.Log (holdEnemy.gameObject.name);
					holdEnemy.beHold = true;
					anim.SetBool ("HoldEnemy",true);
				}
			}
		} 
		else 
		{
			anim.SetBool("HoldEnemy",false);
		}
	}

	void LoadScene()
	{
		SceneManager.LoadScene (0);
	}

	public void SetHoldingWeaponToFalse()
	{
		holdingWeapon = false;
	}	

	public void TimeScale()
	{
		Time.timeScale = 0.4f;
		Time.fixedDeltaTime = 0.04f * Time.timeScale;
	}

	public void ResteTimeScale()
	{
		Time.timeScale = 1f;
	}

	public void SetCombo()
	{
		anim.SetBool ("Combo", false);
	}

	void Impulse()
	{
		//Handheld.Vibrate ();
	}

	void NotHighDamage()
	{
		highDamage = false;
	}

	void ignoreLayer()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Enemy");
	}

	void ResetLayer()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Player");
	}

	void FalseAttack()
	{
		canAttack = false;
	}
	void TrueAttack()
	{
		canAttack = true;
	}
}
