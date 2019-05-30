using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public int damage;
	public float punchForce;
	public GameObject hitEffect;
	public Transform hitEffectPos;

	public Animator anim;

	void Start () 
	{

	}
	

	void Update () 
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		Enemy enemy = other.GetComponent<Enemy> ();
		Player player = other.GetComponent<Player> ();

		if (enemy != null) 
		{

				anim.SetBool ("Combo", true); // player can continue the attack string so long as the enemy is in the collider

		
			enemy.TookDamage(damage);
			Instantiate(hitEffect,hitEffectPos.position,Quaternion.identity);
		}

		if (player != null) 
		{
			player.TookDamage(damage);
			if (transform.position.x - player.transform.position.x > 0) // if the player is facing away from the attack flip them around to face the attack
			{
				if (!player.facingRight) {
					player.Flip ();
				}
			} 
			else 
			{
				if (player.facingRight) {
					player.Flip ();
				}
			}
		}

	}	
}
