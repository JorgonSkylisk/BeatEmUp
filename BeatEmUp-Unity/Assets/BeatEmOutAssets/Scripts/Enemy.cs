using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
	protected float maxSpeed;

	public float minHeight,maxHeight;
	public float damageTime = 0.7f;
	public int maxHealth;
	public float attackRate = 1f;
	public string enemyName;
	public Sprite enemyImage;
	public AudioClip collisionSound, deathSound;

	private int currentHealth;
	protected float currentSpeed;
	protected Rigidbody rb;
	protected Animator anim;
	private Transform groundCheck;
	private bool onGround;

	public bool facingRight = false;
	protected Transform target;              //player.
	protected Transform targetHitBox;       // the players attack hitbox
	public Transform newTarget;         // location of enemy's target
	protected bool isDead = false;
	private float zForce;
	private float walkTimer;            // timer that determines the ememy's delay between movement
	protected bool damaged = false;     // has the actor has been hit
	private float damageTimer;          // time stunned after being hit
	private float nextAttack;
	private CapsuleCollider collider;
	private AudioSource audioS;

	public int damageCount;             // counter that track how many times the character is hit before being knocked down
	public bool highDamage;             // state where the character is knocked down due to taking a certain amount of damage
	public bool beHold = false;         // the state when the character is grappled by the player

	public virtual void Start () 
	{
		currentSpeed = maxSpeed;
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		groundCheck = transform.Find("GroundCheck");
		target = FindObjectOfType<Player> ().transform; 
		newTarget = GameObject.Find ("newTarget").transform;
		targetHitBox = GameObject.FindGameObjectWithTag("PlayerAttack").transform;
		currentHealth = maxHealth;
		collider = GetComponent<CapsuleCollider> ();
		audioS = GetComponent<AudioSource>();
	}
	

	public virtual void Update ()
	{
		onGround = Physics.Linecast (transform.position,groundCheck.position,1<<LayerMask.NameToLayer("Ground"));
		anim.SetBool ("Grounded",onGround);
		anim.SetBool ("Dead",isDead);
		anim.SetBool ("BeHold",beHold);

		if (beHold) 
		{
			Invoke ("NotBeHold",0.5f); // after a half second the character checks to see if the grapple has ended after a half second
		}

		if(!isDead)         // tranform the sprite depending on the direction they are facing 
		{
			facingRight = (target.position.x < transform.position.x) ? false : true;
			
			if (facingRight&&onGround&&!damaged&&!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy1 Attack"))
			{
				Invoke ("Right",0.5f);
			}
			else if(!facingRight&&onGround&&!damaged&&!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy1 Attack"))
			{
				Invoke("Left",0.5f);
			}
        

        }

		if (damaged && !isDead)         // if the character is hit and isn't dead they are stunned for a short time
		{
			damageTimer += Time.deltaTime;
			if(damageTimer>= damageTime || target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Damage"))
			{
				damaged = false;
				damageTimer = 0;
				damageCount = 0;
			}
		}


		walkTimer += Time.deltaTime;

		if(transform.position.y < -10) // if they somehow fall out of world they get killed
		{
			this.gameObject.SetActive(false);
			TookDamage (50);
		}
	}

	protected Vector3 newTargetDistance;
	protected Vector3 targetDistance;
	public LayerMask enemyMask;
	RaycastHit hit;
	protected float hForce;
	public virtual void FixedUpdate()
	{
		if(damageCount >= 4) // after being hit by a powerful attack the character is knocked down to the ground 
		{
			anim.SetTrigger ("HighDamage");
			highDamage = true;
            damageCount = 0;
            Invoke("NotHighDamage",0.05f);
			if(!isDead && Time.timeScale == 1.0f)
			{
				rb.AddRelativeForce(new Vector3(1.5f,0.7f,0),ForceMode.Impulse);
			}
			collider.height = 0.1f;
		} 

		Debug.DrawRay (transform.position,transform.forward*10,Color.red);

		if (!isDead&&onGround) 
		{
			newTargetDistance = newTarget.position - transform.position;
			targetDistance = target.position - transform.position;
			hForce = newTargetDistance.x / Mathf.Abs (newTargetDistance.x);
		    

			if(Physics.Raycast(transform.position,transform.forward,out hit,GetComponent<CapsuleCollider>().radius+3f,enemyMask)||
			    Physics.Raycast(transform.position,-transform.forward,out hit,GetComponent<CapsuleCollider>().radius+3f,enemyMask))
			{
				zForce = -2f*(hit.point - transform.position).normalized.z;
			}
			else
			{
				if(walkTimer >= Random.Range (0.5f,0.8f))
				{
					zForce = Random.Range(-1f,1.5f);
					walkTimer = 0;
				}
			}

			if(Mathf.Abs (newTargetDistance.x) < 0.2f)
			{
				hForce = 0;
				if(newTargetDistance.z!=0)
				zForce = targetDistance.z/Mathf.Abs (newTargetDistance.z);
			}

			if(!damaged&&!highDamage && !beHold&&
				!target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HighDamage1")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack3")
			   )
			{
				rb.velocity = new Vector3(hForce*currentSpeed, rb.velocity.y, zForce * currentSpeed);
			}

			anim.SetFloat ("Speed",Mathf.Abs (rb.velocity.magnitude));

			if(Mathf.Abs (targetDistance.x)<=2.5f && Mathf.Abs (targetDistance.z) < 1.5f &&Time.time > nextAttack&&!damaged&&!highDamage&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HighDamage1")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hold Enemy")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack3")&&
			   !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
			{
				anim.SetTrigger("Attack");
				currentSpeed = 0;
				nextAttack = Time.time + attackRate;
			}
		}

		rb.position = new Vector3(rb.position.x,
		                          rb.position.y,
		                          Mathf.Clamp(rb.position.z,minHeight,maxHeight));
	}

	public void TookDamage(int damage)
	{
		transform.position = new Vector3 (transform.position.x,transform.position.y,target.position.z);

		if (!isDead) 
		{
			damageCount += 1;
			damaged = true;
			currentHealth -= damage;
			anim.SetTrigger ("HitDamage");



			if(target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
			{
				rb.AddForce (new Vector3(0,7,0),ForceMode.Impulse);
			}

			if(target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack3")&&!highDamage)
			{
				damageCount +=4;
			}

			if(target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Voadera"))
			{

				damageCount += 4;
			}

			if(target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HoldAttack"))
			{

				rb.AddRelativeForce (new Vector3(-7,3f,0),ForceMode.Impulse);
				damageCount += 4;
			}

            if (target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HoldAttack1"))
            {

                rb.AddRelativeForce(new Vector3(3, 10f, 0), ForceMode.Impulse);
                damageCount += 4;
            }

            if (target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HoldAttack2"))
            {

                rb.AddRelativeForce(new Vector3(-10f, 5f, 0), ForceMode.Impulse);
                damageCount += 4;
                if (facingRight)
                {
                    Invoke("Left", 0.5f);
                }
                else if (!facingRight)
                {
                    Invoke("Right", 0.5f);
                }
            }

            if (target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("HoldAttack3"))
            {

                rb.AddRelativeForce(new Vector3(-1f, 2f, 0), ForceMode.Impulse);
                damageCount += 4;
            }

            if (target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
			{
				transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
			}

			PlaySong (collisionSound);
			UIManager.instance.UpdateEnemyUI(maxHealth,currentHealth,enemyName,enemyImage);
			if(currentHealth <= 0)
			{
				isDead = true;
				rb.AddRelativeForce(new Vector3(4.5f,3.5f,0),ForceMode.Impulse);
				PlaySong (deathSound);
			}
		}
	}

	public void DisableEnemy()
	{
		gameObject.SetActive (false);
	}

	void ResetSpeed()
	{
		if (onGround&&!damaged) 
		{
			currentSpeed = maxSpeed;
		}
	}

	void ZeroSpeed()
	{
		if (onGround)
		{
			currentSpeed = 0f;
		}
	}

	void NotHighDamage()
	{
		damageCount = 0;
		highDamage = false;
		collider.height = 0.7f;
	}
	void NotBeHold()
	{
		beHold = false;
	}

	void Right()
	{
		transform.eulerAngles = new Vector3 (0, 180, 0);
	}

	void Left()
	{
		transform.eulerAngles = new Vector3(0,0,0);
	}

	public void PlaySong(AudioClip clip)
	{
		audioS.clip = clip;
		audioS.Play ();
	}

	void ignoreLayer()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Player");
	}
	
	void ResetLayer()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Enemy");
	}
}
