using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour 
{
	public int direction = 1;

	private Rigidbody rb;


	void Start ()
	{
		rb = GetComponent<Rigidbody> ();

	}
	

	void FixedUpdate () 
	{
		rb.velocity = new Vector3 (6 * direction, 0, 0 * direction);
	}

}
