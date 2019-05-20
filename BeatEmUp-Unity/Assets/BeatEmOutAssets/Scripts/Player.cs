using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour 
{

	public float maxSpeed = 6;
	public float jumpForce = 500;
	public float minHeight, maxHeight;
	public int maxHealth = 10;
    public int maxSuper = 100;
	public string playerName;
	public Sprite playerImage;
	public AudioClip collisionSound,jumpSound,healthItem,deadSound;
	public Weapon weapon;
	public int damageCount;

	private int currentHealth;
	private float currentSpeed;
    private int currentSuper;
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
    public static float unscaledDeltaTime = 1.0f;
    public bool horizontalMovement = false;
    public bool upMovement = false;
    public bool downMovement = false;
    private float animNormalSpeed = 1f;
    private float animSlowSpeed = 5f;
    private float animStopSpeed = 50f;
    private float maxNormalSpeed = 6;
    private float SlowMultiplier;
    private float normalJump = 500;

    void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		groundCheck = gameObject.transform.Find ("GroundCheck");
		currentSpeed = maxSpeed;
		currentHealth = maxHealth;
        currentSuper = maxSuper;
		audioS = GetComponent<AudioSource>();
        SlowMultiplier = (float)(Time.deltaTime * (1 + (1.0 - Time.timeScale)));
    }
	


	void Update () 
	{
		HoldEnemy ();

		onGround = Physics.Linecast (transform.position,groundCheck.position,1<< LayerMask.NameToLayer("Ground"));

		anim.SetBool ("OnGround", onGround);
		anim.SetBool ("Dead", isDead);
		anim.SetBool ("Weapon",holdingWeapon);
        anim.SetBool("Down", downMovement);
        anim.SetBool("Up", upMovement);
        anim.SetBool("Move", horizontalMovement);



        // Adjust fixed delta time according to timescale
        // The fixed delta time will now be 0.02 frames per real-time second
        Time.fixedDeltaTime = 0.02f * Time.timeScale;


        if (Input.GetButtonDown("Jump"))
		{
			Jump ();
		}


        if (Input.GetKeyDown(KeyCode.W))
        {
            upMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            upMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            downMovement = true;
        }else if (Input.GetKeyUp(KeyCode.S))
        {
            downMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            horizontalMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            horizontalMovement = false;
        }

        /// TIME SLOW

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Time.timeScale == 1.0f && currentSuper != 0)
            {
                Time.timeScale = 0.2f;
                anim.speed = animSlowSpeed;
                maxSpeed = maxNormalSpeed * 5;
                jumpForce = jumpForce * 5;
                currentSuper -= 10;
                StartCoroutine(wait_slowTime());
            }
            else
            {
                Time.timeScale = 1.0f;
                anim.speed = animNormalSpeed;
                maxNormalSpeed = 6;
                jumpForce = normalJump;
                
            }
            // Adjust fixed delta time according to timescale
            // The fixed delta time will now be 0.02 frames per real-time second

            //Time.maximumDeltaTime = 0.01f;
            UIManager.instance.UpdateSuper(currentSuper);
        }


        /// TIME STOP

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Time.timeScale == 1.0f && currentSuper > 30)
            {
                
                
                Time.timeScale = 0.02f;
                anim.speed = animStopSpeed;
                maxSpeed = maxNormalSpeed * 50;
                currentSuper -= 30;
                StartCoroutine(wait_stopTime());
            }
            else
            {
                Time.timeScale = 1.0f;
                anim.speed = animNormalSpeed;
                currentSpeed = maxSpeed;

            }

            UIManager.instance.UpdateSuper(currentSuper);
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

    IEnumerator wait_speedUp()
    {
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1.0f;
        anim.speed = animNormalSpeed;
        maxSpeed = maxNormalSpeed;
        jumpForce = normalJump;
    }

    IEnumerator wait_slowTime()
    {
        yield return new WaitForSecondsRealtime (5);
        Time.timeScale = 1.0f;
        anim.speed = animNormalSpeed;
        maxSpeed = maxNormalSpeed;
        jumpForce = normalJump;
    }

    IEnumerator wait_stopTime()
    {
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1.0f;
        anim.speed = animNormalSpeed;
        maxSpeed = maxNormalSpeed;
        jumpForce = normalJump;
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
            if(!highDamage&&onGround&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2") || !anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
            {
                  h = CrossPlatformInputManager.GetAxisRaw ("Horizontal");
                  z = CrossPlatformInputManager.GetAxisRaw ("Vertical");
            }


    
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2"))
			{
				rb.velocity = Vector3.zero;
			}

			if(!onGround)
			{
				z=0;
			}
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
            {
               rb.velocity = new Vector3(h * maxSpeed, rb.velocity.y, z * maxSpeed);
            }

            if (onGround)
			{
				anim.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));
			}

			if(h>0&&!facingRight&&!highDamage&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
			{
				Flip();
            }
			else if(h<0&&facingRight&&!highDamage&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage2")&&!anim.GetCurrentAnimatorStateInfo(0).IsName("HighDamage1"))
			{
				Flip ();
            }


            if (jump)
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
		if(!anim.GetCurrentAnimatorStateInfo(0).IsTag("JumpAttack"))
		{
			facingRight = !facingRight;

			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;
		}
	}

	void ZeroSpeed()
	{
		maxSpeed = 0;
	}

	void ResetSpeed()
	{
        if (Time.timeScale == 0.2f)
        {
            maxSpeed = maxNormalSpeed * 5;


        }
        else if (Time.timeScale == 0.02f)
        {
            maxSpeed = maxNormalSpeed * 50;


        }else
        maxSpeed = maxNormalSpeed;
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

        if (other.CompareTag("Super Item"))
        {
            //if(Input.GetKeyDown(KeyCode.E))
            //{
            Destroy(other.gameObject);
            anim.SetTrigger("Catching");
            PlaySong(healthItem);
            currentSuper = maxSuper;
            UIManager.instance.UpdateSuper(currentSuper);
            //}
        }

        if (other.CompareTag("Lives Item"))
        {
            //if(Input.GetKeyDown(KeyCode.E))
            //{
            Destroy(other.gameObject);
            anim.SetTrigger("Catching");
            PlaySong(healthItem);
            FindObjectOfType<GameManager>().lives +=1;
            UIManager.instance.UpdateLives();
            //}
        }

        if (other.CompareTag("Speed Item"))
        {
            //if(Input.GetKeyDown(KeyCode.E))
            //{
            Destroy(other.gameObject);
            anim.SetTrigger("Catching");
            PlaySong(healthItem);
            anim.speed = animNormalSpeed * 3f;
            maxSpeed = maxSpeed * 2f;
            StartCoroutine(wait_slowTime());
            //}
        }

        if (other.CompareTag("Weapon"))
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
           !anim.GetCurrentAnimatorStateInfo(0).IsTag("HoldEnemy")&&

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
